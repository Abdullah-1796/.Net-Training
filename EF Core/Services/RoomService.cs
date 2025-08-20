using AutoMapper;
using EF_Core.DTOs;
using EF_Core.Models;
using EF_Core.Repositories.Interfaces;
using EF_Core.Services.Interfaces;

namespace EF_Core.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IMapper _mapper;

        public RoomService(IRoomRepository roomRepository, IMapper mapper)
        {
            _roomRepository = roomRepository;
            _mapper = mapper;
        }

        public async Task<RoomDTO?> CreateRoomAsync(RoomDTO roomDto)
        {
            if(await _roomRepository.Exist(roomDto.RoomNo))
            {
                return null;
            }
            Room room = _mapper.Map<Room>(roomDto);
            var createdRoom = await _roomRepository.CreateRoomAsync(room);
            RoomDTO roomDTO = _mapper.Map<RoomDTO>(createdRoom);
            return roomDTO;
        }

        public Task<bool> DeleteRoomAsync(int roomNo)
        {
            return _roomRepository.DeleteRoomAsync(roomNo);
        }

        public async Task<IEnumerable<RoomDTO>> GetAllRoomsAsync(int limit = 2, int offset = 0)
        {
            var rooms = await _roomRepository.GetAllRoomsAsync(limit, offset);
            var roomDtos = _mapper.Map<IEnumerable<RoomDTO>>(rooms);
            return roomDtos;
        }

        public async Task<IEnumerable<RoomDTO>> GetAvailableRoomsAsync(int capacity)
        {
            var availableRooms = await _roomRepository.GetAvailableRoomsAsync(capacity);
            var roomDtos = _mapper.Map<IEnumerable<RoomDTO>>(availableRooms);
            return roomDtos;
        }

        public async Task<RoomDTO?> GetRoomByRoomNo(int roomNo)
        {
            var room = await _roomRepository.GetRoomByRoomNo(roomNo);
            if (room == null)
            {
                return null;
            }
            RoomDTO roomDto = _mapper.Map<RoomDTO>(room);
            return roomDto;
        }

        public Task<bool> UpdateRoomStatusAsync(int roomNo, string status)
        {
            return _roomRepository.UpdateRoomStatusAsync(roomNo, status);
        }
    }
}
