using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace EducationPlanServiceAPI.Models
{
    public class Session
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SessionType SessionType { get; set; }

        [ForeignKey("Schedule")]
        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; }
        public int UserID { get; set; }
    }
    public enum SessionType {
        [JsonPropertyName("lecture")]
        Lecture,
        [JsonPropertyName("practice")]
        Practice,
        [JsonPropertyName("consultation")]
        Consultation,
        [JsonPropertyName("credit")]
        Credit,
        [JsonPropertyName("exam")]
        Exam
    }
}
