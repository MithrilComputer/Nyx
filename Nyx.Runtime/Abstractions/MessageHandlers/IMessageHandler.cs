using System.Text.Json;

namespace Nyx.Runtime.Abstractions.MessageHandlers
{
    public interface IMessageHandler
    {
        void OnMessageReceive(JsonElement payload);
    }
}
