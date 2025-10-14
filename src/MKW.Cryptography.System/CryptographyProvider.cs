namespace MKW.Cryptography.System
{
    public class CryptographyProvider : IDisposable, ICryptographyProvider
    {
        public CryptographyProvider()
        {
        }

        public SymmetricKey CreateSymmetricKey(SymmetricAlgorithmConfiguration config)
        {
            return SymmetricTransformer.CreateKey();
        }

        public ISymmetricTransformer OpenSymmetricTransformer(SymmetricKey key)
        {
            return SymmetricTransformer.Open(key);
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

        public IUserCredentials CreateUserCredentials(string password,
                                                      PasswordDerivationConfiguration config)
        {
            return UserCredentials.Create(password);
        }

        public IUserCredentials OpenUserCredentials(string password,
                                                    ReadOnlyMemory<byte> salt,
                                                    PasswordDerivationConfiguration config)
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
