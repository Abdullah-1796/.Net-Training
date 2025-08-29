using EF_Core.DTOs;
using EF_Core.DTOs.For_Patch;
using EF_Core.Enumerations;
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
                var booking = await _context.Bookings.Where(b => b.RoomId == roomId && b.CustomerId == customerId && b.CheckOut.Equals(new DateOnly(0001, 01, 01)) && b.Status != Status.Cancelled).OrderBy(b => b.ExpectedCheckIn).FirstOrDefaultAsync();

                return booking;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving booking: " + ex.Message);
            }
        }

        public async Task<bool> CheckInAsync(Booking booking)
        {
            try
            {
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
                return true;
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

        public async Task<IEnumerable<Booking>> GetUncheckedBookingsAsync()
        {
            try
            {
                var bookings = await _context.Bookings.Include(b => b.room).Include(b => b.customer).Where(b => b.ExpectedCheckIn/*.AddDays(1)*/ <= DateOnly.FromDateTime(DateTime.Now) && b.Status == Status.Booked).ToListAsync();
                return bookings;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting unchecked bookings: " + ex.Message);
            }
        }

        public async Task<bool> CancelBookingAsync(Booking booking)
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

        public async Task<bool> CancelPostponedBookingsAsync()
        {
            try
            {
                await _context.Rooms.Where(r => r.Bookings.Any(b => b.ExpectedCheckIn/*.AddDays(1)*/ <= DateOnly.FromDateTime(DateTime.Now) && b.Status == Status.Booked)).ExecuteUpdateAsync(bookings => bookings.SetProperty(p => p.Status, p => "Available"));

                await _context.Bookings.Where(b => b.ExpectedCheckIn/*.AddDays(1)*/ <= DateOnly.FromDateTime(DateTime.Now) && b.Status == Status.Booked).ExecuteUpdateAsync(bookings => bookings.SetProperty(p => p.Status, p => Status.Cancelled));

                return true;
            }
            catch(Exception ex)
            {
                throw new Exception($"Error while canceling postponed bookings{ex.Message}");
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
