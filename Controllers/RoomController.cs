using DoAn.Data;
using DoAn.DTO;
using DoAn.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : Controller
    {
        private readonly IRoomService roomService;
        public RoomController(IRoomService roomService)
        {
            this.roomService = roomService;
        }
        [HttpGet("get_all_rooms")]
        public async Task<IActionResult> GetAllRoom()
        {
            var rooms = await roomService.GetAllRooms();
            return Ok(rooms);

        }
        [HttpGet("get_active_rooms")]
        public async Task<IActionResult> GetActiveRoom()
        {
            var rooms = await roomService.GetAvailbleRoom();
            return Ok(rooms);
        }
        [HttpGet("get_top_rooms")]
        public async Task<IActionResult> GetTopRoom()
        {
            var rooms = await roomService.GetTopRoom();
            return Ok(rooms);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomById(int id)
        {
            var room = await roomService.GetRoomById(id);
            if (room == null)
                return NotFound();
            return Ok(room);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] RoomCreateDTO room)
        {
            await roomService.CreateRoom(room);
            return Ok(new { message = "Tạo phòng thành công" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromForm] RoomUpdateDTO roomDTO)
        {
            var result = await roomService.UpdateRoom(id, roomDTO);

            if (!result)
                return BadRequest(new { message = "Lỗi" });
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await roomService.DeleteRoom(id);
            return Ok();
        }
    }
}
