using eventosoutlook.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eventosoutlook.Graph
{

    [ApiController]
    [Route("[controller]")]
    public class CalendarApiController : ControllerBase
    {
        private readonly ILogger<GraphCalendarClient> _logger;
        private readonly GraphServiceClient _graphServiceClient;

        public CalendarApiController(ILogger<GraphCalendarClient> logger, GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            _graphServiceClient = graphServiceClient;
        }

        [HttpGet]
        public async Task<IUserCalendarViewCollectionPage> Get(DateTime start, DateTime end)
        {
            string userTimeZone = "America/Argentina/Buenos_Aires";//TODO get from config file

            _logger.LogInformation($"User timezone: {userTimeZone}");
            // Configure a calendar view for the current week
            //var startOfWeek = DateTime.Now;
            //var endOfWeek = startOfWeek.AddDays(7);

            var viewOptions = new List<QueryOption>
            {
                new QueryOption("startDateTime", start.ToString("yyyy-MM-ddTHH:mm:ss")),
                new QueryOption("endDateTime",  end.ToString("yyyy-MM-ddTHH:mm:ss"))
            };

            try
            {
                // Use the injected GraphServiceClient object to call Me.CalendarView
                var calendarEvents = await _graphServiceClient
                    .Me
                    .CalendarView
                    .Request(viewOptions)
                    .Header("Prefer", $"outlook.timezone=\"{userTimeZone}\"")
                    .Select(evt => new
                    {
                        evt.Subject,
                        evt.Organizer,
                        evt.Start,
                        evt.End
                    })
                    .OrderBy("start/DateTime")
                    .GetAsync();

                return calendarEvents;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calling Graph /me/calendaview: {ex.Message}");
                throw;
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> PostAsync(Evento evento)
        {
            var calendars = await _graphServiceClient.Me.Calendars.Request().GetAsync();
            string timeZone = "America/Argentina/Buenos_Aires";//TODO get from config file
            var @event = new Event
            {
                Subject = evento.Subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = evento.BodyContent
                },
                Start = new DateTimeTimeZone
                {
                    DateTime = evento.Start.ToString("yyyy-MM-ddTHH:mm:ss"),
                    TimeZone = $"{timeZone}"
                },
                End = new DateTimeTimeZone
                {
                    DateTime = evento.End.ToString("yyyy-MM-ddTHH:mm:ss"),
                    TimeZone = $"{timeZone}"
                },
                Location = new Location
                {
                    DisplayName = evento.LocationName
                },
                AllowNewTimeProposals = true,
                TransactionId = Guid.NewGuid().ToString()
            };

            await _graphServiceClient.Me.Events
                .Request()
                .AddAsync(@event);

            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        [HttpPut("{id}")]
        public async Task<HttpResponseMessage> Put(string id, [FromBody] Evento evento)
        {
            string timeZone = "America/Argentina/Buenos_Aires";//TODO get from config file
            var @event = new Event
            {
                Subject = evento.Subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = evento.BodyContent
                },
                Start = new DateTimeTimeZone
                {
                    DateTime = evento.Start.ToString("yyyy-MM-ddTHH:mm:ss"),
                    TimeZone = $"{timeZone}"
                },
                End = new DateTimeTimeZone
                {
                    DateTime = evento.End.ToString("yyyy-MM-ddTHH:mm:ss"),
                    TimeZone = $"{timeZone}"
                },
                Location = new Location
                {
                    DisplayName = evento.LocationName
                },
            };

            await _graphServiceClient.Me.Events[$"{id}"]
                .Request()
                .UpdateAsync(@event);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            string timeZone = "America/Argentina/Buenos_Aires";//TODO get from config file
            await _graphServiceClient.Me.Events[$"{id}"]
                .Request()
                .DeleteAsync();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
