using DoAn.DTO;
using DoAn.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServiceDetailController : Controller
    {

        private readonly IServiceDetailService _serviceDetailService;

        public ServiceDetailController(IServiceDetailService serviceDetailService)
        {
            _serviceDetailService = serviceDetailService;
        }

        [HttpPost]
        public async Task<IActionResult> AddServiceToRoom([FromBody] CreateServiceDetailDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {   
                if(!Guid.TryParse(userId, out var parsedUserId))
                {
                    return BadRequest(new { message = "Invalid user id" });
                }
            var result = await _serviceDetailService.AddServiceToRoomAsync(dto, parsedUserId);
            return Ok(new {message = "Add service to successfully"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error msg", errors = ex.Message });
            }
        }
    }
}
