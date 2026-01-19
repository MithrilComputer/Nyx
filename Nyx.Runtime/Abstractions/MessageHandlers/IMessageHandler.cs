using System.Text.Json;

namespace Nyx.Runtime.Abstractions.MessageHandlers
{
    internal interface IMessageHandler
    {

        void OnMessageReceive(JsonElement payload);

    }
}
