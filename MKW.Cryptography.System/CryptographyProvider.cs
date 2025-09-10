namespace MKW.Cryptography.System
{
    public class CryptographyProvider : IDisposable, ICryptographyProvider
    {
        public CryptographyProvider()
        {
        }

        public ISymmetricTransformer CreateSymmetricTransformer()
        {
            return SymmetricTransformer.Create();
        }

        public ISymmetricTransformer OpenSymmetricTransformer(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv)
        {
            return SymmetricTransformer.Open(key, iv);
        }

        public IAsymmetricPrivateTransformer CreateAsymmetricTransformer()
        {
            return AsymmetricTransformer.Create();
        }

        public IAsymmetricPublicTransformer OpenAsymmetricTransformer(ReadOnlySpan<byte> publicKey)
        {
            return AsymmetricTransformer.Open(publicKey);
        }

        public IAsymmetricPrivateTransformer OpenAsymmetricTransformer(ReadOnlySpan<byte> publicKey, ReadOnlySpan<byte> privateKey)
        {
            return AsymmetricTransformer.Open(publicKey, privateKey);
        }

        public IUserCredentials CreateUserCredentials(string password)
        {
            return UserCredentials.Create(password);
        }

        public IUserCredentials OpenUserCredentials(string password, ReadOnlyMemory<byte> salt)
        {
            return UserCredentials.Open(password, salt);
        }

        public IRandomGenerator CreateRandomGenerator()
        {
            return new SystemRandomGenerator();
        }

        public void Dispose()
        {
        }
    }
}
