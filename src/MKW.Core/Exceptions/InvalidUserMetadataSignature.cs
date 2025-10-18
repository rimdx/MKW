// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Exceptions
{
    [Serializable]
    public class InvalidUserMetadataSignature()
        : Exception("Metadata signature is not valid.")
    {
    }
}
