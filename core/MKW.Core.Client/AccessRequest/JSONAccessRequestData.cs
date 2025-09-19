namespace MKW.Core.Client.AccessRequest
{
    internal class JSONAccessRequestData
    {
        public required ReadOnlyMemory<byte> Salt { get; set; }

        public required ReadOnlyMemory<byte> PublicKey { get; set; }

        public required ReadOnlyMemory<byte> PrivateKey { get; set; }
    }
}
