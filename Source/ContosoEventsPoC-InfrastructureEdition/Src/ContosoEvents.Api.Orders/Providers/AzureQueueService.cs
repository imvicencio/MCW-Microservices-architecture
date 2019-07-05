using System;
using System.Threading.Tasks;
using ContosoEvents.Api.Orders.Models;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ContosoEvents.Api.Orders.Providers
{
    public class AzureQueueService : IQueueService
    {
        private CloudQueue queue;
        private readonly ILogger<AzureQueueService> logger;
        private readonly string storageConnectionString;
        private readonly string queueName;

        public AzureQueueService(IConfiguration configuration, ILogger<AzureQueueService> logger)
        {
            this.logger = logger;
            storageConnectionString = configuration["storageConnectionString"];
            queueName = configuration["queueName"];

            Initialize();
        }

        public async Task QueueOrderAsync(Order order)
        {

            try
            {
                if (queue != null)
                {
                    var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(order));
                    await queue.AddMessageAsync(queueMessage);
                }

            }
            catch (Exception ex)
            {
                logger?.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private void Initialize()
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

                if (!string.IsNullOrWhiteSpace(queueName))
                {
                    queue = queueClient.GetQueueReference(queueName);
                    queue.CreateIfNotExists();
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
