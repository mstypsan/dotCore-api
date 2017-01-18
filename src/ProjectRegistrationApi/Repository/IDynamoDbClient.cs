namespace ProjectRegistrationApi.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Amazon.DynamoDBv2.DocumentModel;
    using ProjectRegistrationApi.Models.Response;

    public interface IDynamoDbClient
    {
        Task<Document> GetProjectById(string projectId);
        Task<int> GetProjectTotalHours(string id);

        Task SetProjectHours(string projectId, DateTime day, int hours);

        Task<List<ProjectHoursPerDay>> GetProjectHoursPerDateRange(string projectId, DateTime fromDate, DateTime toDate);
    }
}
