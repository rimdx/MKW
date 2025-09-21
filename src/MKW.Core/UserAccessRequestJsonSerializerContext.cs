using System.Text.Json.Serialization;

namespace MKW.Core
{
    [JsonSourceGenerationOptions()]
    [JsonSerializable(typeof(UserAccessRequestData))]
    internal partial class UserAccessRequestJsonSerializerContext : JsonSerializerContext
    {
    }
}
