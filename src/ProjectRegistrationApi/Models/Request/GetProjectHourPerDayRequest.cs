namespace ProjectRegistrationApi.Models.Request
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class GetProjectHourPerDayRequest
    {
        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }
        
    }
}
