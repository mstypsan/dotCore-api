namespace ProjectRegistrationApi.Models.Response
{
    using System;
    using System.Collections.Generic;

    public class ProjectHoursPerDayResponse
    {
        public string ProjectId { get; set; }
        public List<ProjectHoursPerDay> ProjectHoursPerDay { get; set; }
    }

    public class ProjectHoursPerDay
    {
        public DateTime Day { get; set; }

        public int Hours { get; set; }
    }
}
