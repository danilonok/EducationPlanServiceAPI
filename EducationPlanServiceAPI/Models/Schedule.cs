namespace EducationPlanServiceAPI.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UserID { get; set; }
        public ICollection<Event> Sessions { get; set; } = new List<Event>();

        public ICollection<EducationTask> Tasks { get; set; } = new List<EducationTask>();
    }
}
