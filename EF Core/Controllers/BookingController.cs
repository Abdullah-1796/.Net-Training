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
        [HttpPost("new-booking")]
        public async Task<IActionResult> AddBooking([FromBody] BookingRequest bookingRequest)
        {
            var bookingCreated = await bookingService.CreateBookingAsync(bookingRequest);
            if (!bookingCreated)
            {
                return BadRequest($"Booking could not be created for room {bookingRequest.RoomNo} against customer with CNIC {bookingRequest.Cnic}.");
            }
            return Ok(new { message = "Booking created successfully", roomNo = bookingRequest.RoomNo, bookingRequest.Cnic, checkinDate = bookingRequest.CheckinDate });
        }

        [Authorize]
        [HttpPatch("check-in")]
        public async Task<IActionResult> CheckIn(CheckinRequest checkinRequest)
        {
            var checkedIn = await bookingService.CheckInAsync(checkinRequest);
            if (!checkedIn)
            {
                return NotFound($"Check-in failed for room {checkinRequest.RoomNo} against customer with CNIC {checkinRequest.Cnic}.");
            }
            return Ok(new { message = "Checked In successfully", roomNo = checkinRequest.RoomNo, checkinRequest.Cnic });
        }

        [Authorize]
        [HttpPost("end-booking")]
        public async Task<IActionResult> EndBooking([FromBody] CheckoutRequest checkoutRequest)
        {
            var booking = await bookingService.EndBookingAsync(checkoutRequest);
            if (!booking)
            {
                return NotFound($"No booking found for room {checkoutRequest.RoomNo} against customer with CNIC {checkoutRequest.Cnic}.");
            }
            return Ok(new { message = "Booking ended successfully", roomNo = checkoutRequest.RoomNo, checkoutRequest.Cnic, checkoutDate = checkoutRequest.CheckoutDate });
        }

        [Authorize]
        [HttpPatch("cancel-booking")]
        public async Task<IActionResult> CancelBooking(CancelBookingRequest cancelBookingRequest)
        {
            var isCanceled = await bookingService.CancelBookingAsync(cancelBookingRequest);
            if (!isCanceled)
            {
                return BadRequest($"Error while canceling booking with Cnic: {cancelBookingRequest.Cnic} for  Room number: {cancelBookingRequest.RoomNo}");
            }
            return Ok($"Booking with Cnic: {cancelBookingRequest.Cnic} for  Room number: {cancelBookingRequest.RoomNo} has been canceled");
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

        [Authorize]
        [HttpGet("unchecked-bookings")]
        public async Task<IActionResult> GetUncheckedBookings()
        {
            var bookings = await bookingService.GetUncheckedBookingsAsync();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"No bookings found that are still unchecked!");
            }
            return Ok(bookings);
        }
    }
}