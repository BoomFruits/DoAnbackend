using DoAn.Data;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.Repositories
{
    public interface IRoomImageRepository
    {
        Task<ActionResult<IEnumerable<RoomImage>>> GetRoomImages();
        Task<ActionResult<RoomImage>> GetRoomImageById(int id);
        Task<ActionResult<RoomImage>> AddRoomImage(RoomImage roomImage);
        Task<ActionResult> UpdateRoomImage(int id, RoomImage roomImage);
        Task<ActionResult> DeleteRoomImage(int id);
    }
}
