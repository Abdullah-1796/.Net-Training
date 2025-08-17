using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Npgsql;
using Sample.Models.Hotel_Management_System;

namespace Sample.Controllers.Hotel_Management_System
{
    [Route("hms/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly string connectionString = "Host=localhost; Port=5432;  Database=Training; Username=postgres; Password=123456789";
        [HttpPost]
        public ActionResult<Employee> CreateEmployee(Employee employee)
        {
            if (employee == null)
            {
                return BadRequest("Employee data is null.");
            }

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("Connected to PostgreSQL!");

            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("insert into employee(cnic, name, phone, email, role) values(@cnic, @name, @phone, @email, @role)", conn);
                cmd.Parameters.AddWithValue("cnic", employee.Cnic);
                cmd.Parameters.AddWithValue("name", employee.Name);
                cmd.Parameters.AddWithValue("phone", employee.phone);
                cmd.Parameters.AddWithValue("email", employee.email);
                cmd.Parameters.AddWithValue("role", employee.role);
                cmd.ExecuteNonQuery();
                Console.WriteLine("value inserted");

                return CreatedAtAction(nameof(CreateEmployee), new { id = employee.Eid }, employee);
            }
            catch (PostgresException ex)
            {
                Console.WriteLine($"Postgres error: {ex.Message}");
                return Conflict(new { message = "Data already exist" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error while inserting employee data.");
            }           
        }

        [HttpGet]
        public ActionResult<IEnumerable<Employee>> GetAllEmployees(int limit = 2, int offset = 0)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("Connected to PostgreSQL!");

            try
            {
                string query = $"SELECT * FROM employee order by eid limit {limit} offset {offset}";
                if (limit == null || offset == null)
                {
                    limit = 2;
                    offset = 0;
                }
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                NpgsqlDataReader data = cmd.ExecuteReader();

                List<Employee> employees = new List<Employee>();

                while (data.Read())
                {
                    Employee employee = new Employee()
                    {
                        Name = data.GetString(data.GetOrdinal("name")),
                        Cnic = data.GetString(data.GetOrdinal("cnic")),
                        phone = data.GetString(data.GetOrdinal("phone")),
                        email = data.GetString(data.GetOrdinal("email")),
                        role = data.GetString(data.GetOrdinal("role"))
                    };
                    employees.Add(employee);
                }

                Console.WriteLine("Data get successfully!");
                return Ok(employees);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error while inserting employee data.");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Employee> GetEmployeeById(String id)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("Connected to PostgreSQL!");

            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand($"select * from Employee where eid = {id}", conn);
                NpgsqlDataReader data = cmd.ExecuteReader();

                if (data.Read())
                {
                    Employee employee = new Employee()
                    {
                        Name = data.GetString(data.GetOrdinal("name")),
                        Cnic = data.GetString(data.GetOrdinal("cnic")),
                        phone = data.GetString(data.GetOrdinal("phone")),
                        email = data.GetString(data.GetOrdinal("email")),
                        role = data.GetString(data.GetOrdinal("role"))
                    };
                    Console.WriteLine("Data get successfully!");
                    return Ok(employee);
                }
                else
                {
                    return NotFound($"Employee with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error while inserting employee data.");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteEmployeeById(String id)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("Connected to PostgreSQL!");
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand($"delete from Employee where eid = {id}", conn);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Data deleted successfully!");
                    return Ok($"Employee with ID {id} deleted successfully.");
                }
                else
                {
                    return NotFound($"Employee with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error while deleting employee data.");
            }
        }

        [HttpPut("{id}")]
        public ActionResult<Employee> UpdateEmployeeById(String id, Employee employee)
        {
            if (employee == null)
            {
                return BadRequest("Employee data is null.");
            }
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("Connected to PostgreSQL!");
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand($"update Employee set cnic = @cnic, name = @name, phone = @phone, email = @email, role = @role where eid = {id}", conn);
                cmd.Parameters.AddWithValue("cnic", employee.Cnic);
                cmd.Parameters.AddWithValue("name", employee.Name);
                cmd.Parameters.AddWithValue("phone", employee.phone);
                cmd.Parameters.AddWithValue("email", employee.email);
                cmd.Parameters.AddWithValue("role", employee.role);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Data updated successfully!");
                    return Ok(employee);
                }
                else
                {
                    return NotFound($"Employee with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error while updating employee data.");
            }
        }
    }
}
