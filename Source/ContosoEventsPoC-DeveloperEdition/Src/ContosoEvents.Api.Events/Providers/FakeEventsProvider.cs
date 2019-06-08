using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoEvents.Api.Events.Models;

namespace ContosoEvents.Api.Events.Providers
{
    public class FakeEventsProvider : IEventsProvider
    {
        private List<Event> repo = new List<Event>();
        public FakeEventsProvider()
        {
            repo.Add(new Event()
            {
                Id = "1",
                Name = "Event 1",
                PricePerTicket = 30,
                StartDate = DateTime.Now.AddDays(10),
                EndDate = DateTime.Now.AddDays(11),
                TotalTickets = 5000,
                Currency = "USD"
            });
            repo.Add(new Event()
            {
                Id = "2",
                Name = "Event 2",
                PricePerTicket = 100,
                StartDate = DateTime.Now.AddDays(10),
                EndDate = DateTime.Now.AddDays(11),
                TotalTickets = 2500,
                Currency = "USD"
            });
            repo.Add(new Event()
            {
                Id = "3",
                Name = "Event 3",
                PricePerTicket = 50,
                StartDate = DateTime.Now.AddDays(10),
                EndDate = DateTime.Now.AddDays(11),
                TotalTickets = 1000,
                Currency = "USD"
            });
        }

        public Task<ICollection<Event>> GetAllEventsAsync()
        {
            return Task.FromResult((ICollection<Event>)repo.ToList());
        }

        public Task<Event> GetEventAsync(string id)
        {
            var result = repo.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(result);
        }
    }
}