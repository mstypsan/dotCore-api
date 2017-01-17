namespace ProjectRegistrationApi.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using ProjectRegistrationApi.Models;
    using ProjectRegistrationApi.Models.Request;
    using ProjectRegistrationApi.Models.Response;
    using ProjectRegistrationApi.Repository;

    [Route("api/[controller]")]
    public class ProjectsController : Controller
    {
        private readonly IDynamoDbClient dynamoDbClient;

        public ProjectsController(IDynamoDbClient dynamoDbClient)
        {
            this.dynamoDbClient = dynamoDbClient;
        }

        [HttpGet("{projectId}/hours")]
        public ProjectHoursPerDayResponse GetProjectHours(string projectId, [FromQuery]GetProjectHourPerDayRequest hoursPerDateRange)
        {
            var projectHoursPerDay = dynamoDbClient.GetProjectHoursPerDateRange(projectId, hoursPerDateRange.FromDate, hoursPerDateRange.ToDate);

            return new ProjectHoursPerDayResponse
            {
                ProjectId = projectId,
                ProjectHoursPerDay = projectHoursPerDay
            };
        }

        [HttpPost("{projectId}/hours")]
        public NoContentResult SetProjectHours(string projectId, [FromBody]SetProjectHoursRequest hoursForDay)
        {
            dynamoDbClient.SetProjectHours(projectId, hoursForDay.Day, hoursForDay.Hours);

            return NoContent();
        }

        [HttpGet("{projectId}/totalhours")]
        public ProjectTotalHoursResponse GetProjectTotalHours(string projectId)
        {
            var hours = dynamoDbClient.GetProjectTotalHours(projectId);

            return new ProjectTotalHoursResponse { ProjectId = projectId, TotalHours = hours };
        }

    }
}
