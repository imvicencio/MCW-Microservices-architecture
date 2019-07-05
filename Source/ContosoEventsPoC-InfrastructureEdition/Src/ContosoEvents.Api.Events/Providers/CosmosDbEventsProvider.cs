using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoEvents.Api.Events.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ContosoEvents.Api.Events.Providers
{
    public class CosmosDbEventsProvider : IEventsProvider
    {
        private readonly string accountEndpoint;
        private readonly string accountKey;
        private readonly string databaseName;
        private readonly string eventsContainerName;
        
        private readonly ILogger<CosmosDbEventsProvider> logger;

        public CosmosDbEventsProvider(IConfiguration configuration, ILogger<CosmosDbEventsProvider> logger)
        {
            accountEndpoint = configuration["accountEndpoint"];
            accountKey = configuration["accountKey"];
            databaseName = configuration["databaseName"];
            eventsContainerName = configuration["eventsContainerName"];

            this.logger = logger;

            InitializeAsync().Wait();
        }

        public async Task<ICollection<Event>> GetAllEventsAsync()
        {
            try
            {
                using (CosmosClient cosmosClient = new CosmosClient(accountEndpoint, accountKey))
                {
                    var databaseResponse = await cosmosClient.Databases.CreateDatabaseIfNotExistsAsync(databaseName);

                    var containerResponse = await databaseResponse.Database.Containers.CreateContainerIfNotExistsAsync(eventsContainerName, "/id");

                    var query = $"SELECT * FROM Events";
                    var queryDefinition = new CosmosSqlQueryDefinition(query);
                    var iterator = containerResponse.Container.Items.CreateItemQuery<Event>(queryDefinition, maxConcurrency: 1);

                    var ticketEvents = new List<Event>();

                    while (iterator.HasMoreResults)
                    {
                        var queryResponse = await iterator.FetchNextSetAsync();
                        foreach (var item in queryResponse)
                        {
                            ticketEvents.Add(item);
                        }
                    }

                    return ticketEvents;
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<Event> GetEventAsync(string id)
        {
            try
            {
                using (CosmosClient cosmosClient = new CosmosClient(accountEndpoint, accountKey))
                {
                    var databaseResponse = await cosmosClient.Databases.CreateDatabaseIfNotExistsAsync(databaseName);

                    var containerResponse = await databaseResponse.Database.Containers.CreateContainerIfNotExistsAsync(eventsContainerName, "/id");

                    var query = $"SELECT * FROM Events e WHERE e.id = '{id}'";
                    var queryDefinition = new CosmosSqlQueryDefinition(query);
                    var iterator = containerResponse.Container.Items.CreateItemQuery<Event>(queryDefinition, maxConcurrency: 1);

                    var ticketEvents = new List<Event>();

                    while (iterator.HasMoreResults)
                    {
                        var queryResponse = await iterator.FetchNextSetAsync();
                        foreach (var item in queryResponse)
                        {
                            ticketEvents.Add(item);
                        }
                    }

                    return ticketEvents.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private async Task InitializeAsync()
        {
            try
            {
                using (CosmosClient cosmosClient = new CosmosClient(accountEndpoint, accountKey))
                {
                    var databaseResponse = await cosmosClient.Databases.CreateDatabaseIfNotExistsAsync(databaseName);

                    var containerResponse = await databaseResponse.Database.Containers.CreateContainerIfNotExistsAsync(eventsContainerName, "/id");

                    var event1 = CreateDemoEvent();

                    var itemResponse = await containerResponse.Container.Items.ReadItemAsync<Event>(event1.Id, event1.Id);

                    if (itemResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        var ticketEventCreationResponse = await containerResponse.Container.Items.CreateItemAsync(event1.Id, event1);
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private Event CreateDemoEvent()
        {
            return new Event()
            {
                Id = "EVENT1-ID-00001",
                Name = "Seattle Rock and Rollers",
                Summary = "Seattle Rock and Rollers Summary",
                Description = "Seattle Rock and Rollers Description",
                ImageUrl = "http://www.loremimages.com/gen.php?size=300x200&bg=0000&fg=fff&format=png",
                Latitude = 0,
                Longitude = 0,
                StartDate = DateTime.Now.AddDays(-60),
                EndDate = DateTime.Now.AddDays(60),
                TotalTickets = 100000,
                PricePerTicket = 25,
                Currency = "USD",
                PaymentProcessorUrl = string.Empty,
                PaymentProcessorAccount = string.Empty,
                PaymentProcessorPassword = string.Empty,
                SuccessEmailTemplate = @"
                            <h1> Ticket Purchase Notification </h1>
                            Dear @Model.UserName, 
                            <p>
                            Congratulations! Your have @Model.Tickets tickets guranateed to Seattle Rock and Rollers.
                            </p>
                            ",
                FailedEmailTemplate = @"
                            <h1> Ticket Purchase Failure Notification </h1>
                            Dear @Model.UserName, 
                            <p>
                            Sorry! Your @Model.Tickets tickets could not be purchased to Seattle Rock and Rollers.
                            </p>
                            ",
                SuccessSmsTemplate = @"
                            <h1> Ticket Purchase Notification </h1>
                            Dear @Model.UserName, 
                            <p>
                            Congratulations! Your have @Model.Tickets tickets guranateed to Seattle Rock and Rollers.
                            </p>
                            ",
                FailedSmsTemplate = @"
                            <h1> Ticket Purchase Failure Notification </h1>
                            Dear @Model.UserName, 
                            <p>
                            Sorry! Your @Model.Tickets tickets could not be purchased to Seattle Rock and Rollers.
                            </p>
                            "
            };
        }
    }
}