using EF_Core.Models;
namespace EF_Core.Repositories.Interfaces
{
    public interface IRoomRepository
    {
        Task<Room> CreateRoomAsync(Room room);
        Task<IEnumerable<Room>> GetAllRoomsAsync(int limit = 2, int offset = 0);
        Task<Room?> GetRoomByRoomNo(int roomNo);
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(int capacity);
        Task<bool> UpdateRoomStatusAsync(int roomNo, string status);
        Task<bool> DeleteRoomAsync(int roomNo);
        Task<bool> Exist(int roomNo);
    }
}
