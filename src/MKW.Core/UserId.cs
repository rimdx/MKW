// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;

namespace MKW.Core
{
    public sealed class UserId : IdBase
    {
        public const int Size = 8;

        public bool IsAdmin => Equals(Admin());

        private UserId(ReadOnlyMemory<byte> data)
            : base(data.ToArray(), Size)
        {
        }

        public static UserId Admin()
        {
            return new UserId(new byte[Size]);
        }

        public static UserId FromString(string str)
        {
            return new UserId(Base16Convert.GetBytes(str));
        }

        public static UserId FromBytes(ReadOnlyMemory<byte> data)
        {
            return new UserId(data);
        }

        public static UserId Create()
        {
            return new UserId(Create(Size));
        }
    }
}
