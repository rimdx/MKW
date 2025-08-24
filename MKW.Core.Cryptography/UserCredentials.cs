using System.Security.Cryptography;

namespace MKW.Core.Cryptography
{
    public class UserCredentials
    {
        private readonly string password;
        private readonly ReadOnlyMemory<byte> salt;

        protected UserCredentials(string password, ReadOnlyMemory<byte> salt)
        {
            this.password = password;
            this.salt = salt;
        }

        public static UserCredentials Create(string password)
        {
            return new UserCredentials(password, GenerateSalt());
        }

        public static UserCredentials Open(string password, ReadOnlyMemory<byte> salt)
        {
            return new UserCredentials(password, salt);
        }

        public Memory<byte> GetSecretKey()
        {
            Memory<byte> password = GetPasswordBytes(this.password);

            return Rfc2898DeriveBytes.Pbkdf2(password.Span,
                                             salt.Span,
                                             CryptographicConstants.DerivePassword.Iterations,
                                             CryptographicConstants.DerivePassword.HashAlgorithm,
                                             CryptographicConstants.DerivePassword.KeySize);
        }

        public ReadOnlyMemory<byte> ExportSalt()
        {
            return salt;
        }

        private static ReadOnlyMemory<byte> GenerateSalt()
        {
            return RandomNumberGenerator.GetBytes(CryptographicConstants.DerivePassword.SaltSize);
        }

        private Memory<byte> GetPasswordBytes(string password)
        {
            return EncodingConverter.GetBytes(password);
        }
    }
}
