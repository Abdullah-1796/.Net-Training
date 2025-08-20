using EF_Core.DTOs;

namespace EF_Core.Services.Interfaces
{
    public interface IRoomService
    {
        Task<RoomDTO?> CreateRoomAsync(RoomDTO roomDto);
        Task<IEnumerable<RoomDTO>> GetAllRoomsAsync(int limit = 2, int offset = 0);
        Task<RoomDTO?> GetRoomByRoomNo(int roomNo);
        Task<IEnumerable<RoomDTO>> GetAvailableRoomsAsync(int capacity);
        Task<bool> UpdateRoomStatusAsync(int roomNo, string status);
        Task<bool> DeleteRoomAsync(int roomNo);
    }
}
