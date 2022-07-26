using Microsoft.AspNetCore.Mvc;

namespace eventos_outlook.Controllers
{
    public class Evento { 

    }
    [ApiController]
    [Route("[controller]")]
    public class EventosController : ControllerBase
    {


        private readonly ILogger<EventosController> _logger;

        public EventosController(ILogger<EventosController> logger)
        {
            _logger = logger;
        }

        public IEnumerable<Evento> Get()
        {
            return new List<Evento>();
        }
    }
}