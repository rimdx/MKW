// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Client
{
    public class UserTrustProvider
    {
        protected readonly IDatabaseNG.ISnapshot snapshot;
        protected readonly ClientCryptography crypto;
        private readonly IAsymmetricPublicTransformer meKey;
        private readonly IAsymmetricPublicTransformer adminKey;

        public UserTrustProvider(IDatabaseNG.ISnapshot snapshot,
                                 ClientCryptography crypto,
                                 IAsymmetricPublicTransformer meKey,
                                 IAsymmetricPublicTransformer adminKey)
        {
            this.snapshot = snapshot;
            this.crypto = crypto;
            this.meKey = meKey;
            this.adminKey = adminKey;
        }

        private bool VerifyTrust(DatabaseUser user)
        {
            ReadOnlyMemory<byte> bytes = snapshot.SerializeProtectedData(user.ProtectedData);

            // trust ourselves
            if (user.ProtectedData.PublicKey.Span.SequenceEqual(meKey.ExportPublicKey().Span))
            {
                return true;
            }

            // trust admin
            // TODO: verify admin
            if (user.ProtectedData.PublicKey.Span.SequenceEqual(adminKey.ExportPublicKey().Span))
            {
                return true;
            }

            // otherwise verify admin trust to this user
            DatabaseTrustSignature adminSignature = UserSignatureManager.GetAdminSignature(user.ProtectedData.Signature);
            if (adminKey.Verify(bytes.Span, adminSignature.SignatureBytes.Span))
            {
                return true;
            }

            return false;
        }

        public bool VerifyTrust(UserId userId)
        {
            return VerifyTrust(snapshot.OpenUser(userId));
        }

        public IEnumerable<UserId> EnumerateTrustedUsers()
        {
            foreach (DatabaseUser user in snapshot.EnumerateUsers())
            {
                if (VerifyTrust(user))
                {
                    yield return user.Id;
                }
            }
        }
    }
}
