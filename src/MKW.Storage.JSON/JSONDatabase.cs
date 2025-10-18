// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Text.Json.Serialization;

namespace MKW.Storage.JSON
{
    internal sealed class JSONDatabase
    {
        public JSONDatabaseUser? Admin { get; set; }

        [JsonRequired]
        public IDictionary<string, JSONDatabaseUser> Users { get; init; }

        // EntryId -> Entry
        [JsonRequired]
        public IDictionary<string, JSONDatabaseSecretEntry> Entries { get; init; }

        public JSONDatabase()
        {
            Users = new Dictionary<string, JSONDatabaseUser>();
            Entries = new Dictionary<string, JSONDatabaseSecretEntry>();
        }
    }
}
