using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace MKW.Core.Cryptography.BouncyCastle
{
    public class UserCredentials : IUserCredentials, IDisposable
    {
        private readonly ReadOnlyMemory<byte> password;
        private readonly ReadOnlyMemory<byte> salt;
        private Pkcs5S2ParametersGenerator generator;

        protected UserCredentials(ReadOnlyMemory<byte> password, ReadOnlyMemory<byte> salt)
        {
            this.password = password;
            this.salt = salt;

            IDigest digest = DigestUtilities.GetDigest(NistObjectIdentifiers.IdSha256);
            generator = new Pkcs5S2ParametersGenerator(digest);
        }

        public static UserCredentials Create(string password)
        {
            return new UserCredentials(EncodingConverter.GetBytes(password), GenerateSalt());
        }

        public static UserCredentials Open(string password, ReadOnlyMemory<byte> salt)
        {
            return new UserCredentials(EncodingConverter.GetBytes(password), salt);
        }

        public ReadOnlyMemory<byte> ExportSalt()
        {
            return salt.ToArray();
        }

        public Memory<byte> GetSecretKey()
        {
            generator.Init(password.Span, salt.Span, 100_000);

            KeyParameter key = (KeyParameter)generator.GenerateDerivedMacParameters(128);

            return key.GetKey();
        }

        private static ReadOnlyMemory<byte> GenerateSalt()
        {
            SecureRandom random = new SecureRandom();
            return SecureRandom.GetNextBytes(random, 16);
        }

        public void Dispose()
        {
        }
    }
}
