using System.Text.Json.Serialization;

namespace MKW.Core
{
    [JsonSourceGenerationOptions()]
    [JsonSerializable(typeof(UserAccessRequestData))]
    internal sealed partial class UserAccessRequestJsonSerializerContext : JsonSerializerContext
    {
    }
}
