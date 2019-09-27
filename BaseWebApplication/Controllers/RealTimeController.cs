using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

// From https://www.cnblogs.com/cgzl/p/9509207.html

namespace BaseWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RealTimeController : ControllerBase
    {
        protected readonly MyService _myService;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        public RealTimeController(MyService myService, IHttpContextAccessor httpContextAccessor)
        {
            _myService = myService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("polling/{id}")]
        public IActionResult GetCountPolling(int id)
        {
            var count = _myService.GetLastedCount();
            if (count > 10)
            {
                return Ok(new { id, count, finished = true });
            }
            if (count > 6)
            {
                return Ok(new { id, count });
            }
            return NoContent();
        }

        [HttpGet("longpolling/{id}")]
        public IActionResult GetCountLongPolling(int id)
        {
            int count;
            do
            {
                count = _myService.GetLastedCount();
                Thread.Sleep(1000);
            } while (count < 5);
            return Ok(new { id, count, finished = true });
        }

        [HttpGet("sse/{id}")]
        public async void GetCountSSE(int id)
        {
            Response.ContentType = "text/event-stream";
            int count;
            do
            {
                count = _myService.GetLastedCount();
                Thread.Sleep(1000);
                if (count % 3 == 0)
                {
                    await HttpContext.Response.WriteAsync($"data:{count}\n\n");
                    await HttpContext.Response.Body.FlushAsync();
                }

            } while (count < 10);

            Response.Body.Close();
        }

        [HttpGet("ws/{id}")]
        public async void GetCountWebSocket(int id)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await SendEvents(webSocket, id);
                await webSocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure,
                    "Done", CancellationToken.None);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        private async Task SendEvents(WebSocket webSocket, int id)
        {
            int count;
            do
            {
                count = _myService.GetLastedCount();
                Thread.Sleep(2000);

                if (count % 3 != 0) continue;

                var obj = new { id, count };
                var jsonStr = JsonConvert.SerializeObject(obj);

                await webSocket.SendAsync(buffer: new ArraySegment<byte>(
                    array: Encoding.ASCII.GetBytes(jsonStr),
                    offset: 0,
                    count: jsonStr.Length
                    ),
                    messageType: WebSocketMessageType.Text,
                    endOfMessage: true,
                    cancellationToken: CancellationToken.None);

            } while (count < 10);
        }

        [HttpPost("reset")]
        public void Reset()
        {
            _myService.Reset();
        }
    }
}
