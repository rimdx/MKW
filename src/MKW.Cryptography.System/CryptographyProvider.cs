namespace MKW.Cryptography.System
{
    public class CryptographyProvider : IDisposable, ICryptographyProvider
    {
        public CryptographyProvider()
        {
        }

        public ISymmetricTransformer CreateSymmetricTransformer(SymmetricAlgorithmConfiguration config)
        {
            return SymmetricTransformer.Create();
        }

        public ISymmetricTransformer OpenSymmetricTransformer(ReadOnlySpan<byte> key,
                                                              ReadOnlySpan<byte> iv,
                                                              SymmetricAlgorithmConfiguration config)
        {
            return SymmetricTransformer.Open(key, iv);
        }

        public IAsymmetricPrivateTransformer CreateAsymmetricTransformer(AsymmetricAlgorithmConfiguration config)
        {
            return AsymmetricTransformer.Create();
        }

        public IAsymmetricPublicTransformer OpenAsymmetricTransformer(ReadOnlySpan<byte> publicKey,
                                                                      AsymmetricAlgorithmConfiguration config)
        {
            return AsymmetricTransformer.Open(publicKey);
        }

        public IAsymmetricPrivateTransformer OpenAsymmetricTransformer(ReadOnlySpan<byte> publicKey,
                                                                       ReadOnlySpan<byte> privateKey,
                                                                       AsymmetricAlgorithmConfiguration config)
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
