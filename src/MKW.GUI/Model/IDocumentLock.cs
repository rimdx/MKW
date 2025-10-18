// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.GUI.Model
{
    public interface IDocumentLock : IDisposable
    {
        DatabaseModel Database { get; }

        IDocumentLock Clone();
    }
}
