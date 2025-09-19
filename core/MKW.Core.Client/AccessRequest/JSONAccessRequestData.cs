using System.Text.Json.Serialization;

namespace MKW.Core.Client.AccessRequest
{
    [JsonSerializable(typeof(JSONAccessRequestData))]
    internal class JSONAccessRequestData
    {
        public required ReadOnlyMemory<byte> Salt { get; set; }

        public required ReadOnlyMemory<byte> PublicKey { get; set; }

        public required ReadOnlyMemory<byte> PrivateKey { get; set; }
    }
}
