namespace EducationPlanServiceAPI.Models
{
    public class SessionUpdateModel
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
        public SessionType SessionType { get; set; }

    }
}
