// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.GUI.SingleInstance
{
    public interface ISingleInstanceApplication
    {
        void InvokeExternalInstance(RunRequest request);
    }
}
