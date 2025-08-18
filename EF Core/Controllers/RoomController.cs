using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EF_Core.Models;
using EF_Core.DTOs;
using System.Runtime.CompilerServices;
using YourNamespace.Data;
using Microsoft.EntityFrameworkCore;

namespace EF_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private AppDbContext context;

        public RoomController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] Room room)
        {
            if (room == null)
            {
                return BadRequest("Room data is required.");
            }
            context.Rooms.Add(room);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateRoom), new { id = room.RoomId }, room);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Room>> GetAllRooms(int limit = 2, int offset = 0)
        {
            var rooms = context.Rooms.OrderBy(r => r.RoomNo).ToList().Take(limit).Skip(offset);
            return Ok(rooms);
        }

        [HttpGet("availablerooms{capacity}")]
        public ActionResult<IEnumerable<Room>> GetAvailableRooms(int capacity)
        {
            var availableRooms = context.Rooms
                .Where(r => r.Status == "Available" && r.Capacity >= capacity)
                .ToList();
            if (availableRooms.Count == 0)
            {
                return NotFound("No available rooms found with the specified capacity.");
            }
            return Ok(availableRooms);
        }

        [HttpPost("checkin{roomno}")]
        public async Task<IActionResult> CheckInRoom(int roomno)
        {
            var room = await context.Rooms.FirstOrDefaultAsync(r => r.RoomNo == roomno && r.Status == "Available");
            if (room == null)
            {
                return NotFound($"Room {roomno} is not available for check-in.");
            }
            room.Status = "Occupied";
            await context.SaveChangesAsync();
            return Ok(new { message = "Room Reserved Successfully" });
        }

        [HttpPost("checkout{roomno}")]
        public async Task<IActionResult> CheckOutRoom(int roomno)
        {
            var room = await context.Rooms.FirstOrDefaultAsync(r => r.RoomNo == roomno && r.Status == "Occupied");
            if (room == null)
            {
                return NotFound($"Room {roomno} is not occupied or does not exist.");
            }
            room.Status = "Available";
            await context.SaveChangesAsync();
            return Ok(new { message = "Room Checked Out Successfully" });
        }
    }
}
