namespace EF_Core.Middlewares
{
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;

    public class WebSocketHandler
    {
        private readonly RequestDelegate _next;
        private static Dictionary<string, WebSocket> Customers = new();
        private static WebSocket? Receptionist = null; // only one receptionist

        public WebSocketHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    string role = context.Request.Query["role"];
                    string id = Guid.NewGuid().ToString();

                    if (role == "customer") Customers[id] = webSocket;
                    else if (role == "receptionist") Receptionist = webSocket;

                    await Listen(id, role, webSocket);

                    // Cleanup
                    if (role == "customer") Customers.Remove(id);
                    else if (role == "receptionist") Receptionist = null;
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                await _next(context);
            }
        }

        private async Task Listen(string id, string role, WebSocket socket)
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close) break;

                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    if (role == "customer")
                    {
                        if (Receptionist != null && Receptionist.State == WebSocketState.Open)
                        {
                            // Send customer message → receptionist
                            var jsonMsg = $"{{\"customerId\":\"{id}\",\"message\":\"{message}\"}}";
                            await Receptionist.SendAsync(Encoding.UTF8.GetBytes(jsonMsg),
                                WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        else
                        {
                            var jsonMsg = "{\"message\": \"Receptionist not available right now, come back soon!\"}";
                            await Customers[id].SendAsync(Encoding.UTF8.GetBytes(jsonMsg),
                                WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                    else if (role == "receptionist")
                    {
                        // Receptionist message contains target customerId
                        var json = JsonDocument.Parse(message).RootElement;
                        string customerId = json.GetProperty("customerId").GetString()!;
                        string reply = json.GetProperty("message").GetString()!;
                        //string reply = Encoding.UTF8.GetString(buffer, 0, result.Count);

                        if (Customers.ContainsKey(customerId) && Customers[customerId].State == WebSocketState.Open)
                        {
                            var cust = Customers[customerId];
                            var jsonMsg = $"{{\"customerId\":\"{customerId}\",\"message\":\"{reply}\"}}";
                            await cust.SendAsync(Encoding.UTF8.GetBytes(reply),
                                WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");
            }
            finally
            {
                if (socket.State != WebSocketState.Closed)
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                socket.Dispose();
            }
        }
    }

    public static class WebSocketHandlerExtensions
    {
        public static IApplicationBuilder UseWebSocketHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebSocketHandler>();
        }
    }

}
