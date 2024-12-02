using AuthServiceAPI.Data;
using EducationPlanServiceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EducationPlanServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : Controller
    {
        private readonly AppDBContext _context;
        private readonly ILogger<ScheduleController> _logger;
        private readonly string _secretKey;
        public SessionController(IConfiguration configuration, AppDBContext db_context, ILogger<ScheduleController> logger)
        {
            _secretKey = configuration["JWT_SECRET"];
            _context = db_context;
            _logger = logger;
        }

        [HttpPost("session")]
        [Authorize]
        public async Task<ActionResult> CreateSession([FromBody] SessionCreationModel sessionCreationModel)
        {
            if (sessionCreationModel != null)
            {
                _context.Sessions.Add(new Session { CreatedDate = DateTime.UtcNow, Title = sessionCreationModel.Title, Description = sessionCreationModel.Description, StartTime = sessionCreationModel.StartTime, EndTime = sessionCreationModel.EndTime, SessionType = sessionCreationModel.SessionType, ScheduleId = sessionCreationModel.ScheduleId, UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value) });
                await _context.SaveChangesAsync();
                return Ok("Session was created succesfully.");
            }
            return BadRequest("Session parameters are invalid.");
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult> GetSession(int id)
        {

            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            Session session = await _context.Sessions.FirstOrDefaultAsync(u => u.Id == id && u.UserID == userId);
            if (session == null)
            {
                return NotFound("Session not found or you don't have permission to access this session.");
            }
            return Ok(session);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult> UpdateSession(int id, [FromBody] SessionUpdateModel sessionUpdateModel)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            Session session = await _context.Sessions.FirstOrDefaultAsync(u => u.Id == id && u.UserID == userId);
            if (session == null)
            {
                return NotFound("Session not found or you don't have permission to access this session.");
            }

            session.Title = sessionUpdateModel.Title ?? session.Title;
            session.Description = sessionUpdateModel.Description ?? session.Description;

            if (sessionUpdateModel.SessionType != null)
                session.SessionType = sessionUpdateModel.SessionType;

            if (sessionUpdateModel.EndTime != null)
                session.EndTime = sessionUpdateModel.EndTime;
            if (sessionUpdateModel.StartTime != null)
                session.StartTime = sessionUpdateModel.StartTime;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Session updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating session");
                return StatusCode(500, "An error occurred while updating the session.");
            }
        }

    }
}
