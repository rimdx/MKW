namespace MKW.Cryptography
{
    public interface ICryptographyProvider
    {
        AsymmetricPrivateKey CreateAsymmetricKey(AsymmetricAlgorithmConfiguration config);

        AsymmetricPublicKey DecodePkcsPublicKey(ReadOnlySpan<byte> data);
        AsymmetricPrivateKey DecodePkcsPrivateKey(ReadOnlySpan<byte> data);

        Memory<byte> EncodePkcsPublicKey(AsymmetricPublicKey key);
        Memory<byte> EncodePkcsPrivateKey(AsymmetricPrivateKey key);

        IAsymmetricPublicTransformer OpenAsymmetricTransformer(AsymmetricPublicKey publicKey,
                                                               AsymmetricAlgorithmConfiguration config);

        IAsymmetricPrivateTransformer OpenAsymmetricTransformer(AsymmetricPrivateKey privateKey,
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
