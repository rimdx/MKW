using MKW.Cryptography;

namespace MKW.Cryptography
{
    public interface ICryptographyProvider
    {
        IAsymmetricPrivateTransformer CreateAsymmetricTransformer();
        IAsymmetricPublicTransformer OpenAsymmetricTransformer(ReadOnlySpan<byte> publicKey);
        IAsymmetricPrivateTransformer OpenAsymmetricTransformer(ReadOnlySpan<byte> publicKey, ReadOnlySpan<byte> privateKey);

        ISymmetricTransformer CreateSymmetricTransformer();
        ISymmetricTransformer OpenSymmetricTransformer(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv);

        IUserCredentials CreateUserCredentials(string password);
        IUserCredentials OpenUserCredentials(string password, ReadOnlyMemory<byte> salt);

        IRandomGenerator CreateRandomGenerator();
    }
}
