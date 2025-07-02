using DoAn.Data;
using DoAn.DTO;

namespace DoAn.Repositories
{
    public interface IRoomRepository
    {
        Task<List<RoomResponseDTO>> GetAllRoom();
        Task<Room?> GetRoomById(int id);
        Task AddRoom(Room room);
        Task<bool> UpdateRoom(int id,Room room);
        Task<bool> DeleteRoom(int id);
        Task SaveChangeAsync();
        Task<bool> ExistsAsync(int id);
        Task UpdateRoomImages(int roomId, List<IFormFile> newImages, List<string> keptImageIds);
    }
}
