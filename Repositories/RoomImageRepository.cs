using DoAn.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Repositories
{
    public class RoomImageRepository : IRoomImageRepository
    {
        public readonly DbBookingContext _context;
        public RoomImageRepository(DbBookingContext context )
        {
            _context = context;
        }

        public async Task<ActionResult<RoomImage>> AddRoomImage(RoomImage roomImage)
        {
            throw new Exception(); 
        }

        public Task<ActionResult<RoomImage>> DeleteRoomImage(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult<RoomImage>> GetRoomImageById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResult<IEnumerable<RoomImage>>> GetRoomImages()
        {
            return await _context.RoomImages.ToListAsync();
        }

        public async Task<ActionResult> UpdateRoomImage(int id, RoomImage roomImage)
        {
            //return await _context.Rooms.Update(roomImage);
            throw new NotImplementedException();

        }

        Task<ActionResult> IRoomImageRepository.DeleteRoomImage(int id)
        {
            throw new NotImplementedException();
        }
    }
}
