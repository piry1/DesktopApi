using Newtonsoft.Json;
using Ninja.WebSockets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopApi.Server.WebServer
{
    public class WebSocketServer
    {
        private TcpListener _serverSocket;
        private Thread _responseThread;
        private readonly List<Task> _recivingTasks = new List<Task>();
        private readonly Router _router = new Router();
        private int _port;

        public Thread Start(int port = 5000)
        {
            _port = port;
            _serverSocket = new TcpListener(IPAddress.Any, _port);
            _serverSocket.Start();
            _responseThread = new Thread(ResponseThread);
            _responseThread.Start();
            Console.WriteLine($"Websocket server started at {_port}");
            return _responseThread;
        }

        public void Stop()
        {
            foreach (var task in _recivingTasks)
                task.Dispose();
            _recivingTasks.Clear();
            _responseThread.Abort();
        }

        private void ResponseThread()
        {
            while (true)
            {
                var tcpClient = _serverSocket.AcceptTcpClient();
                var stream = tcpClient.GetStream();
                var factory = new WebSocketServerFactory();
                var context = factory.ReadHttpHeaderFromStreamAsync(stream);
                context.Wait();
                if (!context.Result.IsWebSocketRequest) continue;
                var webSocket = factory.AcceptWebSocketAsync(context.Result);
                var task = Task.Run(() => Receive(webSocket.Result));
                _recivingTasks.Add(task);
                Console.WriteLine($"Connected new socket. Socket count: {_recivingTasks.Count}");
            }
        }

        private async void ProcessRequest(WebSocket webSocket, string jsonString)
        {
            var request = JsonConvert.DeserializeObject<Request>(jsonString);
            var response = _router.RouteApiMethod(request);
            var socketResponse = new SocketResponse
            {
                Controller = request.Controller,
                Method = request.Method,
                Response = response
            };

            await Send(webSocket, JsonConvert.SerializeObject(socketResponse));
        }

        private async Task Receive(WebSocket webSocket)
        {
            var buffer = new ArraySegment<byte>(new byte[1024]);
            while (true)
            {
                var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                switch (result.MessageType)
                {
                    case WebSocketMessageType.Close:
                        return;
                    case WebSocketMessageType.Text:
                    case WebSocketMessageType.Binary:
                        ProcessRequest(webSocket, buffer, result);
                        break;
                }
            }
        }

        private void ProcessRequest(WebSocket webSocket, ArraySegment<byte> buffer, WebSocketReceiveResult result)
        {
            var bytes = buffer.Array;
            if (bytes == null) return;
            var value = Encoding.UTF8.GetString(bytes, 0, result.Count);
            ProcessRequest(webSocket, value);
        }

        private static async Task Send(WebSocket webSocket, string message)
        {
            var array = Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<byte>(array);
            await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
