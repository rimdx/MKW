
<<<<<<< TODO: Unmerged change from project 'MKW.Core.Storage.JSON (net472)', Before:
namespace MKW.Core.Storage.JSON.Types
{
=======
using MKW;
using MKW.Core;
using MKW.Core.Storage;
using MKW.Core.Storage.JSON;
using MKW.Core.Storage.JSON;
using MKW.Core.Storage.JSON.Types;

namespace MKW.Core.Storage.JSON
{
>>>>>>> After
    namespace MKW.Core.Storage.JSON
{
    internal record class JSONDatabaseUser
    {
        public required ReadOnlyMemory<byte> Salt { get; init; }

        public required ReadOnlyMemory<byte> PublicKey { get; init; }

        public required ReadOnlyMemory<byte> PrivateKey { get; init; }

        public required ReadOnlyMemory<byte> Metadata { get; init; }
        public required ReadOnlyMemory<byte> MetadataAdminSignature { get; init; }

        public required ReadOnlyMemory<byte> AdminTrustSignature { get; init; }
        public required ReadOnlyMemory<byte> AdminSignature { get; init; }

        public static JSONDatabaseUser Serialize(DatabaseUser user)
        {
            return new JSONDatabaseUser
            {
                Salt = user.Salt,
                PrivateKey = user.PrivateKey.EncryptedPayload,

                PublicKey = user.PublicKey.Payload,
                AdminTrustSignature = user.PublicKey.Signature,

                Metadata = user.Metadata.Payload,
                MetadataAdminSignature = user.Metadata.Signature,

                AdminSignature = user.AdminSignature,
            };
        }

        public static DatabaseUser Deserialize(UserId id, JSONDatabaseUser user)
        {
            return new DatabaseUser
            {
                Id = id,
                Salt = user.Salt,
                PublicKey = new SignedPayload(user.PublicKey, user.AdminTrustSignature),
                PrivateKey = new SecretPayload(user.PrivateKey),
                Metadata = new SignedPayload(user.Metadata, user.MetadataAdminSignature),
                AdminSignature = user.AdminSignature,
            };
        }
    }
}
