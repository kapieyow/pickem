using System;
using System.ComponentModel;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PickEmServer.WebSockets
{
    internal class SuperWebSocketWrapper
    {
        // socket received data
        internal delegate Task ReceivedDataDelegate(SuperWebSocketWrapper socketThatReceivedData, string data);
        internal ReceivedDataDelegate ReceivedData;

        internal int SocketId { get; private set; }

        private WebSocket _nativeWebSocket;

        internal SuperWebSocketWrapper(WebSocket nativeWebSocket, int socketId, ReceivedDataDelegate receivedDataCallback)
        {
            _nativeWebSocket = nativeWebSocket;
            this.SocketId = socketId;
            this.ReceivedData = receivedDataCallback;
        }

        internal async Task<string> Run()
        {
            SuperWebSocketReadResult receivedResult;

            // This is the main "run" method for this wrapped socket. At this point 
            // the socket is open, wrapped and accepted. This waits aynch for a
            // message from the client. 
            do
            {
                // wait for client to send data
                receivedResult = await WaitReadStringAsync();

                // did something poo?
                if (receivedResult.SocketClosed)
                {
                    return receivedResult.SocketCloseMessage;
                }
                else
                {
                    // no poo, send message to other sockets. This goes out the delegate to the pool manager
                    await this.ReceivedData(this, receivedResult.Data);
                }

            } while (true);
        }

        internal async Task SendData(string data)
        {
            // send data through this socket to the other end
            await WriteStringAsync(data);
        }

        internal async Task<SuperWebSocketReadResult> WaitReadStringAsync()
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);

            using (var memStream = new MemoryStream())
            {
                WebSocketReceiveResult asyncResult;

                do
                {
                    try
                    {
                        asyncResult = await _nativeWebSocket.ReceiveAsync(buffer, CancellationToken.None);
                    }
                    catch (Exception e)
                    {
                        return new SuperWebSocketReadResult(
                            null,
                            true,
                            string.Format("Socket ReceiveAsync threw exception ({0})", e.Message));
                    }

                    if (asyncResult.CloseStatus.HasValue)
                    {
                        return new SuperWebSocketReadResult(
                            null,
                            true,
                            string.Format("Socket ReceiveAsync has a CloseStatus of ({0})", asyncResult.CloseStatus));
                    }
                    memStream.Write(buffer.Array, buffer.Offset, asyncResult.Count);
                }
                while (!asyncResult.EndOfMessage);

                memStream.Seek(0, SeekOrigin.Begin);

                switch (asyncResult.MessageType)
                {
                    case WebSocketMessageType.Close:
                        return new SuperWebSocketReadResult(
                            null,
                            true,
                            string.Format("Socket ReceiveAsync result: Closed. Due to ({0})", asyncResult.CloseStatus));

                    case WebSocketMessageType.Text:
                        using (var reader = new StreamReader(memStream, Encoding.UTF8))
                        {
                            string returnText = await reader.ReadToEndAsync();
                            return new SuperWebSocketReadResult(
                                returnText,
                                false,
                                null);
                        }

                    default:
                        throw new InvalidEnumArgumentException(string.Format("MessageType (in WebSocket.ReceiveAsync result) unexpected type ({0})", asyncResult.MessageType));
                }
            }
        }

        internal async Task WriteStringAsync(string data)
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            var segment = new ArraySegment<byte>(buffer);

            await _nativeWebSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);

            return;
        }
    }
}
