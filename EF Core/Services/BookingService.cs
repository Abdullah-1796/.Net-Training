using EF_Core.DTOs;
using EF_Core.Enumerations;
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
            if (bookingRequest.CheckinDate < DateOnly.FromDateTime(DateTime.Now))
            {
                throw new ArgumentException("Check-in date cannot be in the past.");
            }


            var booking = new Booking
            {
                RoomId = room.RoomId,
                CustomerId = customer.CustomerId,
                ExpectedCheckIn = bookingRequest.CheckinDate,
                Duration = bookingRequest.Duration
            };
            await _roomRepository.UpdateRoomStatusAsync(bookingRequest.RoomNo, "Occupied");
            return await _bookingRepository.CreateBookingAsync(booking);
        }

        public async Task<bool> CheckInAsync([FromBody] CheckinRequest checkInRequest)
        {
            //Room validation
            var room = await _roomRepository.GetRoomByRoomNo(checkInRequest.RoomNo);
            if (room == null || room.Status != "Occupied")
            {
                throw new ArgumentException($"Room {checkInRequest.RoomNo} is not available for check-in.");
            }
            //Customer validation
            var customer = await _customerRepository.GetCustomerByCnicAsync(checkInRequest.Cnic);
            if (customer == null)
            {
                throw new ArgumentException($"Customer with CNIC {checkInRequest.Cnic} not found.");
            }
            //Booking validation
            var booking = await _bookingRepository.GetBookingAsync(room.RoomId, customer.CustomerId);
            if (booking == null)
            {
                throw new ArgumentException($"No booking found for room {checkInRequest.RoomNo} and customer with CNIC {checkInRequest.Cnic}.");
            }
            if(booking.Status == Status.CheckedIn)
            {
                throw new ArgumentException($"Customer with CNIC {checkInRequest.Cnic} has already checked in for room {checkInRequest.RoomNo}.");
            }
            if(booking.ExpectedCheckIn > DateOnly.FromDateTime(DateTime.Now))
            {
                throw new ArgumentException("Check-in date cannot be in the past.");
            }
            booking.Status = Status.CheckedIn;
            return await _bookingRepository.CheckInAsync(booking);
        }

        public async Task<IEnumerable<Booking>> GetUncheckedBookingsAsync()
        {
            return await _bookingRepository.GetUncheckedBookingsAsync();
        }

        public async Task<bool> CancelBookingAsync(CancelBookingRequest cancelBookingRequest)
        {
            //Room validation
            var room = await _roomRepository.GetRoomByRoomNo(cancelBookingRequest.RoomNo);
            if (room == null || room.Status != "Occupied")
            {
                throw new ArgumentException($"Room {cancelBookingRequest.RoomNo} has not been booked");
            }

            //Customer validation
            var customer = await _customerRepository.GetCustomerByCnicAsync(cancelBookingRequest.Cnic);
            if (customer == null)
            {
                throw new ArgumentException($"Customer with CNIC {cancelBookingRequest.Cnic} not found.");
            }

            //Booking validation
            var booking = await _bookingRepository.GetBookingAsync(room.RoomId, customer.CustomerId);
            if (booking == null)
            {
                throw new ArgumentException($"No booking found for room {cancelBookingRequest.RoomNo} and customer with CNIC {cancelBookingRequest.Cnic}.");
            }

            //Status check
            if (booking.Status != Status.Booked)
            {
                throw new ArgumentException($"Customer with CNIC {cancelBookingRequest.Cnic} has already checked in for room {cancelBookingRequest.RoomNo}.");
            }

            booking.Status = Status.Cancelled;
            await _roomRepository.UpdateRoomStatusAsync(room.RoomNo, "Available");
            return await _bookingRepository.CancelBookingAsync(booking);
        }

        public async Task<bool> CancelPostponedBookingsAsync()
        {
            return await _bookingRepository.CancelPostponedBookingsAsync();
        }

        public async Task<bool> EndBookingAsync([FromBody] CheckoutRequest checkoutRequest)
        {
            //Room validation
            var room = await _roomRepository.GetRoomByRoomNo(checkoutRequest.RoomNo);
            if (room != null && room.Status != "Occupied")
            {
                throw new ArgumentException($"Room {checkoutRequest.RoomNo} is not available for booking.");
            }

            //Customer validation
            var customer = await _customerRepository.GetCustomerByCnicAsync(checkoutRequest.Cnic);
            if (customer == null)
            {
                throw new ArgumentException($"Customer with CNIC {checkoutRequest.Cnic} not found.");
            }

            //Booking validation
            var booking = await _bookingRepository.GetBookingAsync(room!.RoomId, customer.CustomerId);
            if (booking == null)
            {
                throw new ArgumentException($"No booking found for room {checkoutRequest.RoomNo} and customer with CNIC {checkoutRequest.Cnic}.");
            }

            //Date validation
            if (checkoutRequest.CheckoutDate < booking.ExpectedCheckIn)
            {
                throw new ArgumentException("Check-out date cannot be before check-in date.");
            }

            booking.CheckOut = checkoutRequest.CheckoutDate;
            booking.Status = Enumerations.Status.CheckedOut;
            await _roomRepository.UpdateRoomStatusAsync(checkoutRequest.RoomNo, "Available");
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
                    roomStatus = (b.Status == Status.Cancelled || b.Status == Status.CheckedOut)? "N/A" : "Occupied",
                    bookingStatus = b.Status,
                    ExpectedCheckIn = b.ExpectedCheckIn,
                    CheckOut = b.CheckOut,
                    Duration = b.Duration
            });
            
            return bookingDtos;
        }
    }
}
