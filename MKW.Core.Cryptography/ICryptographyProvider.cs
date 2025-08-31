namespace MKW.Core.Cryptography
{
    public interface ICryptographyProvider
    {
        IAsymmetricPrivateTransformer CreateAsymmetricTransformer();
        IAsymmetricPublicTransformer OpenAsymmetricTransformer(ReadOnlySpan<byte> publicKey);
        IAsymmetricPublicTransformer OpenAsymmetricTransformer(ReadOnlySpan<byte> publicKey, ReadOnlySpan<byte> privateKey);

        ISymmetricTransformer CreateSymmetricTransformer();
        ISymmetricTransformer OpenSymmetricTransformer(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv);

        UserCredentials CreateUserCredentials(string password);
        UserCredentials OpenUserCredentials(string password, ReadOnlyMemory<byte> salt);
    }
}
