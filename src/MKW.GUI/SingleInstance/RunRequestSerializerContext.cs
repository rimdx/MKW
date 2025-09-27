using System.Text.Json.Serialization;

namespace MKW.GUI.SingleInstance
{
    [JsonSourceGenerationOptions()]
    [JsonSerializable(typeof(RunRequest))]
    internal partial class RunRequestSerializerContext : JsonSerializerContext
    {
    }
}
