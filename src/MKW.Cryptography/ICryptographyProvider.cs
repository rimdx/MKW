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

        ISymmetricTransformer CreateSymmetricTransformer(SymmetricAlgorithmConfiguration config);

        ISymmetricTransformer OpenSymmetricTransformer(ReadOnlySpan<byte> key,
                                                       ReadOnlySpan<byte> iv,
                                                       SymmetricAlgorithmConfiguration config);

        IUserCredentials CreateUserCredentials(string password);
        IUserCredentials OpenUserCredentials(string password, ReadOnlyMemory<byte> salt);

        IRandomGenerator CreateRandomGenerator();
    }
}
