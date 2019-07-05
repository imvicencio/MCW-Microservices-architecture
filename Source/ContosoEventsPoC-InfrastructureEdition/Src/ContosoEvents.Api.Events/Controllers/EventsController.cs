using System;
using System.Linq;
using System.Threading.Tasks;
using ContosoEvents.Api.Events.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContosoEvents.Api.Events.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventsProvider eventsProvider;
        private readonly ILogger<EventsController> logger;
        public EventsController(IEventsProvider eventsProvider, ILogger<EventsController> logger)
        {
            this.eventsProvider = eventsProvider;
            this.logger = logger;
        }

        /// <summary>
        ///     Get all the events
        /// </summary>
        /// <returns>A collection with all the events</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllEventsAsync()
        {
            try
            {
                var result = await eventsProvider.GetAllEventsAsync();
                if (result!=null && result.Any())
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.Message);
                return StatusCode(503);
            }
        }

        /// <summary>
        /// Get the event with the id specified in the parameter
        /// </summary>
        /// <param name="id">The event unique identifier</param>
        /// <returns>The event</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(id);
            }

            try
            {
                var result = await eventsProvider.GetEventAsync(id);
                if (result != null)
                {
                    return Ok(result);
                }

                return NotFound(id);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.Message);
                return StatusCode(503);
            }
        }
    }
}
