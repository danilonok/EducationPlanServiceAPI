

using EducationPlanServiceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthServiceAPI.Data
{
    public class AppDBContext: DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options): base(options)
        {
        }
        public DbSet<Event> Events { get; set; }

        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Session> Sessions { get; set; }
    }
}
