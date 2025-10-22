// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Text.Json;

namespace MKW.Storage.JSON
{
    internal sealed class JSONDatabaseUserProtectedData
    {
        public ReadOnlyMemory<byte> PublicKey { get; init; }
        public ReadOnlyMemory<byte> Metadata { get; init; }

        public static ReadOnlyMemory<byte> Serialize(DatabaseUserProtectedData obj)
        {
            JSONDatabaseUserProtectedData json = new JSONDatabaseUserProtectedData
            {
                PublicKey = obj.PublicKey,
                Metadata = obj.Metadata,
            };

            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }
    }
}
