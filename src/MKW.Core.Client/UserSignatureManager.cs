// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Storage;

namespace MKW.Core.Client
{
    internal static class UserSignatureManager
    {
        // returns mutated user
        public static DatabaseUser AddSignature(DatabaseUser user, DatabaseTrustSignature signature)
        {
            foreach (DatabaseTrustSignature item in user.ProtectedData.Signature)
            {
                if (item.Id.Equals(signature.Id))
                {
                    throw new Exception("Signature of this user already exists.");
                }
            }

            IReadOnlyCollection<DatabaseTrustSignature> newSignatures = [
                .. user.ProtectedData.Signature,
                signature,
            ];

            DatabaseUserProtectedDataSigned newProtectedData = user.ProtectedData with
            {
                Signature = newSignatures,
            };

            return user with
            {
                ProtectedData = newProtectedData,
            };
        }

        // finds signature in targetSignatures's enumerator, created by sourceId
        public static DatabaseTrustSignature GetSignature(IEnumerable<DatabaseTrustSignature> targetSignatures, UserId sourceId)
        {
            foreach (DatabaseTrustSignature signature in targetSignatures)
            {
                if (signature.Id.Equals(sourceId))
                {
                    return signature;
                }
            }

            throw new Exception("No signature was found.");
        }

        public static DatabaseTrustSignature GetAdminSignature(IEnumerable<DatabaseTrustSignature> targetSignatures)
        {
            return GetSignature(targetSignatures, UserId.Admin());
        }
    }
}
