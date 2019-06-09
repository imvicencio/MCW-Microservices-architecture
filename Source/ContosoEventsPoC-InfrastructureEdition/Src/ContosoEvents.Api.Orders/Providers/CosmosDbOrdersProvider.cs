using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoEvents.Api.Orders.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ContosoEvents.Api.Orders.Providers
{
    public class CosmosDbOrdersProvider : IOrdersProvider
    {
        private readonly string accountEndpoint;
        private readonly string accountKey;
        private readonly string databaseName;
        private readonly string ordersContainerName;

        private readonly ILogger<CosmosDbOrdersProvider> logger;
        public CosmosDbOrdersProvider(IConfiguration configuration, ILogger<CosmosDbOrdersProvider> logger)
        {
            accountEndpoint = configuration["accountEndpoint"];
            accountKey = configuration["accountKey"];
            databaseName = configuration["databaseName"];
            ordersContainerName = configuration["ordersContainerName"];

            this.logger = logger;
        }

        public async Task<ICollection<Order>> GetUserOrdersAsync(string username)
        {
            try
            {
                using (CosmosClient cosmosClient = new CosmosClient(accountEndpoint, accountKey))
                {
                    var databaseResponse = await cosmosClient.Databases.CreateDatabaseIfNotExistsAsync(databaseName);

                    var containerResponse = await databaseResponse.Database.Containers.CreateContainerIfNotExistsAsync(ordersContainerName, "/EventId");

                    var query = $"SELECT * FROM Orders o WHERE o.UserName = '{username}'";
                    var queryDefinition = new CosmosSqlQueryDefinition(query);
                    var iterator = containerResponse.Container.Items.CreateItemQuery<Order>(queryDefinition, maxConcurrency: 1);

                    var ticketOrders = new List<Order>();

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
                logger?.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
