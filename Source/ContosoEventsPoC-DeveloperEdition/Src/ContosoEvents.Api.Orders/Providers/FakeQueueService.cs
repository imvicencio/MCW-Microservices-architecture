using System.Collections.Concurrent;
using System.Threading.Tasks;
using ContosoEvents.Api.Orders.Models;

namespace ContosoEvents.Api.Orders.Providers
{
    public class FakeQueueService : IQueueService
    {
        private readonly ConcurrentQueue<Order> queue = new ConcurrentQueue<Order>();
        public Task QueueOrderAsync(Order order)
        {
            queue.Enqueue(order);
            return Task.CompletedTask;
        }
    }
}
