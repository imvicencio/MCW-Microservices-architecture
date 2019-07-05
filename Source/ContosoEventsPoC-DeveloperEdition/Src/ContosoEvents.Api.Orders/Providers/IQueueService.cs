using System.Threading.Tasks;
using ContosoEvents.Api.Orders.Models;

namespace ContosoEvents.Api.Orders.Providers
{
    public interface IQueueService
    {
        Task QueueOrderAsync(Order order);
    }
}
