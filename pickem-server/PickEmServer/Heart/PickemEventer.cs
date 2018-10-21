using Microsoft.Extensions.Logging;
using PickEmServer.App.Models;
using PickEmServer.WebSockets;
using Newtonsoft.Json;

namespace PickEmServer.Heart
{
    public class PickemEventer
    {
        private readonly ILogger<PickemEventer> _logger;
        private readonly SuperWebSocketPoolManager _webSocketPoolManager;

        public PickemEventer(SuperWebSocketPoolManager webSocketPoolManager, ILogger<PickemEventer> logger)
        {
            _logger = logger;
            _webSocketPoolManager = webSocketPoolManager;
        }

        public async void Emit(PickemSystemEvent pickemSystemEvent)
        {
            var pickemEventJson = JsonConvert.SerializeObject(pickemSystemEvent);
            _logger.LogInformation(pickemEventJson);
            await _webSocketPoolManager.SendToAllSockets(pickemEventJson);
        }
    }
}
