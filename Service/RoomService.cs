using AutoMapper;
using DoAn.Data;
using DoAn.DTO;
using DoAn.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace DoAn.Service
{
        public class RoomService : IRoomService
        {
            private readonly IRoomRepository _roomRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly DbBookingContext _context;
            public RoomService(IRoomRepository roomRepository,IWebHostEnvironment env,IMapper mapper)
                {
                    _roomRepository = roomRepository;
                    _env = env;
                    _mapper = mapper;   
                }
        public async Task CreateRoom(RoomCreateDTO dto)
        {
            Console.WriteLine("Số ảnh nhận được: " + dto.NewImages.Count);
            var room = new Room
            {
                Id = dto.Id,
                Room_No = dto.Room_No,
                Room_Name = dto.Room_Name,
                Capacity = dto.Capacity,
                Type = dto.Type,
                Price = dto.Price,
                Bed = dto.Bed,
                Bath = dto.Bath,
                Area = dto.Area,
                description = dto.Description,
                CreatedAt = DateTime.Now,
                IsAvailable = dto.IsAvailable,
                Images = new List<RoomImage>()
            };
            //save images
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads/rooms");
            Directory.CreateDirectory(uploadPath);
            foreach(var formFile in dto.NewImages)
            {
                var fileName = $"{Guid.NewGuid()}_{ formFile.FileName}";
                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                }
                room.Images.Add(new RoomImage
                {
                    ImageUrl = $"/uploads/rooms/{fileName}",
                    Room = room
                });
            }
            await _roomRepository.AddRoom(room); 
        }

        public async Task<bool> DeleteRoom(int id)
            {
            if (await _roomRepository.DeleteRoom(id))
            {
                await _roomRepository.SaveChangeAsync();
                return true;
            }
            return false;
        }
        public async Task<List<RoomResponseDTO>> GetAllRooms()
        {
            var rooms = await _roomRepository.GetAllRoom();
            return rooms;
        }

        public async Task<List<RoomResponseDTO>> GetAvailbleRoom()
        {
            var rooms = await _roomRepository.GetAllRoom();
            var availableRooms = rooms.Where(r => r.IsAvailable == true).ToList();
            return availableRooms;
        }

        public async Task<RoomResponseDTO> GetRoomById(int id)
            {
                var room = await _roomRepository.GetRoomById(id);  
                var result = new RoomResponseDTO
                {
                    Id = room.Id,
                    Room_No = room.Room_No,
                    Room_Name = room.Room_Name,
                    Capacity = room.Capacity,
                    Type = room.Type,
                    Price = room.Price,
                    IsAvailable = room.IsAvailable,
                    Bed = room.Bed,
                    Bath = room.Bath,
                    Area = room.Area,
                    Description = room.description,
                    Images = room.Images.Select(i => i.ImageUrl).ToList()
                };
                return result;
        }

        public async Task<List<RoomResponseDTO>> GetTopRoom()
        {
           var topRooms = await _roomRepository.GetTopRoom();
            return topRooms;
        }

        public async Task<bool> UpdateRoom(int id, RoomUpdateDTO dto)
            {
                var room = await _roomRepository.GetRoomById(id);
                    if (room == null) 
                        return false;
                    _mapper.Map(dto,room);
                    await _roomRepository.UpdateRoom(id, room);
                    var keptImageUrls = new List<string>();
                    if (!string.IsNullOrEmpty(dto.KeptImageUrls))
                    {
                        try
                        {
                            keptImageUrls = JsonSerializer.Deserialize<List<string>>(dto.KeptImageUrls);
                    
                }
                        catch (Exception ex)
                        {
                            Console.Write(ex.Message);
                            return false;
                        }
                    }
                    await _roomRepository.UpdateRoomImages(id, dto.NewImages, keptImageUrls ?? new List<string>());
                    return true;
            }
  
    }
}
