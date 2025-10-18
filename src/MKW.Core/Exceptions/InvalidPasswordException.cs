// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Exceptions
{
    public class InvalidPasswordException(Exception ex)
        : Exception("The password is not correct.", ex)
    {
    }
}
