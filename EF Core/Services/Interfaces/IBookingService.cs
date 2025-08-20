using EF_Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EF_Core.Services.Interfaces
{
    public interface IBookingService
    {
        Task<bool> CreateBookingAsync(int roomno, string cnic, [FromBody] DateOnly checkinDate);
        Task<bool> EndBookingAsync(int roomno, string cnic, [FromBody] DateOnly checkoutdate);
        Task<IEnumerable<BookingDTO>> GetBookingsByCnicAsync(string cnic);
    }
}
