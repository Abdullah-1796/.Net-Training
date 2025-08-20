using EF_Core.Models;
using EF_Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using YourNamespace.Data;

namespace EF_Core.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private AppDbContext _context;

        public RoomRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Room> CreateRoomAsync(Room room)
        {
            try
            {
                var newRoom = await _context.AddAsync(room);
                await _context.SaveChangesAsync();
                return newRoom.Entity;
            }
            catch (Exception ex)
            {
                throw new Exception("Error message in Room Repository: " + ex.Message);
            }
        }

        public async Task<bool> DeleteRoomAsync(int roomNo)
        {
            try
            {
                var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomNo == roomNo);
                if (room == null)
                {
                    return false;
                }
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error message in Room Repository: " + ex.Message);
            }
        }

        public async Task<Room?> GetRoomByRoomNo(int roomNo)
        {
            try
            {
                return await _context.Rooms.FirstOrDefaultAsync(r => r.RoomNo == roomNo);
            }
            catch (Exception ex)
            {
                throw new Exception("Error message in Room Repository: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync(int limit = 2, int offset = 0)
        {
            try
            {
                return await _context.Rooms.OrderBy(r => r.RoomNo).Take(limit).Skip(offset).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error message in Room Repository: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(int capacity)
        {
            try
            {
                return await _context.Rooms.Where(r => r.Status == "Available" && r.Capacity >= capacity).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error message in Room Repository: " + ex.Message);
            }
        }

        public async Task<bool> UpdateRoomStatusAsync(int roomNo, string status)
        {
            try
            {
                var existingRoom = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomNo == roomNo);
                if (existingRoom == null)
                {
                    return false;
                }
                existingRoom.Status = status;
                _context.Rooms.Update(existingRoom);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error message in Room Repository: " + ex.Message);
            }
        }

        public async Task<bool> Exist(int roomNo)
        {
            try
            {
                return await _context.Rooms.AnyAsync(r => r.RoomNo == roomNo);
            }
            catch (Exception ex)
            {
                throw new Exception("Error message in Room Repository: " + ex.Message);
            }
        }
    }
}
