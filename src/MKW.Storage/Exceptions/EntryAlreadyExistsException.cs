// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.Exceptions
{
    public sealed class EntryAlreadyExistsException() : Exception("Entry with the same ID already exist.");
}
