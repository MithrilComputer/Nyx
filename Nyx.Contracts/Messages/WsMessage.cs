using System.Text.Json;

namespace Nyx.Contracts.Messages
{
    public class WsMessage
    {
        public string Type { get; set; }
        public JsonElement Payload { get; set; }
    }
}
