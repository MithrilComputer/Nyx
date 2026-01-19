using System.Net.WebSockets;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

app.UseWebSockets();

app.Map("/ws", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = 400;
        return;
    }

    using var socket = await context.WebSockets.AcceptWebSocketAsync();
    Console.WriteLine("Client connected");

    var buffer = new byte[4096];
    var messageBuilder = new StringBuilder();

    while (socket.State == WebSocketState.Open)
    {
        var result = await socket.ReceiveAsync(
            new ArraySegment<byte>(buffer),
            CancellationToken.None
        );

        if (result.MessageType == WebSocketMessageType.Close)
        {
            Console.WriteLine("Client disconnected");
            await socket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Bye",
                CancellationToken.None
            );
            break;
        }

        messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

        if (result.EndOfMessage)
        {
            var readme = messageBuilder.ToString();
            messageBuilder.Clear();

            Console.WriteLine("=== README RECEIVED ===");
            Console.WriteLine(readme);
            Console.WriteLine("=======================");
        }
    }
});

app.Run("http://127.0.0.1:8080");
