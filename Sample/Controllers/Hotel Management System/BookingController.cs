using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Sample.Models.Hotel_Management_System;

namespace Sample.Controllers.Hotel_Management_System
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly string connectionString = "Host=localhost; Port=5432;  Database=Training; Username=postgres; Password=123456789";

        [HttpPost("{roomno}")]
        public IActionResult AddBooking(int roomno, string cnic, [FromBody] DateOnly checkinDate)
        {
            if (string.IsNullOrEmpty(cnic))
            {
                return BadRequest("CNIC cannot be null or empty.");
            }
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("Connected to PostgreSQL!");

            try {
                //checking room availability
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM room WHERE room_no = @roomno", conn);
                cmd.Parameters.AddWithValue("roomno", roomno);
                NpgsqlDataReader data = cmd.ExecuteReader();
                if (!data.Read())
                {
                    return NotFound($"Room number {roomno} does not exist.");
                }
                int rid = data.GetInt32(data.GetOrdinal("rid"));
                string status = data.GetString(data.GetOrdinal("status"));
                if (status != "Available")
                {
                    return Conflict($"Room number {roomno} is not available for booking.");
                }
                data.Close();
                conn.Close();

                //checking customer registration
                conn.Open();
                NpgsqlCommand cmd1 = new NpgsqlCommand("SELECT * FROM customer WHERE cnic = @cnic", conn);
                cmd1.Parameters.AddWithValue("cnic", cnic);
                data = cmd1.ExecuteReader();
                if (!data.Read())
                {
                    return NotFound($"Cnic {cnic} does not exist.");
                }
                int cid = data.GetInt32(data.GetOrdinal("cid"));
                conn.Close();

                //adding new booking
                conn.Open();
                NpgsqlCommand cmd2 = new NpgsqlCommand("INSERT INTO booking (rid, cid, checkin) VALUES (@roomno, @cnic, @checkin)", conn);
                cmd2.Parameters.AddWithValue("roomno", rid);
                cmd2.Parameters.AddWithValue("cnic", cid);
                cmd2.Parameters.AddWithValue("checkin", checkinDate);
                cmd2.ExecuteNonQuery();
                conn.Close();

                RoomController roomController = new RoomController();
                roomController.CheckInRoom(roomno);

                Console.WriteLine($"Booking created for room number {roomno} with CNIC {cnic}.");
                return Ok(new { message = "Booking created successfully", roomNo = roomno, cnic });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to database: {ex.Message}");
                return StatusCode(500, "Internal server error while connecting to the database.");
            }

        }

        [HttpPost("{roomno}/endbooking")]
        public IActionResult EndBooking(int roomno, string cnic, [FromBody] DateOnly checkoutDate)
        {
            if (checkoutDate == default)
            {
                return BadRequest("Checkout date cannot be null or empty.");
            }
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("Connected to PostgreSQL!");
            try
            {
                //checking room availability
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM room WHERE room_no = @roomno", conn);
                cmd.Parameters.AddWithValue("roomno", roomno);
                NpgsqlDataReader data = cmd.ExecuteReader();
                if (!data.Read())
                {
                    return NotFound($"Room number {roomno} does not exist.");
                }
                int rid = data.GetInt32(data.GetOrdinal("rid"));
                string status = data.GetString(data.GetOrdinal("status"));
                if (status != "Occupied")
                {
                    return Conflict($"Room number {roomno} is not booked.");
                }
                data.Close();
                conn.Close();

                //checking customer registration
                conn.Open();
                NpgsqlCommand cmd1 = new NpgsqlCommand("SELECT * FROM customer WHERE cnic = @cnic", conn);
                cmd1.Parameters.AddWithValue("cnic", cnic);
                data = cmd1.ExecuteReader();
                if (!data.Read())
                {
                    return NotFound($"Cnic {cnic} does not exist.");
                }
                int cid = data.GetInt32(data.GetOrdinal("cid"));
                conn.Close();

                //ending booking
                conn.Open();
                NpgsqlCommand cmd2 = new NpgsqlCommand("UPDATE booking SET checkout = @checkout WHERE rid = @rid", conn);
                cmd2.Parameters.AddWithValue("checkout", checkoutDate);
                cmd2.Parameters.AddWithValue("rid", rid);
                cmd2.ExecuteNonQuery();
                conn.Close();

                RoomController roomController = new RoomController();
                roomController.CheckOutRoom(roomno);

                Console.WriteLine($"Booking ended for room number {roomno}.");
                return Ok(new { message = "Booking ended successfully", roomNo = roomno });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to database: {ex.Message}");
                return StatusCode(500, "Internal server error while connecting to the database.");
            }
        }

        [HttpGet("{cnic}")]
        public IActionResult GetBookingDetails(string cnic)
        {
            if (string.IsNullOrEmpty(cnic))
            {
                return BadRequest("CNIC cannot be null or empty.");
            }
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("Connected to PostgreSQL!");
            try
            {
                //checking customer registration
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM customer WHERE cnic = @cnic", conn);
                cmd.Parameters.AddWithValue("cnic", cnic);
                NpgsqlDataReader data = cmd.ExecuteReader();
                if (!data.Read())
                {
                    return NotFound($"Cnic {cnic} does not exist.");
                }
                int cid = data.GetInt32(data.GetOrdinal("cid"));
                data.Close();
                //getting booking details
                NpgsqlCommand cmd1 = new NpgsqlCommand("select * from (SELECT * FROM booking b inner join (select * from customer where cnic = @cnic) c on b.cid = c.cid) d inner join room r on d.rid = r.rid where status = 'Occupied';", conn);
                cmd1.Parameters.AddWithValue("cnic", cnic);
                data = cmd1.ExecuteReader();
                if (!data.Read())
                {
                    return NotFound($"No available bookings found for cnic {cnic}.");
                }
                int bid = data.GetInt32(data.GetOrdinal("bid"));
                int roomNo = data.GetInt32(data.GetOrdinal("room_no"));
                string name = data.GetString(data.GetOrdinal("name"));
                string phone = data.GetString(data.GetOrdinal("phone"));
                string email = data.GetString(data.GetOrdinal("email"));
                int capacity = data.GetInt32(data.GetOrdinal("capacity"));
                string status = data.GetString(data.GetOrdinal("status"));
                return Ok(new {
                    BookingId = bid,
                    RoomNo = roomNo,
                    CustomerName = name,
                    Phone = phone,
                    Email = email,
                    Capacity = capacity,
                    Status = status
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to database: {ex.Message}");
                return StatusCode(500, "Internal server error while connecting to the database.");
            }
            finally
            {
                conn.Close();
            }
        }
    }
}