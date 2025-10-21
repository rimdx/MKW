// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core
{
    public interface IUserSession : IDisposable
    {
        UserId Id { get; }

        EntryId CreateEntry(EntryPayload payload);
        void CreateEntry(EntryId id, EntryPayload payload);
        void UpdateEntry(EntryId id, EntryPayload newPayload);

        void DeleteEntry(EntryId id);

        EntryPayload? OpenEntry(EntryId id);

        IEnumerable<KeyValuePair<EntryId, EntryPayload?>> EnumerateEntries();

        IEnumerable<UserId> EnumerateTrustedUsers();

        bool VerifyTrust(UserId userId);

        UserMetadata OpenMetadata();
    }
}
