// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.JSON
{
    internal sealed class JSONDatabaseUserProtectedDataSigned
    {
        public ReadOnlyMemory<byte> PublicKey { get; init; }
        public ReadOnlyMemory<byte> Metadata { get; init; }
        public ReadOnlyMemory<byte> Signature { get; init; }

        public static JSONDatabaseUserProtectedDataSigned Serialize(DatabaseUserProtectedDataSigned obj)
        {
            return new JSONDatabaseUserProtectedDataSigned
            {
                PublicKey = obj.PublicKey,
                Metadata = obj.Metadata,
                Signature = obj.Signature,
            };
        }

        public static DatabaseUserProtectedDataSigned Deserialize(JSONDatabaseUserProtectedDataSigned obj)
        {
            return new DatabaseUserProtectedDataSigned
            {
                PublicKey = obj.PublicKey,
                Metadata = obj.Metadata,
                Signature = obj.Signature,
            };
        }
    }
}
