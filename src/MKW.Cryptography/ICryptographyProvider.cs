namespace MKW.Cryptography
{
    public interface ICryptographyProvider
    {
        IAsymmetricPrivateTransformer CreateAsymmetricTransformer(AsymmetricAlgorithmConfiguration config);

        IAsymmetricPublicTransformer OpenAsymmetricTransformer(ReadOnlySpan<byte> publicKey,
                                                               AsymmetricAlgorithmConfiguration config);

        IAsymmetricPrivateTransformer OpenAsymmetricTransformer(ReadOnlySpan<byte> publicKey,
                                                                ReadOnlySpan<byte> privateKey,
                                                                AsymmetricAlgorithmConfiguration config);

        SymmetricKey CreateSymmetricKey(SymmetricAlgorithmConfiguration config);

        ISymmetricTransformer OpenSymmetricTransformer(SymmetricKey key);

        IUserCredentials CreateUserCredentials(string password,
                                               PasswordDerivationConfiguration config);

        IUserCredentials OpenUserCredentials(string password,
                                             ReadOnlyMemory<byte> salt,
                                             PasswordDerivationConfiguration config);

        IRandomGenerator CreateRandomGenerator();
    }
}
