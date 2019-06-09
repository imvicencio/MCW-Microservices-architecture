using System;
using Newtonsoft.Json;

namespace ContosoEvents.Api.Events.Models
{
    public class Event
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalTickets { get; set; }
        public double PricePerTicket { get; set; }
        public string Currency { get; set; }
        public string PaymentProcessorUrl { get; set; }
        public string PaymentProcessorAccount { get; set; }
        public string PaymentProcessorPassword { get; set; }
        public string SuccessEmailTemplate { get; set; }
        public string SuccessSmsTemplate { get; set; }
        public string FailedEmailTemplate { get; set; }
        public string FailedSmsTemplate { get; set; }
    }
}
