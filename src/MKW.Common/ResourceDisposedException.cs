// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Common
{
    [Serializable]
    public class ResourceDisposedException()
        : Exception("Resource is disposed.")
    {
    }
}
