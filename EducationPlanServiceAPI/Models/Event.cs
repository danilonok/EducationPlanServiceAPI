using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace EducationPlanServiceAPI.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public EventType EventType { get; set; }

        [ForeignKey("Schedule")]
        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; }
        public int UserID { get; set; }
    }
    public enum EventType {
        Lecture,
        Practice,
        Consultation,
        Credit,
        Exam
    }
}
