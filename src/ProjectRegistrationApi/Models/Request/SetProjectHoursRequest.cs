namespace ProjectRegistrationApi.Models.Request
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SetProjectHoursRequest
    {
        [Required]
        public DateTime Day { get; set; }

        [Required]
        public int Hours { get; set; }
    }
}
