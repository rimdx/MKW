namespace MKW.Core.Cryptography.BouncyCastle
{
    public class BouncyCastleCryptographyProvider : ICryptographyProvider
    {
        public BouncyCastleCryptographyProvider()
        {
        }

        public IAsymmetricPrivateTransformer CreateAsymmetricTransformer()
        {
            throw new NotImplementedException();
        }

        public ISymmetricTransformer CreateSymmetricTransformer()
        {
            throw new NotImplementedException();
        }

        public IUserCredentials CreateUserCredentials(string password)
        {
            throw new NotImplementedException();
        }

        public IAsymmetricPublicTransformer OpenAsymmetricTransformer(ReadOnlySpan<byte> publicKey)
        {
            throw new NotImplementedException();
        }

        public IAsymmetricPrivateTransformer OpenAsymmetricTransformer(ReadOnlySpan<byte> publicKey, ReadOnlySpan<byte> privateKey)
        {
            throw new NotImplementedException();
        }

        public ISymmetricTransformer OpenSymmetricTransformer(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv)
        {
            throw new NotImplementedException();
        }

        public IUserCredentials OpenUserCredentials(string password, ReadOnlyMemory<byte> salt)
        {
            throw new NotImplementedException();
        }
    }
}
