using System.Collections.Generic;
using System.Threading.Tasks;
using ContosoEvents.Api.Events.Models;

namespace ContosoEvents.Api.Events.Providers
{
    public interface IEventsProvider
    {
        Task<ICollection<Event>> GetAllEventsAsync();

        Task<Event> GetEventAsync(string id);
    }
}
