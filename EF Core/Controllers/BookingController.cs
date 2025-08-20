using EF_Core.DTOs;
using EF_Core.Models;
using EF_Core.Services.Interfaces;
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
        private readonly IBookingService bookingService;

        public BookingController(IBookingService bookingService)
        {
            this.bookingService = bookingService;
        }

        [HttpPost("{roomno}")]
        public async Task<IActionResult> AddBooking(int roomno, string cnic, [FromBody] DateOnly checkinDate)
        {
            var bookingCreated = await bookingService.CreateBookingAsync(roomno, cnic, checkinDate);
            if (!bookingCreated)
            {
                return BadRequest($"Booking could not be created for room {roomno} against customer with CNIC {cnic}.");
            }
            return Ok(new { message = "Booking created successfully", roomNo = roomno, cnic });
        }

        [HttpPost("endbooking{roomno}")]
        public async Task<IActionResult> EndBooking(int roomno, string cnic, [FromBody] DateOnly checkoutdate)
        {
            var booking = await bookingService.EndBookingAsync(roomno, cnic, checkoutdate);
            if (!booking)
            {
                return NotFound($"No booking found for room {roomno} against customer with CNIC {cnic}.");
            }
            return Ok(new { message = "Booking ended successfully", roomNo = roomno, cnic, checkoutDate = checkoutdate });
        }

        [HttpGet("{cnic}")]
        public async Task<IActionResult> GetBookingByCnic(string cnic)
        {
            var details = await bookingService.GetBookingsByCnicAsync(cnic);
            if (details == null || !details.Any())
            {
                return NotFound($"No bookings found for customer with CNIC {cnic}.");
            }
            return Ok(details);
        }
    }
}
