// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core
{
    public sealed class EntryId : IdBase
    {
        public const int Size = 16;

        private EntryId(byte[] data)
            : base(data, Size)
        {
        }

        public string GetString()
        {
            return new Guid(data.ToArray()).ToString();
        }

        public static EntryId FromBytes(ReadOnlySpan<byte> id)
        {
            return new EntryId(id.ToArray());
        }

        public static EntryId FromString(string str)
        {
            return new EntryId(new Guid(str).ToByteArray());
        }

        public static EntryId Create()
        {
            return new EntryId(Guid.NewGuid().ToByteArray());
        }
    }
}
