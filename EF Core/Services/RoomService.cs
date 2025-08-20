using EF_Core.DTOs;
using EF_Core.Models;
using EF_Core.Repositories.Interfaces;
using EF_Core.Services.Interfaces;

namespace EF_Core.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<RoomDTO?> CreateRoomAsync(RoomDTO roomDto)
        {
            if(await _roomRepository.Exist(roomDto.RoomNo))
            {
                return null;
            }
            Room room = new Room
            {
                RoomNo = roomDto.RoomNo,
                Capacity = roomDto.Capacity,
                Status = roomDto.Status
            };
            var createdRoom = await _roomRepository.CreateRoomAsync(room);
            return new RoomDTO
            {
                RoomNo = createdRoom.RoomNo,
                Capacity = createdRoom.Capacity,
                Status = createdRoom.Status
            };
        }

        public Task<bool> DeleteRoomAsync(int roomNo)
        {
            return _roomRepository.DeleteRoomAsync(roomNo);
        }

        public async Task<IEnumerable<RoomDTO>> GetAllRoomsAsync(int limit = 2, int offset = 0)
        {
            var rooms = await _roomRepository.GetAllRoomsAsync(limit, offset);
            return rooms.Select(r => new RoomDTO
            {
                RoomNo = r.RoomNo,
                Capacity = r.Capacity,
                Status = r.Status
            });
        }

        public async Task<IEnumerable<RoomDTO>> GetAvailableRoomsAsync(int capacity)
        {
            var availableRooms = await _roomRepository.GetAvailableRoomsAsync(capacity);
            return availableRooms.Select(r => new RoomDTO
            {
                RoomNo = r.RoomNo,
                Capacity = r.Capacity,
                Status = r.Status
            });
        }

        public async Task<RoomDTO?> GetRoomByRoomNo(int roomNo)
        {
            var room = await _roomRepository.GetRoomByRoomNo(roomNo);
            if (room == null)
            {
                return null;
            }
            return new RoomDTO
            {
                RoomNo = room.RoomNo,
                Capacity = room.Capacity,
                Status = room.Status
            };
        }

        public Task<bool> UpdateRoomStatusAsync(int roomNo, string status)
        {
            return _roomRepository.UpdateRoomStatusAsync(roomNo, status);
        }
    }
}
