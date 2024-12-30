using System.ComponentModel.DataAnnotations.Schema;

namespace EducationPlanServiceAPI.Models
{
    public class EducationTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DeadLine { get; set; }
        public int UserID { get; set; }
        public Schedule Schedule { get; set; }
        [ForeignKey("Schedule")]
        public int ScheduleId { get; set; }
        public EducationTaskStatus Status { get; set; }
    }
    public enum EducationTaskStatus
    {
        Complete,
        Incomplete
    }
}
