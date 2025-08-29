using EF_Core.DTOs;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace EF_Core.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<bool> CreateBookingAsync(Booking booking);
        Task<Booking?> GetBookingAsync(Guid roomId, Guid customerId);
        Task<bool> CheckInAsync(Booking booking);
        public Task<IEnumerable<Booking>> GetUncheckedBookingsAsync();
        public Task<bool> CancelBookingAsync(Booking booking);
        public Task<bool> CancelPostponedBookingsAsync();
        Task<bool> EndBookingAsync(Booking booking);
        Task<IEnumerable<Booking>> GetBookingsByCnicAsync(Customer customer);
    }
}
