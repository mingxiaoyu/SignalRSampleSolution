using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRJS
{
    public class CountHub : Hub
    {
        private readonly CountService _countService;
        public CountHub(CountService countService)
        {
            _countService = countService;
        }

        public async Task GetLatestCount(string random)
        {
            int count;
            do
            {
                count = _countService.GetLastedCount();
                Thread.Sleep(1000);

                await Clients.All.SendAsync("ReceiveUpdate",count);

            } while (count < 10);

            await Clients.All.SendAsync("Finished");
        }

        public override async Task OnConnectedAsync()
        {
            //return base.OnConnectedAsync();
            var connectionId = Context.ConnectionId;
            await Clients.Client(connectionId).SendAsync("someFunc", new { });
            await Clients.AllExcept(connectionId).SendAsync("someFunc");

            await Groups.AddToGroupAsync(connectionId, "MyGroup");
            await Groups.RemoveFromGroupAsync(connectionId, "MyGroup");

            await Clients.Groups("MyGroup").SendAsync("someFunc");
        }
    }
}
