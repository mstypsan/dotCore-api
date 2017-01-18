namespace ProjectRegistrationApi.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetProjectHours(string projectId, [FromQuery]GetProjectHourPerDayRequest hoursPerDateRange)
        {
            var project = await dynamoDbClient.GetProjectById(projectId);

            if (project == null)
            {
                return NotFound($"Projectid {projectId} is not found");
            }
            
            var projectHoursPerDay = await dynamoDbClient.GetProjectHoursPerDateRange(projectId, hoursPerDateRange.FromDate, hoursPerDateRange.ToDate);

            var response = new ProjectHoursPerDayResponse
            {
                ProjectId = projectId,
                ProjectHoursPerDay = projectHoursPerDay
            };

            return Ok(response);

        }

        [HttpPost("{projectId}/hours")]
        public async Task<IActionResult> SetProjectHours(string projectId, [FromBody]SetProjectHoursRequest hoursForDay)
        {
            var project = await dynamoDbClient.GetProjectById(projectId);

            if (project == null)
            {
                return NotFound($"Projectid {projectId} is not found");
            }

            await dynamoDbClient.SetProjectHours(projectId, hoursForDay.Day, hoursForDay.Hours);

            return NoContent();
        }

        [HttpGet("{projectId}/totalhours")]
        public async Task<IActionResult> GetProjectTotalHours(string projectId)
        {
            var project = await dynamoDbClient.GetProjectById(projectId);

            if (project == null)
            {
                return NotFound($"Projectid {projectId} is not found");
            }

            var hours = await dynamoDbClient.GetProjectTotalHours(projectId);

            var response = new ProjectTotalHoursResponse { ProjectId = projectId, TotalHours = hours };

            return Ok(response);
        }

    }
}
