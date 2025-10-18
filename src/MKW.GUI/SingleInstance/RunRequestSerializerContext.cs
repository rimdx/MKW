// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Text.Json.Serialization;

namespace MKW.GUI.SingleInstance
{
    [JsonSourceGenerationOptions()]
    [JsonSerializable(typeof(RunRequest))]
    internal partial class RunRequestSerializerContext : JsonSerializerContext
    {
    }
}
