// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Common
{
    [Serializable]
    public class ResourceNotOwnedException()
        : Exception("Object is not owned by the resource handler.")
    {
    }
}
