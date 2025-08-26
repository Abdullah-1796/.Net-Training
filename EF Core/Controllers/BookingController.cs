using EF_Core.DTOs;
using EF_Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddBooking([FromBody] BookingRequest bookingRequest)
        {
            var bookingCreated = await bookingService.CreateBookingAsync(bookingRequest);
            if (!bookingCreated)
            {
                return BadRequest($"Booking could not be created for room {bookingRequest.RoomNo} against customer with CNIC {bookingRequest.Cnic}.");
            }
            return Ok(new { message = "Booking created successfully", roomNo = bookingRequest.RoomNo, bookingRequest.Cnic, checkinDate = bookingRequest.date });
        }

        [Authorize]
        [HttpPost("endbooking")]
        public async Task<IActionResult> EndBooking([FromBody] BookingRequest bookingRequest)
        {
            var booking = await bookingService.EndBookingAsync(bookingRequest);
            if (!booking)
            {
                return NotFound($"No booking found for room {bookingRequest.RoomNo} against customer with CNIC {bookingRequest.Cnic}.");
            }
            return Ok(new { message = "Booking ended successfully", roomNo = bookingRequest.RoomNo, bookingRequest.Cnic, checkoutDate = bookingRequest.date });
        }

        [Authorize]
        [HttpGet]
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