using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace PickEmServer.WebSockets
{
    public class SuperWebSocketPoolManager
    {
        private static ConcurrentDictionary<int, SuperWebSocketWrapper> _wrappedSockets = new ConcurrentDictionary<int, SuperWebSocketWrapper>();
        private int _nextSocketId = -1;
        private readonly ILogger<SuperWebSocketPoolManager> _logger;

        public SuperWebSocketPoolManager(ILogger<SuperWebSocketPoolManager> logger)
        {
            _logger = logger;
        }

        // takes and accepted native socket and "runs" it within 
        // the pool of sockets this manager maintains. 
        internal async Task RunSocket(WebSocket nativeWebSocket)
        {
            // build wrapper for this new socket
            var webSocketWrapper = new SuperWebSocketWrapper(
                nativeWebSocket,
                this.GetNextSocketId(),
                this.SocketReceivedData);

            AddWebSocketWrapperToPool(webSocketWrapper);

            string socketCloseReason = null;

            try
            {
                _logger.LogInformation("Added socket ({0}) to pool, running", webSocketWrapper.SocketId);
                socketCloseReason = await webSocketWrapper.Run();
            }
            finally
            {
                _logger.LogInformation("Removing Socket ({0}) from pool. Close reason [{1}]", webSocketWrapper.SocketId, socketCloseReason);
                RemoveWebSocketWrapperFromPool(webSocketWrapper);
            }
        }

        private async Task SendToAllSockets(SuperWebSocketWrapper socketThatReceivedData, string message)
        {
            foreach (var wrappedSocket in _wrappedSockets.Values)
            {
                // do not echo back to the socket that sent this
                if (socketThatReceivedData != null && socketThatReceivedData.SocketId == wrappedSocket.SocketId)
                {
                    _logger.LogInformation("Suppressing echo to source socket ({1})", wrappedSocket.SocketId);
                }
                else
                {
                    try
                    {
                        _logger.LogInformation("Sending ({0}) to Socket ({1})", message, wrappedSocket.SocketId);
                        await wrappedSocket.SendData(message);
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation(e, "Send data to socket ({0}) failed. Cleaning up socket.", wrappedSocket.SocketId);
                        // TODO: what does this do to the iterator?
                        RemoveWebSocketWrapperFromPool(wrappedSocket);
                    }
                }
            }
        }

        internal async Task SendToAllSockets(string message)
        {
            await SendToAllSockets(null, message);
        }

        private int GetNextSocketId()
        {
            _nextSocketId++;
            return _nextSocketId;
        }

        private void AddWebSocketWrapperToPool(SuperWebSocketWrapper webSocketWrapper)
        {
            _wrappedSockets.TryAdd(webSocketWrapper.SocketId, webSocketWrapper);
        }

        private void RemoveWebSocketWrapperFromPool(SuperWebSocketWrapper webSocketWrapper)
        {
            SuperWebSocketWrapper removedWebSocketWrapper;
            _wrappedSockets.TryRemove(webSocketWrapper.SocketId, out removedWebSocketWrapper);
        }

        private async Task SocketReceivedData(SuperWebSocketWrapper socketThatReceivedData, string data)
        {
            // sending source socket to suppress echos
            await SendToAllSockets(socketThatReceivedData, data);
        }
    }
}
