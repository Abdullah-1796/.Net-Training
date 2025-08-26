using Microsoft.AspNetCore.Mvc;
using EF_Core.Models;
using EF_Core.DTOs;
using EF_Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace EF_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] RoomDTO room)
        {
            if (room == null)
            {
                return BadRequest("Room data is required.");
            }
            var r = await _roomService.CreateRoomAsync(room);
            return CreatedAtAction("GetRoomByRoomNo", new { roomno = room.RoomNo }, room);
        }

        [Authorize]
        [HttpGet("{roomno}", Name = "GetRoomByRoomNo")]
        public async Task<ActionResult<RoomDTO>> GetRoomByRoomNo(int roomno)
        {
            var room = await _roomService.GetRoomByRoomNo(roomno);
            if (room == null)
            {
                return NotFound($"Room with number {roomno} not found.");
            }
            return Ok(room);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetAllRooms(int limit = 2, int offset = 0)
        {
            var rooms = await _roomService.GetAllRoomsAsync(limit, offset);
            return Ok(rooms);
        }

        [Authorize]
        [HttpGet("availablerooms{capacity}")]
        public async Task<ActionResult<IEnumerable<Room>>> GetAvailableRooms(int capacity)
        {
            var availableRooms = await _roomService.GetAvailableRoomsAsync(capacity);
            if (!availableRooms.Any())
            {
                return NotFound("No available rooms found with the specified capacity.");
            }
            return Ok(availableRooms);
        }

        [Authorize]
        [HttpPost("checkin{roomno}")]
        public async Task<IActionResult> CheckInRoom(int roomno)
        {
            var isOccupied = await _roomService.UpdateRoomStatusAsync(roomno, "Occupied");
            if (!isOccupied)
            {
                return NotFound($"Room {roomno} is not available for check-in.");
            }
            return Ok(new { message = "Room Reserved Successfully" });
        }

        [Authorize]
        [HttpPost("checkout{roomno}")]
        public async Task<IActionResult> CheckOutRoom(int roomno)
        {
            var isAvailable = await _roomService.UpdateRoomStatusAsync(roomno, "Available");
            if (!isAvailable)
            {
                return NotFound($"Room {roomno} is not occupied or does not exist.");
            }
            return Ok(new { message = "Room Checked Out Successfully" });
        }
    }
}
