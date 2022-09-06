using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Autobarn.Website.Hubs {
    public class AutobarnHub : Hub {
        public async Task ThisMethodNameCanBeAnythingWeLike(string user, string message) {
            await Clients.All.SendAsync("ThisIsAnotherSpecialMagicString", user, message);
        }
    }
}
