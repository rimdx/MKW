// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.MKPG
{
    internal partial interface IFileEditorFactory
    {
        public interface ITransaction : IDisposable
        {
            Stream Reader { get; }
            Stream Writer { get; }

            public void Commit();
        }
    }
}
