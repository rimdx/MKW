namespace MKW.Core
{
    internal class UserAccessRequestData
    {
        public required ReadOnlyMemory<byte> Salt { get; set; }

        public required ReadOnlyMemory<byte> PublicKey { get; set; }

        public required ReadOnlyMemory<byte> PrivateKey { get; set; }

        public required ReadOnlyMemory<byte> AdminSignature { get; set; }
    }
}
