using EF_Core.DTOs;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace EF_Core.Services.Interfaces
{
    public interface IBookingService
    {
        Task<bool> CreateBookingAsync([FromBody] BookingRequest bookingRequest);
        Task<bool> EndBookingAsync([FromBody] CheckoutRequest bookingRequest);
        Task<bool> CheckInAsync([FromBody] CheckinRequest checkInRequest);
        public Task<IEnumerable<Booking>> GetUncheckedBookingsAsync();
        public Task<bool> CancelBookingAsync(CancelBookingRequest cancelBookingRequest);
        public Task<bool> CancelPostponedBookingsAsync();
        Task<IEnumerable<BookingDTO>> GetBookingsByCnicAsync(string cnic);
    }
}
