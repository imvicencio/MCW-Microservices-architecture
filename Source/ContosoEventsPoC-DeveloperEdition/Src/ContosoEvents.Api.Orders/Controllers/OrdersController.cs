using System;
using System.Linq;
using System.Threading.Tasks;
using ContosoEvents.Api.Orders.Models;
using ContosoEvents.Api.Orders.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContosoEvents.Api.Orders.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersProvider ordersProvider;
        private readonly IQueueService queueService;
        private readonly ILogger<OrdersController> logger;
        public OrdersController(IOrdersProvider ordersProvider, IQueueService queueService, ILogger<OrdersController> logger)
        {
            this.ordersProvider = ordersProvider;
            this.queueService = queueService;
            this.logger = logger;
        }

        /// <summary>
        ///     Gets all the orders related with the username specified in the parameter
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>A collection of orders</returns>
        [HttpGet("users/{username}")]
        public async Task<IActionResult> GetUserOrdersAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(nameof(username));
            }

            try
            {
                var result = await ordersProvider.GetUserOrdersAsync(username);
                if (result!=null && result.Any())
                {
                    return Ok(result);
                }

                return NotFound(username);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.Message);
                return StatusCode(503);
            }
        }

        /// <summary>
        ///     Places a new order
        /// </summary>
        /// <param name="order">The order</param>
        /// <returns></returns>

        [HttpPost]
        public async Task<IActionResult> PlaceUserOrderAsync(OrderRequest order)
        {
            try
            {
                var newId = Guid.NewGuid().ToString();

                var newOrder = new Order()
                {
                    Id = newId,
                    OrderDate = order.OrderDate,
                    EventId = order.EventId,
                    Email = order.Email,
                    UserName = order.UserName,
                    PaymentProcessorTokenId = order.PaymentProcessorTokenId,
                    Tickets = order.Tickets
                };

                await queueService.QueueOrderAsync(newOrder);

                return Ok(newId);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.Message);
                return StatusCode(503);
            }
        }
    }
}