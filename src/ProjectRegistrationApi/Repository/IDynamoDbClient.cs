namespace ProjectRegistrationApi.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ProjectRegistrationApi.Models.Response;

    public interface IDynamoDbClient
    {
        int GetProjectTotalHours(string id);

        Task SetProjectHours(string projectId, DateTime day, int hours);

        List<ProjectHoursPerDay> GetProjectHoursPerDateRange(string projectId, DateTime fromDate, DateTime toDate);
    }
}
