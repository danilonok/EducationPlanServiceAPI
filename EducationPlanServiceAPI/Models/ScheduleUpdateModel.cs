﻿namespace EducationPlanServiceAPI.Models
{
    public class ScheduleUpdateModel
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
