using System.Text.Json.Serialization;

namespace MKW.Core.Storage.JSON.Types
{
    public record class JSONDatabaseSecretEntry
    {
        [JsonRequired]
        public IDictionary<Guid, ReadOnlyMemory<byte>> Keys { get; set; }

        [JsonRequired]
        public ReadOnlyMemory<byte> Salt { get; set; }

        [JsonRequired]
        public ReadOnlyMemory<byte> Data { get; set; }
    }
}
