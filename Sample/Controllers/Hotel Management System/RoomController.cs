using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Sample.Models.Hotel_Management_System;

namespace Sample.Controllers.Hotel_Management_System
{
    [Route("hms/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly string connectionString = "Host=localhost; Port=5432;  Database=Training; Username=postgres; Password=123456789";

        [HttpPost]
        public IActionResult CreateRoom(Room room)
        {
            if (room == null)
            {
                return BadRequest("Room data is null.");
            }

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();

            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO room (capacity, status) VALUES (@capacity, @status)", conn);
                cmd.Parameters.AddWithValue("capacity", room.Capacity);
                cmd.Parameters.AddWithValue("status", room.Status ?? "Available"); // Default status is "Available"
                cmd.ExecuteNonQuery();
                Console.WriteLine("Room inserted successfully.");
                return Ok(new { message = "Record inserted" });
            }
            catch (PostgresException ex)
            {
                Console.WriteLine($"Postgres error: {ex.Message}");
                return Conflict(new { message = "Data already exists" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error while inserting room data.");
            }
            finally
            {
                conn.Close();
            }
        }

        [HttpGet]
        public IActionResult GetAllRooms(int limit = 2, int offset = 0)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();
            try
            {
                string query = $"SELECT * FROM room order by room_no limit {limit} offset {offset}";
                if (limit == null || offset == null)
                {
                    limit = 2;
                    offset = 0;
                }
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                NpgsqlDataReader data = cmd.ExecuteReader();
                List<Room> rooms = new List<Room>();
                while (data.Read())
                {
                    Room room = new Room
                    {
                        RoomNo = data.GetInt32(data.GetOrdinal("room_no")),
                        Capacity = data.GetInt32(data.GetOrdinal("capacity")),
                        Status = data.IsDBNull(3) ? "Available" : data.GetString(data.GetOrdinal("status"))
                    };
                    rooms.Add(room);
                }
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error while retrieving room data.");
            }
            finally
            {
                conn.Close();
            }
        }

        [HttpGet("available/{capacity}")]
        public IActionResult GetAvailableRooms(int capacity)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM room WHERE status = 'Available' AND capacity >= @capacity", conn);
                cmd.Parameters.AddWithValue("capacity", capacity);
                NpgsqlDataReader data = cmd.ExecuteReader();
                List<Room> availableRooms = new List<Room>();
                while (data.Read())
                {
                    Room room = new Room
                    {
                        RoomNo = data.GetInt32(data.GetOrdinal("room_no")),
                        Capacity = data.GetInt32(data.GetOrdinal("capacity")),
                        Status = data.IsDBNull(3) ? "Available" : data.GetString(data.GetOrdinal("status"))
                    };
                    availableRooms.Add(room);
                }
                return Ok(availableRooms);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error while retrieving available rooms.");
            }
            finally
            {
                conn.Close();
            }
        }

        [HttpPut("checkin/{roomNo}")]
        public IActionResult CheckInRoom(int roomNo)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("UPDATE room SET status = 'Occupied' WHERE room_no = @roomNo", conn);
                cmd.Parameters.AddWithValue("roomNo", roomNo);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Room checked in successfully." });
                }
                else
                {
                    return NotFound(new { message = "Room not found or already occupied." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error while checking in the room.");
            }
            finally
            {
                conn.Close();
            }
        }

        [HttpPut("checkout/{roomNo}")]
        public IActionResult CheckOutRoom(int roomNo)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("UPDATE room SET status = 'Available' WHERE room_no = @roomNo", conn);
                cmd.Parameters.AddWithValue("roomNo", roomNo);
                int rowsAffected = cmd.ExecuteNonQuery();
                if(rowsAffected > 0)
                {
                    return Ok(new { message = "Room checked out successfully." });
                }
                else
                {
                    return NotFound(new { message = "Room not found or already available." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error while checking out the room.");
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
