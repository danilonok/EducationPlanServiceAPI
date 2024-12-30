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
    public class ScheduleController : Controller
    {
        private readonly AppDBContext _context;
        private readonly ILogger<ScheduleController> _logger;
        private readonly string _secretKey;
        public ScheduleController(IConfiguration configuration, AppDBContext db_context, ILogger<ScheduleController> logger)
        {
            _secretKey = configuration["JWT_SECRET"];
            _context = db_context;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateSchedule([FromBody] ScheduleCreationModel scheduleCreationModel)
        {
            if (scheduleCreationModel != null)
            {
                var schedule = new Schedule
                {
                    CreatedDate = DateTime.UtcNow,
                    Title = scheduleCreationModel.Title,
                    Description = scheduleCreationModel.Description,
                    StartDate = DateTime.SpecifyKind(scheduleCreationModel.StartDate, DateTimeKind.Utc),

                   
                    EndDate = DateTime.SpecifyKind(scheduleCreationModel.EndDate, DateTimeKind.Utc),
                    UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                };
                _context.Schedules.Add(schedule);
                await _context.SaveChangesAsync();
                return Ok(new { id = schedule.Id, message = "Schedule was created succesfully." });
            }
            return BadRequest("Schedule parameters are invalid.");
        }


        [HttpGet()]
        [Authorize]
        public async Task<ActionResult> GetAllSchedules()
        {

            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            List<Schedule> schedules = await _context.Schedules.Where(e => e.UserID == userId).ToListAsync();
            if (schedules == null)
            {
                return NotFound("Event not found or you don't have permission to access this event.");
            }
            return Ok(schedules);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult> GetSchedule(int id)
        {

            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            Schedule schedule = await _context.Schedules.FirstOrDefaultAsync(u => u.Id == id && u.UserID == userId);
            if (schedule == null)
            {
                return NotFound("Schedule not found or you don't have permission to access this schedule.");
            }
            return Ok(schedule);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult> UpdateSchedule(int id, [FromBody] ScheduleUpdateModel scheduleUpdateModel)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            Schedule schedule = await _context.Schedules.FirstOrDefaultAsync(u => u.Id == id && u.UserID == userId);
            if (schedule == null)
            {
                return NotFound("Schedule not found or you don't have permission to access this schedule.");
            }

            schedule.Title = scheduleUpdateModel.Title ?? schedule.Title;
            schedule.Description = scheduleUpdateModel.Description ?? schedule.Description;
            if (scheduleUpdateModel.EndDate != null)
                schedule.EndDate = scheduleUpdateModel.EndDate;
            if (scheduleUpdateModel.StartDate != null)
                schedule.StartDate = scheduleUpdateModel.StartDate;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Schedule updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event");
                return StatusCode(500, "An error occurred while updating the schedule.");
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<ActionResult> DeleteSchedule(int id)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            Schedule schedule = await _context.Schedules.FirstOrDefaultAsync(u => u.Id == id && u.UserID == userId);

            try
            {
                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();
                return Ok("Schedule deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting schedule");
                return StatusCode(500, "An error occurred while deleting the schedule.");
            }
        }

    }
}
