using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using EducationPlanServiceAPI.Models;
using EducationPlanServiceAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration.UserSecrets;
using EducationPlanServiceAPI.RabbitMQ;
using System.Text.Json;

namespace EducationPlanServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : Controller
    {
        private readonly AppDBContext _context;
        private readonly ILogger<TaskController> _logger;
        private readonly RabbitMqProducer _producer;
        public TaskController(IConfiguration configuration, AppDBContext db_context, ILogger<TaskController> logger, RabbitMqProducer producer)
        {
            _context = db_context;
            _logger = logger;
            _producer = producer;
        }
        
        [HttpPost("")]
        [Authorize]
        public async Task<ActionResult> CreateTask([FromBody] TaskUpdateDto taskUpdateModel)
        {
            if (taskUpdateModel != null)
            {
                var task = new EducationTask
                {
                    CreatedDate = DateTime.UtcNow,
                    Title = taskUpdateModel.Title,
                    Description = taskUpdateModel.Description,
                    DeadLine =DateTime.SpecifyKind(taskUpdateModel.DeadLine, DateTimeKind.Utc),

                    ScheduleId = taskUpdateModel.ScheduleId,
                    Status = EducationTaskStatus.Incomplete,
                    UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)
                };
                
                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();
                var message = JsonSerializer.Serialize(new
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    DeadLine = task.DeadLine,
                    ScheduleId = task.ScheduleId,
                    Status = task.Status.ToString(),
                    UserID = task.UserID,
                    CreatedDate = task.CreatedDate
                });

                _producer.SendMessage("my-queue", message);

                return Ok(new { id = task.Id, message = "Task was created succesfully."});
            }
            return BadRequest("Task parameters are invalid.");
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult> GetTask(int id)
        {
            
            if(!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            EducationTask ev = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == id && u.UserID == userId);
            if (ev == null)
            {
                return NotFound("Task not found or you don't have permission to access this task.");
            }
            return Ok(ev);
        }

        [HttpGet()]
        [Authorize]
        public async Task<ActionResult> GetAllTasks()
        {

            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            List<EducationTask> tasks = await _context.Tasks.Where(e => e.UserID == userId).ToListAsync();
            if (tasks == null)
            {
                return NotFound("Task not found or you don't have permission to access this task.");
            }
            return Ok(tasks);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult> UpdateTask(int id, [FromBody] TaskUpdateDto taskUpdateModel)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            EducationTask ev = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == id && u.UserID == userId);
            if (ev == null)
            {
                return NotFound("Task not found or you don't have permission to access this task.");
            }

            ev.Title = taskUpdateModel.Title ?? ev.Title;
            ev.Description = taskUpdateModel.Description ?? ev.Description;
            
            if (taskUpdateModel.DeadLine != null)
                ev.DeadLine = taskUpdateModel.DeadLine;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Task updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task");
                return StatusCode(500, "An error occurred while updating the task.");
            }
        }

        [HttpPatch("{id:int}/status")]
        [Authorize]
        public async Task<ActionResult> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusDto dto)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }
            EducationTask ev = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == id && u.UserID == userId);
            if (ev == null)
            {
                return NotFound("Task not found or you don't have permission to access this task.");
            }

            ev.Status = dto.Status;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Task updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task");
                return StatusCode(500, "An error occurred while updating the task.");
            }
        }


        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<ActionResult> DeleteTask(int id)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            EducationTask task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == id && u.UserID == userId);

            try
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                return Ok("Task deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task");
                return StatusCode(500, "An error occurred while deleting the task.");
            }
        }




    }
}
