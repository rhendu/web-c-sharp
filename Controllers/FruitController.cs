using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FruitController : ControllerBase
    {
        private readonly string _connectionString;

        public FruitController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // 🔹 POST /api/fruit/add
        [HttpPost("add")]
        public IActionResult InsertFruit([FromBody] Fruit fruit)
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                using var cmd = new MySqlCommand("CALL insert_fruit(@name, @type, @price, @stock)", conn);
                cmd.Parameters.AddWithValue("@name", fruit.Name);
                cmd.Parameters.AddWithValue("@type", fruit.Type);
                cmd.Parameters.AddWithValue("@price", fruit.Price);
                cmd.Parameters.AddWithValue("@stock", fruit.Stock);

                cmd.ExecuteNonQuery();

                return Ok(new { message = "Fruit inserted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Insert Error: {ex.Message}");
            }
        }

        // 🔹 GET /api/fruit
        [HttpGet]
        public ActionResult<List<Fruit>> GetFruits()
        {
            var fruits = new List<Fruit>();

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                using var cmd = new MySqlCommand("CALL get_all_fruits()", conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    fruits.Add(new Fruit
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        Type = reader.GetString("type"),
                        Price = reader.GetDecimal("price"),
                        Stock = reader.GetInt32("stock")
                    });
                }

                return Ok(fruits);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fetch Error: {ex.Message}");
            }
        }

        // 🔹 PUT /api/fruit/update/{id}
        [HttpPut("update/{id}")]
        public IActionResult UpdateFruit(int id, [FromBody] Fruit fruit)
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                using var cmd = new MySqlCommand("CALL update_fruit(@id, @name, @type, @price, @stock)", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", fruit.Name);
                cmd.Parameters.AddWithValue("@type", fruit.Type);
                cmd.Parameters.AddWithValue("@price", fruit.Price);
                cmd.Parameters.AddWithValue("@stock", fruit.Stock);

                cmd.ExecuteNonQuery();

                return Ok(new { message = "Fruit updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Update Error: {ex.Message}");
            }
        }

        // 🔹 DELETE /api/fruit/delete/{id}
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteFruit(int id)
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                using var cmd = new MySqlCommand("DELETE FROM fruits WHERE id = @id", conn);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();

                return Ok(new { message = "Fruit deleted successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Delete Error: " + ex.Message);
                return StatusCode(500, $"Delete Error: {ex.Message}");
                
            }
        }
    }
}
