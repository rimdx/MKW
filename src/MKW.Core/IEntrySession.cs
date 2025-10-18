// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core
{
    public interface IEntrySession : IDisposable
    {
        EntryId Id { get; }

        void UpdatePayload(EntryPayload payload);
        EntryPayload? OpenPayload();

        IEnumerable<UserId> EnumerateAccess();
    }
}
