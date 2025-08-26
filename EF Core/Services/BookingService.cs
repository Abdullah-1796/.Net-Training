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
        public async Task<bool> CreateBookingAsync([FromBody] BookingRequest bookingRequest)
        {
            //Room validation
            var room = await _roomRepository.GetRoomByRoomNo(bookingRequest.RoomNo);
            if (room == null || room.Status != "Available")
            {
                throw new ArgumentException($"Room {bookingRequest.RoomNo} is not available for booking.");
            }

            //Customer validation
            var customer = await _customerRepository.GetCustomerByCnicAsync(bookingRequest.Cnic);
            if (customer == null)
            {
                throw new ArgumentException($"Customer with CNIC {bookingRequest.Cnic} not found.");
            }

            //Date validation
            if (bookingRequest.date < DateOnly.FromDateTime(DateTime.Now))
            {
                throw new ArgumentException("Check-in date cannot be in the past.");
            }


            var booking = new Booking
            {
                RoomId = room.RoomId,
                CustomerId = customer.CustomerId,
                CheckIn = bookingRequest.date
            };
            await _roomRepository.UpdateRoomStatusAsync(bookingRequest.RoomNo, "Occupied");
            return await _bookingRepository.CreateBookingAsync(booking);
        }

        public async Task<bool> EndBookingAsync([FromBody] BookingRequest bookingRequest)
        {
            //Room validation
            var room = await _roomRepository.GetRoomByRoomNo(bookingRequest.RoomNo);
            if (room != null && room.Status != "Occupied")
            {
                throw new ArgumentException($"Room {bookingRequest.RoomNo} is not available for booking.");
            }

            //Customer validation
            var customer = await _customerRepository.GetCustomerByCnicAsync(bookingRequest.Cnic);
            if (customer == null)
            {
                throw new ArgumentException($"Customer with CNIC {bookingRequest.Cnic} not found.");
            }

            //Booking validation
            var booking = await _bookingRepository.GetBookingAsync(room.RoomId, customer.CustomerId);
            if (booking == null)
            {
                throw new ArgumentException($"No booking found for room {bookingRequest.RoomNo} and customer with CNIC {bookingRequest.Cnic}.");
            }

            //Date validation
            if (bookingRequest.date < booking.CheckIn)
            {
                throw new ArgumentException("Check-out date cannot be before check-in date.");
            }

            booking.CheckOut = bookingRequest.date;
            await _roomRepository.UpdateRoomStatusAsync(bookingRequest.RoomNo, "Available");
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
