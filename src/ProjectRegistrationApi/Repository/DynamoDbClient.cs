namespace ProjectRegistrationApi.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Amazon;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DocumentModel;
    using ProjectRegistrationApi.Models.Response;

    public class DynamoDbClient : IDynamoDbClient
    {
        private const string ProjectHoursTable = "ProjectHours";
        private const string ProjectTable = "Projects";

        private const string DateFormat = "yyyy-MM-dd";

        public async Task<Document> GetProjectById(string projectId)
        {
            var client = GetClient();
            var projectHoursTable = Table.LoadTable(client, ProjectTable);

            return await projectHoursTable.GetItemAsync(projectId);
        }

        public async Task<int> GetProjectTotalHours(string projectId)
        {
            var client = GetClient();
            var projectHoursTable = Table.LoadTable(client, ProjectHoursTable);

            var config = new QueryOperationConfig
            {
                AttributesToGet = new List<string> { "Hours" },
                Filter = new QueryFilter("ProjectId", QueryOperator.Equal, projectId),
                ConsistentRead = true,
                Select = SelectValues.SpecificAttributes
            };
            
            var search = projectHoursTable.Query(config);
            var hours = 0;

            var documentSet = new List<Document>();
            do
            {
                documentSet = await search.GetNextSetAsync();
                foreach (var document in documentSet)
                {
                    hours += int.Parse(document["Hours"].AsPrimitive().Value.ToString());
                }

            } while (!search.IsDone);
            return hours;
        }
        
        public async Task<List<ProjectHoursPerDay>> GetProjectHoursPerDateRange(string projectId, DateTime fromDate, DateTime toDate)
        {
            var client = GetClient();
            var projectHoursTable = Table.LoadTable(client, ProjectHoursTable);

            var filter = new QueryFilter("ProjectId", QueryOperator.Equal, projectId);
            filter.AddCondition("Day", QueryOperator.Between, fromDate.ToString(DateFormat), toDate.ToString(DateFormat));
            var config = new QueryOperationConfig
            {   
                Filter = filter,
                ConsistentRead = true
            };
            
            var search = projectHoursTable.Query(config);
            var projectHoursPerDay = new List<ProjectHoursPerDay>();

            var documentSet = new List<Document>();
            do
            {
                documentSet = await search.GetNextSetAsync();
                foreach (var document in documentSet)
                {
                    projectHoursPerDay.Add(new ProjectHoursPerDay
                                           {
                                               Day = DateTime.Parse(document["Day"].AsPrimitive().Value.ToString()),
                                               Hours = int.Parse(document["Hours"].AsPrimitive().Value.ToString())
                                           });
                }
            } while (!search.IsDone);

            return projectHoursPerDay;
        }

        public async Task SetProjectHours(string projectId, DateTime day, int hours)
        {
            var client = GetClient();
            var projectHoursTable = Table.LoadTable(client, ProjectHoursTable);

            var projectHoursItem = new Document();
            projectHoursItem["ProjectId"] = projectId;
            projectHoursItem["Day"] = day.ToString(DateFormat);
            projectHoursItem["Hours"] = hours;

            await projectHoursTable.UpdateItemAsync(projectHoursItem);
        }

        private static AmazonDynamoDBClient GetClient()
        {
            var clientConfig = new AmazonDynamoDBConfig();

            clientConfig.RegionEndpoint = RegionEndpoint.EUWest1;
            var client = new AmazonDynamoDBClient(clientConfig);

            return client;
        }
    
    }
}
