using MKW.Core.Implementation;
using MKW.Core.Storage;
using MKW.Cryptography;
using MKW.Cryptography.Exceptions;

namespace MKW.Core.Client
{
    internal sealed class UserController : IUserController, IDisposable
    {
        private readonly ClientSession client;
        private readonly ICryptographyProvider crypto;
        private readonly IDatabase database;

        public UserController(ClientSession client,
                              ICryptographyProvider crypto,
                              IDatabase database)
        {
            this.client = client;
            this.crypto = crypto;
            this.database = database;
        }

        public UserAccessRequest CreateUserAccessRequest(string password)
        {
            DatabaseUser admin = database.OpenUser(UserId.Admin());

            SystemCredentialsManager credManager = new SystemCredentialsManager(crypto);

            IUserCredentials userCreds = crypto.CreateUserCredentials(password, CommonCryptographyAlgorithms.Pbkdf2);
            using SystemCredentials systemCreds = credManager.GenerateCredentials(userCreds);

            // TODO: prompt user?
            Memory<byte> signature = systemCreds.Transformer.Sign(admin.PublicKey.Payload.Span);

            return new UserAccessRequest
            {
                Salt = systemCreds.Salt,
                PublicKey = systemCreds.PublicKey,
                EncryptedPrivateKey = systemCreds.PrivateKey,
                AdminSignature = signature,
            };
        }

        public IUserSession OpenUser(UserId id, string password)
        {
            if (id.IsAdmin)
            {
                // todo:
                return client.OpenAdmin(password);
            }
            else
            {
                DatabaseUser user = database.OpenUser(id);

                // Credentials can be opened within the entered password and the public salt
                IUserCredentials creds = crypto.OpenUserCredentials(password,
                                                                    user.Salt,
                                                                    CommonCryptographyAlgorithms.Pbkdf2);

                return OpenUserInternal(user, creds);
            }
        }

        public IUserSession OpenUser(string password)
        {
            foreach (DatabaseUser user in database.EnumerateUsers())
            {
                try
                {
                    IUserCredentials creds = crypto.OpenUserCredentials(password,
                                                                        user.Salt,
                                                                        CommonCryptographyAlgorithms.Pbkdf2);

                    return OpenUserInternal(database.OpenUser(user.Id), creds);
                }
                catch (Exceptions.InvalidPasswordException)
                {
                    // Ignore this user if the credentials are invalid
                }
            }

            throw new Exception("No valid user found with the provided password.");
        }

        private IUserSession OpenUserInternal(DatabaseUser user, IUserCredentials creds)
        {
            try
            {
                // Private data of the user is encrypted symmetrically using our creds (decoder
                // also needs some data stored in the public section of the object).
                using ISymmetricTransformer decoder = crypto.OpenSymmetricTransformer(
                    creds.GetSecretKey().Span,
                    creds.ExportSalt().Span,
                    CommonCryptographyAlgorithms.Aes128Gcm);

                // Let's try'N decode the private key. We could potentially fail here. So
                // some validation may be required.
                Memory<byte> privateKeyBytes = decoder.Decrypt(user.PrivateKey.EncryptedPayload.Span);

                return new UserSession(crypto, database, user, privateKeyBytes.Span);
            }
            catch (SymmetricOperationFailedException ex)
            {
                throw new Exceptions.InvalidPasswordException(ex);
            }
            catch (InvalidKeyException ex)
            {
                // Possible occurrence, as experiments have shown. Fails in 1/~35 times.
                throw new Exceptions.InvalidPasswordException(ex);
            }
        }

        public IEnumerable<UserInfo> EnumerateUsers()
        {
            DatabaseUser admin = database.OpenUser(UserId.Admin());

            using IAsymmetricPublicTransformer adminKey = crypto.OpenAsymmetricTransformer(
                admin.PublicKey.Payload.Span,
                CommonCryptographyAlgorithms.Rsa2048);

            UserMetadataDecoder metadataDecoder = new UserMetadataDecoder(adminKey);

            foreach (DatabaseUser user in database.EnumerateUsers())
            {
                yield return CreateUserInfo(user, metadataDecoder.OpenMetadata(user));
            }
        }

        public UserInfo GetUserInfo(UserId id)
        {
            DatabaseUser admin = database.OpenUser(UserId.Admin());
            DatabaseUser user = database.OpenUser(id);

            using IAsymmetricPublicTransformer adminKey = crypto.OpenAsymmetricTransformer(
                admin.PublicKey.Payload.Span,
                CommonCryptographyAlgorithms.Rsa2048);

            UserMetadataDecoder metadataDecoder = new UserMetadataDecoder(adminKey);

            return CreateUserInfo(user, metadataDecoder.OpenMetadata(user));
        }

        public void Dispose()
        {
            /* no-op */
        }

        private static UserInfo CreateUserInfo(DatabaseUser user, UserMetadata metadata, Trust trust = Trust.Unknown)
        {
            return new UserInfo
            {
                Id = user.Id,
                PublicKey = user.PublicKey.Payload,
                Trust = trust,
                Metadata = metadata
            };
        }
    }
}
