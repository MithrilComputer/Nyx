using Nyx.Contracts.Messages;
using Nyx.Core.Diagnostics.Logging;
using Nyx.Runtime.Abstractions.MessageHandlers;
using Nyx.Runtime.Abstractions.MessageRouting;

namespace Nyx.Runtime.MessageRouting
{
    public class WebSocketMessageRouter : IWebSocketMessageRouter
    {
        Queue<WsMessage> webSocketMessages = new Queue<WsMessage>();

        private readonly Dictionary<string, IMessageHandler> typeHandlerRouter = new Dictionary<string, IMessageHandler>();

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0);

        private readonly CancellationTokenSource cancelToken = new CancellationTokenSource();

        private readonly INyxLogger logger;

        public WebSocketMessageRouter(INyxLogger logger)
        {
            this.logger = logger;
        }

        public void AddMessageToQueue(WsMessage message)
        {
            if(string.IsNullOrEmpty(message.Type))
            {
                throw new ArgumentNullException(nameof(message));
            }

            lock (webSocketMessages) webSocketMessages.Enqueue(message);

            semaphore.Release();
        }

        private async Task ProcessLoop()
        {
            while (!cancelToken.IsCancellationRequested)
            {
                try
                {
                    await semaphore.WaitAsync(cancelToken.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError("[WebSocketMessageRouter]", ex);
                    break;
                }

                try
                {
                    WsMessage message;

                    lock(webSocketMessages)
                    {
                        if (webSocketMessages.Count <= 0)
                            continue;

                        message = webSocketMessages.Dequeue();
                    }

                    if (typeHandlerRouter.TryGetValue(message.Type, out IMessageHandler handler))
                    {
                        handler.OnMessageReceive(message.Payload);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("[WebSocketMessageRouter]", ex);
                }
            }
        }
    }
}
