using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace PickEmServer.WebSockets
{
    public class SuperWebSocketMiddleware
    {
        private readonly ILogger<SuperWebSocketMiddleware> _logger;
        private readonly RequestDelegate _nextMiddleware;
        private readonly SuperWebSocketPoolManager _webSocketPoolManager;

        public SuperWebSocketMiddleware(RequestDelegate nextMiddleware, SuperWebSocketPoolManager webSocketManager, ILogger<SuperWebSocketMiddleware> logger)
        {
            _logger = logger;
            _nextMiddleware = nextMiddleware;
            _webSocketPoolManager = webSocketManager;
        }

        // This is the call asp net core calls during the request pipeline
        public async Task Invoke(HttpContext httpContext)
        {

            // This will receive all http requests, only care about the web sockets, else hand off to next middleware
            if (!httpContext.WebSockets.IsWebSocketRequest)
            {
                await _nextMiddleware.Invoke(httpContext);
                return;
            }

            var nativeSocket = await httpContext.WebSockets.AcceptWebSocketAsync();

            // This will setup a socket wrapper for this request and "run" it, which means
            // it will stay open and read/write data and all management within the socket manager and below
            // when control returns here this socket has been closed.
            await _webSocketPoolManager.RunSocket(nativeSocket);

        }
    }
}
