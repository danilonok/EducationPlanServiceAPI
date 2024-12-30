namespace EducationPlanServiceAPI.Models
{
    public class EventCreationDto
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
        public EventType EventType { get; set; }
        public int ScheduleId { get; set; }
    }
}
