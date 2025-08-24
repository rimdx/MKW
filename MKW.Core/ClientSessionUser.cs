using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;
using System.Security.Cryptography;

namespace MKW.Core.Client
{
    public partial class ClientSession : IDisposable
    {
        public UserInfo PromoteUser(string password)
        {
            SystemCredentialsManager credManager = new SystemCredentialsManager();

            UserCredentials userCreds = UserCredentials.Create(password);
            SystemCredentials systemCreds = credManager.GenerateCredentials(userCreds);

            IDatabaseUser user = Database.CreateUser(UserId.Create());

            user.PublicKey = systemCreds.PublicKey;
            user.PrivateKey = systemCreds.PrivateKey;
            user.Salt = systemCreds.Salt;

            user.Save();

            return UserInfo.FromDatabaseUser(user);
        }

        public UserSession OpenUser(UserId id, string password)
        {
            if (id.IsAdmin)
            {
                return OpenAdmin(password);
            }
            else
            {
                IDatabaseUser user = Database.OpenUser(id, false);

                // Credentials can be opened within the entered password and the public salt
                UserCredentials creds = UserCredentials.Open(password, user.Salt);

                return OpenUser(user, creds);
            }
        }

        public UserSession OpenUser(string password)
        {
            foreach (IDatabaseUser user in Database.EnumerateUsers())
            {
                try
                {
                    UserCredentials creds = UserCredentials.Open(password, user.Salt);

                    return OpenUser(Database.OpenUser(user.Id, false), creds);
                }
                catch (CryptographicException)
                {
                    // Ignore this user if the credentials are invalid
                }
            }

            throw new Exception("No valid user found with the provided password.");
        }

        public UserSession OpenUser(IDatabaseUser user, UserCredentials creds)
        {
            // Private data of the user is encrypted symmetrically using our creds (decoder
            // also needs some data stored in the public section of the object).
            using SymmetricTransformer decoder = SymmetricTransformer.Open(creds.GetEncodingHash().Span,
                                                                           creds.ExportSalt().Span);

            // Let's try'N decode the private key. We could potentially fail here. So
            // some validation may be required.
            Memory<byte> privateKeyBytes = decoder.Decrypt(user.PrivateKey.Span);

            return new UserSession(this, user, privateKeyBytes.Span);
        }
    }
}
