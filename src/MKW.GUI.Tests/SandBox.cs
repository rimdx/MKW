// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.Services;
using MKW.Testing.Common;

namespace MKW.GUI.Tests
{
    public class SandBox : SandBoxBase
    {
        public RegistryService RegistryService;

        public SandBox()
        {
            RegistryService = new RegistryService(TestRootKey);
        }

        public override void Dispose()
        {
            RegistryService.Dispose();
        }
    }
}
