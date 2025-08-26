using EF_Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EF_Core.Services.Interfaces
{
    public interface IBookingService
    {
        Task<bool> CreateBookingAsync([FromBody] BookingRequest bookingRequest);
        Task<bool> EndBookingAsync([FromBody] BookingRequest bookingRequest);
        Task<IEnumerable<BookingDTO>> GetBookingsByCnicAsync(string cnic);
    }
}
