using EducationPlanServiceAPI.Data;
using EducationPlanServiceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EducationPlanServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : Controller
    {
        private readonly AppDBContext _context;
        private readonly ILogger<ScheduleController> _logger;
        public EventController(IConfiguration configuration, AppDBContext db_context, ILogger<ScheduleController> logger)
        {
            _context = db_context;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateEvent([FromBody] EventCreationDto eventCreationDto)
        {
            if (eventCreationDto != null)
            {
                Event ev = new Event { CreatedDate = DateTime.UtcNow, Title = eventCreationDto.Title, Description = eventCreationDto.Description, StartTime = DateTime.SpecifyKind(eventCreationDto.StartTime, DateTimeKind.Utc), EndTime = DateTime.SpecifyKind(eventCreationDto.EndTime, DateTimeKind.Utc), EventType = eventCreationDto.EventType, ScheduleId = eventCreationDto.ScheduleId, UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value) };

                _context.Events.Add(ev);
                await _context.SaveChangesAsync();
                return Ok(new { id = ev.Id, message = "Event was created succesfully." });
            }
            return BadRequest("Event parameters are invalid.");
        }


        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult> GetEvent(int id)
        {

            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            Event ev = await _context.Events.FirstOrDefaultAsync(u => u.Id == id && u.UserID == userId);
            if (ev == null)
            {
                return NotFound("Event not found or you don't have permission to access this session.");
            }
            return Ok(ev);
        }

        [HttpGet()]
        [Authorize]
        public async Task<ActionResult> GetAllSessions()
        {

            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            List<Event> events = await _context.Events.Where(e => e.UserID == userId).ToListAsync();
            if (events == null)
            {
                return NotFound("Event not found or you don't have permission to access this event.");
            }
            return Ok(events);
        }



        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult> UpdateSession(int id, [FromBody] EventUpdateDto sessionUpdateModel)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            Event ev = await _context.Events.FirstOrDefaultAsync(u => u.Id == id && u.UserID == userId);
            if (ev == null)
            {
                return NotFound("Event not found or you don't have permission to access this session.");
            }

            ev.Title = sessionUpdateModel.Title ?? ev.Title;
            ev.Description = sessionUpdateModel.Description ?? ev.Description;

            if (sessionUpdateModel.EventType != null)
                ev.EventType = sessionUpdateModel.EventType;

            if (sessionUpdateModel.EndTime != null)
                ev.EndTime = sessionUpdateModel.EndTime;
            if (sessionUpdateModel.StartTime != null)
                ev.StartTime = sessionUpdateModel.StartTime;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Event updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating session");
                return StatusCode(500, "An error occurred while updating the session.");
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<ActionResult> DeleteEvent(int id)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            Event tevent = await _context.Events.FirstOrDefaultAsync(u => u.Id == id && u.UserID == userId);

            try
            {
                _context.Events.Remove(tevent);
                await _context.SaveChangesAsync();
                return Ok("Event deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event");
                return StatusCode(500, "An error occurred while deleting the event.");
            }
        }

    }
}
