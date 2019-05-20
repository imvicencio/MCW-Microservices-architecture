using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoEvents.Models;
using ContosoEvents.Shared.Handlers;
using Microsoft.Azure.Cosmos;

namespace ContosoEvents.Shared.Services
{
    public class DataStoreService : IDataStoreService
    {
        private const string LOG_TAG = "DataStoreService";
        private ISettingService settingService;
        private ILoggerService logger;
        private string endpointUri;
        private string primaryKey;
        private string databaseName;
        private string eventsContainerName;
        private string ordersContainerName;
        private string logMessagesContainerName;

        public DataStoreService(ISettingService settingService, ILoggerService logger)
        {
            this.settingService = settingService;
            this.logger = logger;

            endpointUri = settingService.GetDataStoreEndpointUri();
            primaryKey = settingService.GetDataStorePrimaryKey();
            databaseName = settingService.GetDataStoreDatabaseName();
            eventsContainerName = settingService.GetDataStoreEventsCollectionName();
            ordersContainerName = settingService.GetDataStoreOrdersCollectionName();
            logMessagesContainerName = settingService.GetDataStoreLogMessagesCollectionName();

            // Initialize Data Store
            Initialize().Wait();
        }

        public async Task<string> CreateEvent(TicketEvent tEvent)
        {
            var error = string.Empty;
            var handler = HandlersFactory.GetProfilerHandler(settingService, logger);
            handler.Start(LOG_TAG, "CreateEvent", null);
            var id = tEvent.Id;

            try
            {
                using (CosmosClient cosmosClient = new CosmosClient(endpointUri, primaryKey))
                {
                    var databaseResponse = await cosmosClient.Databases.CreateDatabaseIfNotExistsAsync(databaseName);

                    var containerResponse = await databaseResponse.Database.Containers.CreateContainerIfNotExistsAsync(eventsContainerName, "/id");

                    await containerResponse.Container.Items.CreateItemAsync(tEvent.Id, tEvent);
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                throw new Exception(error);
            }
            finally
            {
                handler.Stop(error);
            }

            return id;
        }

        public Task DeleteAllEvents()
        {
            return Task.CompletedTask;
        }

        public Task DeleteAllLogMessages()
        {
            return Task.CompletedTask;
        }

        public Task DeleteAllOrders()
        {
            return Task.CompletedTask;
        }

        public async Task<TicketEvent> GetEventById(string eventId)
        {
            var error = string.Empty;
            var handler = HandlersFactory.GetProfilerHandler(settingService, logger);
            handler.Start(LOG_TAG, "GetEventById", null);

            try
            {
                using (CosmosClient cosmosClient = new CosmosClient(endpointUri, primaryKey))
                {
                    var databaseResponse = await cosmosClient.Databases.CreateDatabaseIfNotExistsAsync(databaseName);

                    var containerResponse = await databaseResponse.Database.Containers.CreateContainerIfNotExistsAsync(eventsContainerName, "/id");

                    var query = $"SELECT * FROM Events e WHERE e.id = '{eventId}'";
                    var queryDefinition = new CosmosSqlQueryDefinition(query);
                    var iterator = containerResponse.Container.Items.CreateItemQuery<TicketEvent>(queryDefinition, maxConcurrency: 1);

                    var ticketEvents = new List<TicketEvent>();

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
                error = ex.Message;
                throw new Exception(error);
            }
            finally
            {
                handler.Stop(error);
            }
        }

        public async Task<List<TicketEvent>> GetEvents()
        {
            var error = string.Empty;
            var handler = HandlersFactory.GetProfilerHandler(settingService, logger);
            handler.Start(LOG_TAG, "GetEvents", null);

            try
            {
                using (CosmosClient cosmosClient = new CosmosClient(endpointUri, primaryKey))
                {
                    var databaseResponse = await cosmosClient.Databases.CreateDatabaseIfNotExistsAsync(databaseName);

                    var containerResponse = await databaseResponse.Database.Containers.CreateContainerIfNotExistsAsync(eventsContainerName, "/id");

                    var query = $"SELECT * FROM Events";
                    var queryDefinition = new CosmosSqlQueryDefinition(query);
                    var iterator = containerResponse.Container.Items.CreateItemQuery<TicketEvent>(queryDefinition, maxConcurrency: 1);

                    var ticketEvents = new List<TicketEvent>();

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
                error = ex.Message;
                throw new Exception(error);
            }
            finally
            {
                handler.Stop(error);
            }
        }

        public async Task<TicketEventStats> GetEventStats(string eventId)
        {
            var error = string.Empty;
            var handler = HandlersFactory.GetProfilerHandler(settingService, logger);
            handler.Start(LOG_TAG, "GetEventStats", null);

            try
            {
                var ticketEvent = await GetEventById(eventId);

                if (ticketEvent == null)
                {
                    return new TicketEventStats();
                }

                using (CosmosClient cosmosClient = new CosmosClient(endpointUri, primaryKey))
                {
                    var databaseResponse = await cosmosClient.Databases.CreateDatabaseIfNotExistsAsync(databaseName);

                    var containerResponse = await databaseResponse.Database.Containers.CreateContainerIfNotExistsAsync(ordersContainerName, "/EventId");

                    var query = $"SELECT * FROM Orders o WHERE o.EventId = '{eventId}'";
                    var queryDefinition = new CosmosSqlQueryDefinition(query);
                    var iterator = containerResponse.Container.Items.CreateItemQuery<TicketOrder>(queryDefinition, maxConcurrency: 1);

                    var ticketOrders = new List<TicketOrder>();

                    while (iterator.HasMoreResults)
                    {
                        var queryResponse = await iterator.FetchNextSetAsync();
                        foreach (var item in queryResponse)
                        {
                            ticketOrders.Add(item);
                        }
                    }

                    var requestedCount = ticketOrders.Sum(o => o.Tickets);
                    var failedCount = ticketOrders.Where(o => o.IsFulfilled == false && o.IsCancelled == false).Sum(o => o.Tickets);
                    var canxCount = ticketOrders.Where(o => o.IsFulfilled == false && o.IsCancelled == true).Sum(o => o.Tickets);

                    return new TicketEventStats()
                    {
                        Tickets = ticketEvent.TotalTickets,
                        RequestedTickets = requestedCount,
                        FailedTickets = failedCount,
                        CancelledTickets = canxCount,
                        Orders = ticketOrders.Count
                    };
                }

            }
            catch (Exception ex)
            {
                error = ex.Message;
                throw new Exception(error);
            }
            finally
            {
                handler.Stop(error);
            }
        }

        public async Task<List<TicketOrder>> GetOrdersByUserName(string userName)
        {
            var error = string.Empty;
            var handler = HandlersFactory.GetProfilerHandler(settingService, logger);
            handler.Start(LOG_TAG, "GetOrdersByUserName", null);

            try
            {
                using (CosmosClient cosmosClient = new CosmosClient(endpointUri, primaryKey))
                {
                    var databaseResponse = await cosmosClient.Databases.CreateDatabaseIfNotExistsAsync(databaseName);

                    var containerResponse = await databaseResponse.Database.Containers.CreateContainerIfNotExistsAsync(ordersContainerName, "/EventId");

                    var query = $"SELECT * FROM Orders o WHERE o.UserName = '{userName}'";
                    var queryDefinition = new CosmosSqlQueryDefinition(query);
                    var iterator = containerResponse.Container.Items.CreateItemQuery<TicketOrder>(queryDefinition, maxConcurrency: 1);

                    var ticketOrders = new List<TicketOrder>();

                    while (iterator.HasMoreResults)
                    {
                        var queryResponse = await iterator.FetchNextSetAsync();
                        foreach (var item in queryResponse)
                        {
                            ticketOrders.Add(item);
                        }
                    }

                    return ticketOrders.OrderByDescending(o => o.OrderDate).ToList();
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                throw new Exception(error);
            }
            finally
            {
                handler.Stop(error);
            }
        }

        public async Task<List<TicketOrderStats>> GetOrderStats()
        {
            var error = string.Empty;
            var handler = HandlersFactory.GetProfilerHandler(settingService, logger);
            handler.Start(LOG_TAG, "GetOrderStats", null);

            try
            {
                using (CosmosClient cosmosClient = new CosmosClient(endpointUri, primaryKey))
                {
                    var databaseResponse = await cosmosClient.Databases.CreateDatabaseIfNotExistsAsync(databaseName);

                    var containerResponse = await databaseResponse.Database.Containers.CreateContainerIfNotExistsAsync(ordersContainerName, "/EventId");

                    var query = $"SELECT * FROM Orders o WHERE IS_NULL(o.FulfillDate)=false";
                    var queryDefinition = new CosmosSqlQueryDefinition(query);
                    var iterator = containerResponse.Container.Items.CreateItemQuery<TicketOrder>(queryDefinition, maxConcurrency: 1);

                    var ticketOrders = new List<TicketOrder>();

                    while (iterator.HasMoreResults)
                    {
                        var queryResponse = await iterator.FetchNextSetAsync();
                        foreach (var item in queryResponse)
                        {
                            ticketOrders.Add(item);
                        }
                    }

                    return ticketOrders.GroupBy(o => new { ID = o.Tag }).Select(g => new TicketOrderStats { Tag = g.Key.ID, Count = g.Count(), SumSeconds = g.Sum(x => ((DateTime)x.FulfillDate - x.OrderDate).TotalSeconds), AverageSeconds = g.Average(x => ((DateTime)x.FulfillDate - x.OrderDate).TotalSeconds) }).ToList();
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                throw new Exception(error);
            }
            finally
            {
                handler.Stop(error);
            }
        }

        private async Task Initialize()
        {
            var error = string.Empty;
            var handler = HandlersFactory.GetProfilerHandler(settingService, logger);
            handler.Start(LOG_TAG, "Initialize", null);

            try
            {
                using (CosmosClient cosmosClient = new CosmosClient(endpointUri, primaryKey))
                {
                    var databaseResponse = await cosmosClient.Databases.CreateDatabaseIfNotExistsAsync(databaseName);

                    var containerResponse = await databaseResponse.Database.Containers.CreateContainerIfNotExistsAsync(eventsContainerName, "/id");

                    var event1 = CreateTicketEvent();

                    var itemResponse = await containerResponse.Container.Items.ReadItemAsync<TicketEvent>(event1.Id, event1.Id);

                    if (itemResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        var ticketEventCreationResponse = await containerResponse.Container.Items.CreateItemAsync(event1.Id, event1);
                    }
                }

            }
            catch (Exception ex)
            {
                error = ex.Message;
                throw new Exception(error);
            }
            finally
            {
                handler.Stop(error);
            }
        }

        private TicketEvent CreateTicketEvent()
        {
            return new TicketEvent()
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