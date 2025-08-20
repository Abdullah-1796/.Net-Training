using EF_Core.DTOs;
using EF_Core.Models;
using EF_Core.Repositories.Interfaces;
using EF_Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EF_Core.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ICustomerRepository _customerRepository;

        public BookingService(IBookingRepository bookingRepository, IRoomRepository roomRepository, ICustomerRepository customerRepository)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _customerRepository = customerRepository;
        }
        public async Task<bool> CreateBookingAsync(int roomno, string cnic, [FromBody] DateOnly checkinDate)
        {
            //Room validation
            var room = await _roomRepository.GetRoomByRoomNo(roomno);
            if (room == null || room.Status != "Available")
            {
                throw new ArgumentException($"Room {roomno} is not available for booking.");
            }

            //Customer validation
            var customer = await _customerRepository.GetCustomerByCnicAsync(cnic);
            if (customer == null)
            {
                throw new ArgumentException($"Customer with CNIC {cnic} not found.");
            }


            var booking = new Booking
            {
                RoomId = room.RoomId,
                CustomerId = customer.CustomerId,
                CheckIn = checkinDate
            };
            await _roomRepository.UpdateRoomStatusAsync(roomno, "Occupied");
            return await _bookingRepository.CreateBookingAsync(booking);
        }

        public async Task<bool> EndBookingAsync(int roomno, string cnic, [FromBody] DateOnly checkoutdate)
        {
            //Room validation
            var room = await _roomRepository.GetRoomByRoomNo(roomno);
            if (room != null && room.Status != "Occupied")
            {
                throw new ArgumentException($"Room {roomno} is not available for booking.");
            }

            //Customer validation
            var customer = await _customerRepository.GetCustomerByCnicAsync(cnic);
            if (customer == null)
            {
                throw new ArgumentException($"Customer with CNIC {cnic} not found.");
            }

            //Booking validation
            var booking = await _bookingRepository.GetBookingAsync(room.RoomId, customer.CustomerId);
            if (booking == null)
            {
                throw new ArgumentException($"No booking found for room {roomno} and customer with CNIC {cnic}.");
            }

            booking.CheckOut = checkoutdate;
            await _roomRepository.UpdateRoomStatusAsync(roomno, "Available");
            return await _bookingRepository.EndBookingAsync(booking);
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByCnicAsync(string cnic)
        {
            if (string.IsNullOrEmpty(cnic))
            {
                throw new ArgumentException("CNIC cannot be null or empty.");
            }

            //Customer validation
            Customer? customer = await _customerRepository.GetCustomerByCnicAsync(cnic);
            if (customer == null)
            {
                throw new ArgumentException($"Customer with CNIC {cnic} not found.");
            }

            var bookings = await _bookingRepository.GetBookingsByCnicAsync(customer);
            var bookingDtos = bookings.Select(b => new BookingDTO
            {
                BookingId = b.BookingId,
                RoomNo = b.room?.RoomNo ?? 0,  // default if null
                CustomerName = b.customer?.Name ?? "Unknown",
                Phone = b.customer?.Phone ?? "N/A",
                Email = b.customer?.Email ?? "N/A",
                Capacity = b.room?.Capacity ?? 0,
                Status = b.room?.Status ?? "N/A"
            });

            return bookingDtos;
        }
    }
}
