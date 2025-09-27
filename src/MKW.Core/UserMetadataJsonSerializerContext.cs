using System.Text.Json.Serialization;

namespace MKW.Core
{
    [JsonSourceGenerationOptions()]
    [JsonSerializable(typeof(UserMetadataJson))]
    internal sealed partial class UserMetadataJsonSerializerContext : JsonSerializerContext
    {
    }
}
