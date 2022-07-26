using eventosoutlook.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

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



        [HttpPost]
        public async Task<HttpResponseMessage> PostAsync(Evento evento)
        {
            var calendars = await _graphServiceClient.Me.Calendars.Request().GetAsync();

			var @event = new Event
			{
				Subject = "Let's go for lunch",
				Body = new ItemBody
				{
					ContentType = BodyType.Html,
					Content = "Does noon work for you?"
				},
				Start = new DateTimeTimeZone
				{
					DateTime = "2017-04-15T12:00:00",
					TimeZone = "Pacific Standard Time"
				},
				End = new DateTimeTimeZone
				{
					DateTime = "2017-04-15T14:00:00",
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
							Address = "samanthab@contoso.onmicrosoft.com",
							Name = "Samantha Booth"
						},
						Type = AttendeeType.Required
					}
				},
				AllowNewTimeProposals = true,
				TransactionId = Guid.NewGuid().ToString()
			};

			await _graphServiceClient.Me.Events
				.Request()
				.Header("Prefer", "outlook.timezone=\"Pacific Standard Time\"")
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
