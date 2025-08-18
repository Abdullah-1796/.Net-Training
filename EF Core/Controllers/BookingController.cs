using EF_Core.DTOs;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Xml.Linq;
using YourNamespace.Data;

namespace EF_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private AppDbContext context;

        public BookingController(AppDbContext appDbContext)
        {
            context = appDbContext;
        }

        [HttpPost("{roomno}")]
        public async Task<IActionResult> AddBooking(int roomno, string cnic, [FromBody] DateOnly checkinDate)
        {
            var room = await context.Rooms.FirstOrDefaultAsync(r => r.RoomNo == roomno && r.Status == "Available");
            if (room == null)
            {
                return NotFound($"Room {roomno} is not available for booking.");
            }
            var customer = await context.Customers.FirstOrDefaultAsync(c => c.Cnic == cnic);
            if (customer == null)
            {
                return NotFound($"Customer with CNIC {cnic} not found.");
            }

            //var prevBooking = await context.Bookings.FirstOrDefaultAsync(b => b.CustomerId == customer.CustomerId);

            //if (prevBooking != null)
            //{
            //    return BadRequest($"Customer with CNIC {cnic} already has a booking.");
            //}

            Booking booking = new Booking
            {
                RoomId = room.RoomId,
                CustomerId = customer.CustomerId,
                CheckIn = checkinDate
            };

            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

            RoomController roomController = new RoomController(context);
            await roomController.CheckInRoom(roomno);

            return Ok(new { message = "Booking created successfully", roomNo = roomno, cnic });
        }

        [HttpPost("endbooking{roomno}")]
        public async Task<IActionResult> EndBooking(int roomno, string cnic, [FromBody] DateOnly checkoutdate)
        {
            if (string.IsNullOrEmpty(cnic))
            {
                return BadRequest("Cnic cannot be null or empty!");
            }
            await context.SaveChangesAsync();

            var room = await context.Rooms.FirstOrDefaultAsync(r => r.RoomNo == roomno);
            if (room == null)
            {
                return NotFound($"Room {roomno} not found.");
            }

            var customer = await context.Customers.FirstOrDefaultAsync(c => c.Cnic == cnic);
            if(customer == null)
            {
                return BadRequest($"Customer with CNIC {cnic} not found.");
            }

            var booking = await context.Bookings.FirstOrDefaultAsync(b => b.RoomId == room.RoomId && b.CustomerId == customer.CustomerId && b.CheckOut.Equals(new DateOnly(0001,01,01)));
            //Console.WriteLine(booking.CheckOut);
            if(booking == null)
            {
                return NotFound($"No booking found for room {roomno} against customer with CNIC {cnic}.");
            }

            booking.CheckOut = checkoutdate;
            await context.SaveChangesAsync();

            RoomController roomController = new RoomController(context);
            await roomController.CheckOutRoom(roomno);

            return Ok(new { message = "Booking ended successfully", roomNo = roomno, cnic, checkoutDate = checkoutdate });
        }

        [HttpGet("{cnic}")]
        public async Task<IActionResult> GetBookingByCnic(string cnic)
        {
            if (string.IsNullOrEmpty(cnic))
            {
                return BadRequest("Cnic cannot be null or empty!");
            }

            var customer = await context.Customers.FirstOrDefaultAsync(c => c.Cnic == cnic);
            if (customer == null)
            {
                return NotFound($"Customer with CNIC {cnic} not found.");
            }
            var booking = await context.Bookings
                .Include(b => b.room)
                .Where(b => b.CustomerId == customer.CustomerId).ToListAsync();
            if (booking == null)
            {
                return NotFound($"No active booking found for customer with CNIC {cnic}.");
            }
            //return Ok(new
            //{
            //    BookingId = booking.BookingId,
            //    RoomNo = booking.room.RoomNo,
            //    CustomerName = booking.customer.Name,
            //    Phone = booking.customer.Phone,
            //    Email = booking.customer.Email,
            //    Capacity = booking.room.Capacity,
            //    Status = booking.room.Status
            //});

            return Ok(booking.Select(b => new
            {
                BookingId = b.BookingId,
                RoomNo = b.room.RoomNo,
                CustomerName = b.customer.Name,
                Phone = b.customer.Phone,
                Email = b.customer.Email,
                Capacity = b.room.Capacity,
                Status = b.room.Status,
                CheckIn = b.CheckIn,
                CheckOut = b.CheckOut
            }));
        }
    }
}
