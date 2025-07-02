using DoAn.Data;
using DoAn.DTO;

namespace DoAn.Service
{
    public interface IRoomService
    {
        Task<List<RoomResponseDTO>> GetAllRooms();
        Task<RoomResponseDTO> GetRoomById(int id);
        Task CreateRoom(RoomCreateDTO room);
        Task<bool> UpdateRoom(int id, RoomUpdateDTO room);
        Task<bool> DeleteRoom(int id);
        Task<List<RoomResponseDTO>> GetAvailbleRoom();
    }
}
