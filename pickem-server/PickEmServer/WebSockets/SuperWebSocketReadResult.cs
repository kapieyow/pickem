
namespace PickEmServer.WebSockets
{
    public class SuperWebSocketReadResult
    {
        public SuperWebSocketReadResult(string data, bool socketClosed, string socketCloseMessage)
        {
            Data = data;
            SocketClosed = socketClosed;
            SocketCloseMessage = socketCloseMessage;
        }

        public string Data { get; protected set; }
        public bool SocketClosed { get; protected set; }
        public string SocketCloseMessage { get; protected set; }
    }
}
