namespace Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Amazon.DynamoDBv2.DocumentModel;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using ProjectRegistrationApi.Controllers;
    using ProjectRegistrationApi.Models.Request;
    using ProjectRegistrationApi.Models.Response;
    using ProjectRegistrationApi.Repository;
    using Xunit;

    public class ProjectsControllerTests
    {
        private readonly Mock<IDynamoDbClient> dynamoDbClient;

        private readonly ProjectsController projectsController;

        public ProjectsControllerTests()
        {
            dynamoDbClient = new Mock<IDynamoDbClient>();
            projectsController = new ProjectsController(dynamoDbClient.Object);
        }

        [Fact]
        public async Task GetProjectHours_WhenFromDatesIsGreaterThanToDate_Throw400()
        {
            var request = new GetProjectHourPerDayRequest { FromDate = new DateTime(2016, 10, 10), ToDate = new DateTime(2015, 10, 10)  };

            var response = await projectsController.GetProjectHours(It.IsAny<string>(), request);
            
            Assert.Equal(((ObjectResult)response).StatusCode, 400);
        }

        [Fact]
        public async Task GetProjectHours_WhenProjectIsNotFound_Throw404()
        {
            var request = new GetProjectHourPerDayRequest { FromDate = new DateTime(2016, 10, 10), ToDate = new DateTime(2017, 10, 10) };
            dynamoDbClient.Setup(x => x.GetProjectById(It.IsAny<string>())).
                Returns(Task.FromResult<Document>(null));

            var response = await projectsController.GetProjectHours(It.IsAny<string>(), request);

            Assert.Equal(((ObjectResult)response).StatusCode, 404);
        }

        [Fact]
        public async Task GetProjectHours_WhenProjectIsFound_ReturnTheResult()
        {
            var request = new GetProjectHourPerDayRequest { FromDate = new DateTime(2016, 10, 10), ToDate = new DateTime(2017, 10, 10) };
            dynamoDbClient.Setup(x => x.GetProjectById(It.IsAny<string>())).
                Returns(Task.FromResult(new Document()));

            var day = new DateTime(2016, 11, 11);
            var hours = 10;
            var projectHoursPerDay = new List<ProjectHoursPerDay> { new ProjectHoursPerDay { Day = day, Hours = hours } };
            dynamoDbClient.Setup(x => x.GetProjectHoursPerDateRange(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).
                Returns(Task.FromResult(projectHoursPerDay));

            var response = await projectsController.GetProjectHours(It.IsAny<string>(), request);
            
            var responseResult = (ObjectResult)response;
            Assert.Equal(responseResult.StatusCode, 200);
            Assert.Equal(((ProjectHoursPerDayResponse)responseResult.Value).ProjectHoursPerDay[0].Day, day);
            Assert.Equal(((ProjectHoursPerDayResponse)responseResult.Value).ProjectHoursPerDay[0].Hours, hours);
        }

        [Fact]
        public async Task SetProjectHours_WhenProjectIsNotFound_Throw404()
        {
            var request = new SetProjectHoursRequest { Day = new DateTime(2016, 10, 10), Hours = 1 };
            dynamoDbClient.Setup(x => x.GetProjectById(It.IsAny<string>())).
                Returns(Task.FromResult<Document>(null));

            var response = await projectsController.SetProjectHours(It.IsAny<string>(), request);

            Assert.Equal(((ObjectResult)response).StatusCode, 404);
        }

        [Fact]
        public async Task SetProjectHours_WhenProjectIsFound_SetTheHours()
        {
            var projectId = "id";
            var day = new DateTime(2016, 10, 10);
            var hours = 1;
            var request = new SetProjectHoursRequest { Day = day, Hours = hours };
            dynamoDbClient.Setup(x => x.GetProjectById(projectId)).
                Returns(Task.FromResult(new Document()));
            
            var response = await projectsController.SetProjectHours(projectId, request);

            Assert.Equal(((NoContentResult)response).StatusCode, 204);
            dynamoDbClient.Verify(x => x.SetProjectHours(projectId, day, hours), Times.Once);
        }

        [Fact]
        public async Task GetProjectTotalHours_WhenProjectIsNotFound_Throw404()
        {
            dynamoDbClient.Setup(x => x.GetProjectById(It.IsAny<string>())).
                Returns(Task.FromResult<Document>(null));

            var response = await projectsController.GetProjectTotalHours(It.IsAny<string>());

            Assert.Equal(((ObjectResult)response).StatusCode, 404);
        }

        [Fact]
        public async Task GetProjectTotalHours_WhenProjectIsFound_ReturnTheResult()
        {
            var hours = 10;
            dynamoDbClient.Setup(x => x.GetProjectById(It.IsAny<string>())).
                Returns(Task.FromResult(new Document()));
            dynamoDbClient.Setup(x => x.GetProjectTotalHours(It.IsAny<string>())).
                Returns(Task.FromResult(hours));

            var response = await projectsController.GetProjectTotalHours(It.IsAny<string>());

            var responseResult = (ObjectResult)response;
            Assert.Equal(responseResult.StatusCode, 200);
            Assert.Equal(((ProjectTotalHoursResponse)responseResult.Value).TotalHours, hours);
        }
    }
}
