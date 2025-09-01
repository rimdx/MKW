namespace MKW.Core.Cryptography.BouncyCastle
{
    public class UserCredentials : IUserCredentials, IDisposable
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

        public ReadOnlyMemory<byte> ExportSalt()
        {
            throw new NotImplementedException();
        }

        public Memory<byte> GetSecretKey()
        {
            throw new NotImplementedException();
        }

        private static ReadOnlyMemory<byte> GenerateSalt()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
