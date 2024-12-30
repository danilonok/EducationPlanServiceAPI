namespace EducationPlanServiceAPI.Models
{
    public class TaskUpdateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DeadLine { get; set; }
        public int ScheduleId { get; set; }
    }
}
