using EF_Core.DTOs;
using EF_Core.DTOs.For_Patch;
using EF_Core.Models;
using EF_Core.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourNamespace.Data;

namespace EF_Core.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateBookingAsync(Booking booking)
        {
            try
            {
                var bookingEntry = await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating booking: " + ex.Message);
            }
        }

        public async Task<Booking?> GetBookingAsync(Guid roomId, Guid customerId)
        {
            try
            {
                var booking = await _context.Bookings.Where(b => b.RoomId == roomId && b.CustomerId == customerId && b.CheckOut.Equals(new DateOnly(0001, 01, 01))).FirstOrDefaultAsync();

                return booking;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving booking: " + ex.Message);
            }
        }

        public async Task<bool> EndBookingAsync(Booking booking)
        {
            try
            {
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error ending booking: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Booking>> GetBookingsByCnicAsync(Customer customer)
        {
            try
            {
                var bookings = await _context.Bookings
                .Include(b => b.room)
                .Include(b => b.customer)
                .Where(b => b.CustomerId == customer.CustomerId).ToListAsync();

                

                return bookings;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving bookings: " + ex.Message);
            }
        }
    }
}
