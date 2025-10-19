// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.MKPG
{
    internal partial interface IFileEditorFactory : IDisposable
    {
        Stream CreateReader();

        ITransaction CreateTransaction();
    }
}
