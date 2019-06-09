using System.Collections.Generic;
using System.Threading.Tasks;
using ContosoEvents.Api.Orders.Models;

namespace ContosoEvents.Api.Orders.Providers
{
    public interface IOrdersProvider
    {
        Task<ICollection<Order>> GetUserOrdersAsync(string username);
    }
}
