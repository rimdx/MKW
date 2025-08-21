using System.Text.Json.Serialization;

namespace MKW.Core.Storage.JSON.Types
{
    public record class JSONDatabaseUser
    {
        protected readonly MemoryDatabaseSession? host;

        [JsonRequired]
        public ReadOnlyMemory<byte> Salt { get; set; }

        [JsonRequired]
        public ReadOnlyMemory<byte> PublicKey { get; set; }

        [JsonRequired]
        public ReadOnlyMemory<byte> PrivateKey { get; set; }
    }
}
