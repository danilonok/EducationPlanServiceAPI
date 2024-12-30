

using EducationPlanServiceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EducationPlanServiceAPI.Data
{
    public class AppDBContext: DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options): base(options)
        {
        }
        public DbSet<EducationTask> Tasks { get; set; }

        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Event> Events { get; set; }
    }
}
