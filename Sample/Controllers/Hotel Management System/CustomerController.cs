
using Microsoft.AspNetCore.Mvc;
using Sample.Models.Hotel_Management_System;
using Npgsql;

namespace Sample.Controllers.Hotel_Management_System
{
    [Route("hms/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly string connectionString = "Host=localhost; Port=5432;  Database=Training; Username=postgres; Password=123456789";

        [HttpPost]
        public IActionResult CreateCustomer(Customer customer)
        {
            if (customer == null)
            {
                return BadRequest("Customer data is null.");
            }
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("Connected to PostgreSQL!");
            try
            {
                Npgsql.NpgsqlCommand cmd = new NpgsqlCommand("insert into customer(cnic, name, phone, email) values(@cnic, @name, @phone, @email)", conn);
                cmd.Parameters.AddWithValue("cnic", customer.Cnic);
                cmd.Parameters.AddWithValue("name", customer.Name);
                cmd.Parameters.AddWithValue("phone", customer.Phone);
                cmd.Parameters.AddWithValue("email", customer.Email);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Customer inserted");
                return CreatedAtAction(nameof(CreateCustomer), new { id = customer.Cid }, customer);
            }
            catch (Npgsql.PostgresException ex)
            {
                Console.WriteLine($"Postgres error: {ex.Message}");
                return Conflict(new { message = "Data already exists" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error while inserting customer data.");
            }
        }

        [HttpGet]
        public IActionResult GetAllCustomers(int limit = 2, int offset = 0)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("Connected to PostgreSQL!");
            try
            {
                string query = $"SELECT * FROM customer order by cid limit {limit} offset {offset}";
                if (limit == null || offset == null)
                {
                    limit = 2;
                    offset = 0;
                }
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                Npgsql.NpgsqlDataReader data = cmd.ExecuteReader();
                List<Customer> customers = new List<Customer>();
                while (data.Read())
                {
                    Customer customer = new Customer
                    {
                        Cnic = data.GetString(data.GetOrdinal("cnic")),
                        Name = data.GetString(data.GetOrdinal("name")),
                        Phone = data.GetString(data.GetOrdinal("phone")),
                        Email = data.GetString(data.GetOrdinal("email"))
                    };
                    customers.Add(customer);
                }
                return Ok(customers);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error while retrieving customer data.");
            }
        }

        [HttpGet("{cnic}")]
        public IActionResult GetCustomerById(string cnic)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("Connected to PostgreSQL!");
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("select * from customer where cnic = @cnic", conn);
                cmd.Parameters.AddWithValue("cnic", cnic);
                NpgsqlDataReader data = cmd.ExecuteReader();
                if (data.Read())
                {
                    Customer customer = new Customer
                    {
                        Cnic = data.GetString(data.GetOrdinal("cnic")),
                        Name = data.GetString(data.GetOrdinal("name")),
                        Phone = data.GetString(data.GetOrdinal("phone")),
                        Email = data.GetString(data.GetOrdinal("email"))
                    };
                    return Ok(customer);
                }
                else
                {
                    return NotFound($"Customer with Cnic {cnic} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error while retrieving customer data.");
            }
        }

        [HttpDelete("{cnic}")]
        public IActionResult DeleteCustomer(string cnic)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("Connected to PostgreSQL!");
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("delete from customer where cnic = @cnic", conn);
                cmd.Parameters.AddWithValue("cnic", cnic);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return Ok($"Customer with Cnic {cnic} deleted successfully.");
                }
                else
                {
                    return NotFound($"Customer with Cnic {cnic} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error while deleting customer data.");
            }
        }

        [HttpPut("{cnic}")]
        public IActionResult UpdateCustomer(string cnic, Customer customer)
        {
            if (customer == null || customer.Cnic != cnic)
            {
                return BadRequest("Customer data is null or Cnic does not match.");
            }
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("Connected to PostgreSQL!");
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("update customer set name = @name, phone = @phone, email = @email where cnic = @cnic", conn);
                cmd.Parameters.AddWithValue("cnic", customer.Cnic);
                cmd.Parameters.AddWithValue("name", customer.Name);
                cmd.Parameters.AddWithValue("phone", customer.Phone);
                cmd.Parameters.AddWithValue("email", customer.Email);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return Ok($"Customer with Cnic {cnic} updated successfully.");
                }
                else
                {
                    return NotFound($"Customer with Cnic {cnic} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error while updating customer data.");
            }
        }
    }
}
