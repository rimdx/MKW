using System.Text.Json.Serialization;

namespace MKW.Core
{
    [JsonSourceGenerationOptions()]
    [JsonSerializable(typeof(UserAccessRequestData))]
    public partial class UserAccessRequestJsonSerializerContext : JsonSerializerContext
    {
    }
}
