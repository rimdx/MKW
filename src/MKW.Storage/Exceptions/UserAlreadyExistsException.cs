// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.Exceptions
{
    public sealed class UserAlreadyExistsException() : Exception("User with the same ID already exist.");
}
