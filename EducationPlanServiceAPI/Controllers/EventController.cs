using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using EducationPlanServiceAPI.Models;
using AuthServiceAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace EducationPlanServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : Controller
    {
        private readonly AppDBContext _context;
        private readonly ILogger<EventController> _logger;
        private readonly string _secretKey;
        public EventController(IConfiguration configuration, AppDBContext db_context, ILogger<EventController> logger)
        {
            _secretKey = configuration["JWT_SECRET"];
            _context = db_context;
            _logger = logger;
        }
        
        [HttpPost("event")]
        [Authorize]
        public async Task<ActionResult> CreateEvent([FromBody] EventCreationModel eventCreationModel)
        {
            if (eventCreationModel != null)
            {
                _context.Events.Add(new Event { CreatedDate = DateTime.UtcNow, Title = eventCreationModel.Title, Description = eventCreationModel.Description, DeadLine = eventCreationModel.DeadLine, UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) });
                await _context.SaveChangesAsync();
                return Ok("Event was created succesfully.");
            }
            return BadRequest("Event parameters are invalid.");
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult> GetEvent(int id)
        {
            
            if(!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            Event ev = await _context.Events.FirstOrDefaultAsync(u => u.Id == id && u.UserID == userId);
            if (ev == null)
            {
                return NotFound("Event not found or you don't have permission to access this event.");
            }
            return Ok(ev);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult> UpdateEvent(int id, [FromBody] EventUpdateModel eventUpdateModel)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            Event ev = await _context.Events.FirstOrDefaultAsync(u => u.Id == id && u.UserID == userId);
            if (ev == null)
            {
                return NotFound("Event not found or you don't have permission to access this event.");
            }

            ev.Title = eventUpdateModel.Title ?? ev.Title;
            ev.Description = eventUpdateModel.Description ?? ev.Description;
            if (eventUpdateModel.DeadLine != null)
                ev.DeadLine = eventUpdateModel.DeadLine;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Event updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event");
                return StatusCode(500, "An error occurred while updating the event.");
            }
        }

        }
}
