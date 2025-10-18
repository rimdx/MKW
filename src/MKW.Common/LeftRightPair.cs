// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Common
{
    public record class LeftRightPair<T>(
        T? Left,
        T? Right
    ) where T : class;
}
