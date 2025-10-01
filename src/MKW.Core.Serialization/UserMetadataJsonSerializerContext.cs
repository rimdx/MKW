using System.Text.Json.Serialization;

namespace MKW.Core.Serialization
{
    [JsonSourceGenerationOptions()]
    [JsonSerializable(typeof(UserMetadataJson))]
    internal sealed partial class UserMetadataJsonSerializerContext : JsonSerializerContext
    {
    }
}
