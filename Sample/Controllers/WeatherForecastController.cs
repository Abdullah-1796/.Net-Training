using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Sample.Controllers
{
    [ApiController]
    [Route("weatherforecast")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            DB();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        private void DB() {
            string connection = "Host=localhost; Port=5432;  Database=Training; Username=postgres; Password=123456789";
            using var conn = new NpgsqlConnection(connection);
            conn.Open();
            Console.WriteLine("Connected to PostgreSQL!");

            NpgsqlCommand cmd = new NpgsqlCommand("Create table if not exists Name(id serial primary key, name varchar(255))", conn);
            cmd.ExecuteNonQuery();
            Console.WriteLine("Table created");

            try
            {
                cmd = new NpgsqlCommand("insert into name(name) values(@name)", conn);
                cmd.Parameters.AddWithValue("name", "Abdullah");
                cmd.ExecuteNonQuery();
                Console.WriteLine("value inserted");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                cmd = new NpgsqlCommand("select * from name", conn);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(reader.GetOrdinal("id"));
                    string name = "Not Defined";
                    if (!reader.IsDBNull(reader.GetOrdinal("name")))
                    {
                        name = reader.GetString(reader.GetOrdinal("name"));
                    }
                    Console.WriteLine($"ID: {id}, Name: {name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
