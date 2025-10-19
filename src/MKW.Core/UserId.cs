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
            try
            {
                return new UserId(Base16Convert.GetBytes(str));
            }
            catch (Exception)
            {
                // backward compat: just strips 8 least significant bytes and put them
                // into the resulting id.
                Guid guid = new Guid(str);
                ReadOnlyMemory<byte> bytes = guid.ToByteArray();
                ReadOnlyMemory<byte> sliced = bytes.Slice(bytes.Length - Size, Size);
                return new UserId(sliced);
            }
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
