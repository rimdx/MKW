using System.Text.Json.Serialization;

namespace MKW.Core
{
    [JsonSourceGenerationOptions()]
    [JsonSerializable(typeof(UserMetadataJson))]
    internal partial class UserMetadataJsonSerializerContext : JsonSerializerContext
    {
    }
}
