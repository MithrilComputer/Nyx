using Nyx.Contracts.Messages;
using Nyx.Core.Diagnostics.Logging;
using Nyx.Runtime.Abstractions.MessageHandlers;
using Nyx.Runtime.Abstractions.MessageRouting;

namespace Nyx.Runtime.MessageRouting
{
    public sealed class WebSocketMessageRouter : IWebSocketMessageRouter, IDisposable
    {
        private readonly Queue<WsMessage> webSocketMessages = new Queue<WsMessage>();

        private readonly Dictionary<string, IMessageHandler> typeHandlerRouter = new Dictionary<string, IMessageHandler>();

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0);

        private readonly CancellationTokenSource cancelToken = new CancellationTokenSource();

        private readonly Task processTask;

        private readonly INyxContextLogger<WebSocketMessageRouter> logger;

        public WebSocketMessageRouter(INyxContextLogger<WebSocketMessageRouter> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            processTask = Task.Run(ProcessLoop);
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
                    logger.LogError(ex);
                    break;
                }

                try
                {
                    WsMessage message;

                    lock(webSocketMessages)
                    {
                        if (webSocketMessages.Count == 0)
                            continue;

                        message = webSocketMessages.Dequeue();
                    }

                    if (typeHandlerRouter.TryGetValue(message.Type.ToLowerInvariant(), out IMessageHandler? handler))
                    {
                        handler?.OnMessageReceive(message.Payload);


                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex);
                }
            }
        }

        public void AddMessageRoute(IMessageHandler handler, string type)
        {
            string lowerType = type.ToLowerInvariant();

            if (typeHandlerRouter.ContainsKey(lowerType))
                throw new ArgumentException("Cant add a message handler to router if there is an existing route for the type.", nameof(handler));

            typeHandlerRouter.Add(lowerType, handler);
        }

        public void Stop()
        {
            if (cancelToken.IsCancellationRequested)
                return;

            cancelToken.Cancel();

            semaphore.Release(); // just to make sure it wakes if its waiting
        }

        public void Dispose()
        {
            Stop();

            try
            {
                processTask.GetAwaiter().GetResult(); // Make sure its finished before nuking it
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
            }

            semaphore.Dispose();
            cancelToken.Dispose();
        }
    }
}
