// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.Exceptions
{
    public sealed class InvalidUserAccessRequestException(Exception innerException)
        : Exception("User access request is invalid.", innerException)
    {
    }
}
