using eventosoutlook.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eventosoutlook.Graph
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ILogger<GraphCalendarClient> _logger;
        private readonly GraphServiceClient _graphServiceClient;

        public CalendarController(ILogger<GraphCalendarClient> logger, GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            _graphServiceClient = graphServiceClient;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }


        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public async Task<HttpResponseMessage> PostAsync(Evento evento)
        {
            var @event = new Event
            {
                Subject = "Let's go for lunch",
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = "Does mid month work for you?"
                },
                Start = new DateTimeTimeZone
                {
                    DateTime = "2019-03-15T12:00:00",
                    TimeZone = "Pacific Standard Time"
                },
                End = new DateTimeTimeZone
                {
                    DateTime = "2019-03-15T14:00:00",
                    TimeZone = "Pacific Standard Time"
                },
                Location = new Location
                {
                    DisplayName = "Harry's Bar"
                },
                Attendees = new List<Attendee>()
                {
                    new Attendee
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = "adelev@contoso.onmicrosoft.com",
                            Name = "Adele Vance"
                        },
                        Type = AttendeeType.Required
                    }
                },
                TransactionId = "7E163156-7762-4BEB-A1C6-729EA81755A7"
            };

            await _graphServiceClient.Me.Calendars["{calendar-id}"].Events
                .Request()
                .AddAsync(@event);
            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
