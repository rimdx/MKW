using System.Security.Cryptography;
using System.Text;

namespace MKW.Core.Cryptography
{
    public class UserCredentials
    {
        private readonly string password;
        private readonly byte[] salt;

        protected UserCredentials(string password, byte[] salt)
        {
            this.password = password;
            this.salt = salt;
        }

        public static UserCredentials Create(string password)
        {
            return new UserCredentials(password, GenerateSalt());
        }

        public static UserCredentials Open(string password, byte[] salt)
        {
            return new UserCredentials(password, salt);
        }

        public byte[] GetEncodingHash()
        {
            return Rfc2898DeriveBytes.Pbkdf2(GetPasswordBytes(password),
                                             salt,
                                             CryptographicConstants.DerivePassword.Iterations,
                                             CryptographicConstants.DerivePassword.HashAlgorithm,
                                             CryptographicConstants.DerivePassword.KeySize);
        }

        public byte[] ExportSalt()
        {
            return salt;
        }

        private static byte[] GenerateSalt()
        {
            return RandomNumberGenerator.GetBytes(CryptographicConstants.DerivePassword.SaltSize);
        }

        private byte[] GetPasswordBytes(string password)
        {
            return EncodingConverter.GetBytes(password);
        }
    }
}
