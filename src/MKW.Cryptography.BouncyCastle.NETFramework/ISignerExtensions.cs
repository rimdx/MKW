using Org.BouncyCastle.Crypto;
using System.Buffers;

namespace MKW.Cryptography.BouncyCastle
{
    public static class ISignerExtensions
    {
        public static void BlockUpdate(this ISigner signer, ReadOnlySpan<byte> input)
        {
            byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(input.Length);

            try
            {
                input.CopyTo(sharedBuffer);
                signer.BlockUpdate(sharedBuffer, 0, input.Length);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }
    }
}
