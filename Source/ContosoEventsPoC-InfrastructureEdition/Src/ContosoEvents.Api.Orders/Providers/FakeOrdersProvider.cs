using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoEvents.Api.Orders.Models;

namespace ContosoEvents.Api.Orders.Providers
{
    public class FakeOrdersProvider : IOrdersProvider
    {
        private readonly List<Order> repo = new List<Order>();
        public FakeOrdersProvider()
        {
            repo.Add(new Order()
            {
                Id = Guid.NewGuid().ToString(),
                EventId = "1",
                OrderDate = DateTime.Now,
                Tickets = 1,
                Price = 40,
                Email = "test@test.com",
                UserName = "user1"
            });
            repo.Add(new Order()
            {
                Id = Guid.NewGuid().ToString(),
                EventId = "1",
                OrderDate = DateTime.Now,
                Tickets = 1,
                Price = 40,
                Email = "test@test.com",
                UserName = "user2"
            });
            repo.Add(new Order()
            {
                Id = Guid.NewGuid().ToString(),
                EventId = "2",
                OrderDate = DateTime.Now,
                Tickets = 1,
                Price = 40,
                Email = "test@test.com",
                UserName = "user3"
            });
        }

        public Task<ICollection<Order>> GetUserOrdersAsync(string username)
        {
            return Task.FromResult((ICollection<Order>)repo.Where(o => o.UserName == username).ToList());
        }
    }
}
