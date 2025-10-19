// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.MKPG
{
    internal interface IFileEditorFactory
    {
        Stream CreateReader();
        Stream CreateWriter();
    }
}
