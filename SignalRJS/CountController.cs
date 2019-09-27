using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SignalRJS
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountController : Controller
    {
        private readonly IHubContext<CountHub> _hubContext;

        public CountController(IHubContext<CountHub> hubContext)
        {
            _hubContext = hubContext;
        }
     

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            await _hubContext.Clients.All.SendAsync("someFunc", new { random ="abcd" });
            return Accepted(1);
        }
 
    }
}
