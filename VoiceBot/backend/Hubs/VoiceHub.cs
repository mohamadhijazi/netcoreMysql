using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace VoiceBot.Hubs
{
    public class VoiceHub : Hub
    {
        // Called by client to send audio data
        public async Task SendAudio(byte[] audioData)
        {
            // Broadcast to all clients except sender
            await Clients.Others.SendAsync("ReceiveAudio", audioData);
        }

        // Called by client to send text message
        public async Task SendText(string message)
        {
            await Clients.Others.SendAsync("ReceiveText", message);
        }
    }
}
