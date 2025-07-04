using DoAn.Data;
using DoAn.DTO;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly DbBookingContext _context;
        private readonly IWebHostEnvironment _env;
        public RoomRepository(DbBookingContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task AddRoom(Room room)
        {
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteRoom(int id)
        {
            var room = await GetRoomById(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
                return true;
            }         
            return false;
        }

        public async Task<bool> ExistsAsync(int id)
        {
           return await _context.Rooms.AnyAsync(r => r.Id == id);
        }

        public async Task<List<RoomResponseDTO>> GetAllRoom()
        {
           var rooms = await _context.Rooms.Include(r => r.Images)
                .ToListAsync(); 
            var result = rooms.Select(r => new RoomResponseDTO
            {
                Id = r.Id,
                Room_No = r.Room_No,
                Room_Name = r.Room_Name,
                Capacity = r.Capacity,
                Type = r.Type,
                Price = r.Price,
                IsAvailable = r.IsAvailable,
                Bed = r.Bed,
                Bath = r.Bath,
                Area = r.Area,
                Description = r.description,
                Images = r.Images.Select(i => i.ImageUrl).ToList()
            }).ToList();
            return result;
        }

        public Task<Room?> GetRoomById(int id)
        {
            return _context.Rooms.Include(r => r.Images).FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<RoomResponseDTO>> GetTopRoom()
        {
            var topRooms = await _context.BookingDetails
                .GroupBy(d => d.RoomId)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => new RoomResponseDTO
                {
                    Id = g.First().Room.Id,
                    Room_No = g.First().Room.Room_No, 
                    Room_Name = g.First().Room.Room_Name,
                    Capacity = g.First().Room.Capacity,
                    Type = g.First().Room.Type,
                    Price = g.First().Room.Price,
                    IsAvailable = g.First().Room.IsAvailable,
                    Bed = g.First().Room.Bed,
                    Bath = g.First().Room.Bath,
                    Area = g.First().Room.Area,
                    Description = g.First().Room.description,
                    Images = g.First().Room.Images.Select(i => i.ImageUrl).ToList()
                })
                .ToListAsync();
            return topRooms;
        }

        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateRoom(int id,Room room)
        {
            try
            {
                var existingRoom = await _context.Rooms.FindAsync(id);
                if (existingRoom == null) return false;
                _context.Rooms.Update(room);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task UpdateRoomImages(int roomId, List<IFormFile> newImages, List<string> keptImageIds)
        {
            var room = await _context.Rooms
              .Include(r => r.Images)
              .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null)
                return;

            // Xóa ảnh không nằm trong danh sách giữ lại
            var imagesToRemove = room.Images
                .Where(img => !keptImageIds.Contains(img.ImageUrl))
                .ToList();
            foreach (var img in imagesToRemove)
            {
                _context.RoomImages.Remove(img);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", img.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            // Thêm ảnh mới
            foreach (var imageFile in newImages)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var filePath = Path.Combine(_env.WebRootPath, "uploads", "rooms", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                room.Images.Add(new RoomImage
                {
                    ImageUrl = $"/uploads/rooms/{fileName}",
                    RoomId = roomId,
                    Room = room
                });
            }  
            await _context.SaveChangesAsync();
        }
    }
}

