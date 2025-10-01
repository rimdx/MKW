using System.Text.Json.Serialization;

namespace MKW.Core.Serialization
{
    [JsonSourceGenerationOptions()]
    [JsonSerializable(typeof(UserAccessRequestData))]
    internal sealed partial class UserAccessRequestJsonSerializerContext : JsonSerializerContext
    {
    }
}
