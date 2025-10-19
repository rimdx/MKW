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

        public static EntryId FromBytes(ReadOnlySpan<byte> id)
        {
            return new EntryId(id.ToArray());
        }

        public static EntryId FromString(string str)
        {
            try
            {
                return new EntryId(new Guid(str).ToByteArray());
            }
            catch (Exception)
            {
                // backward compat: parse id as a guid.
                Guid guid = new Guid(str);
                return new EntryId(guid.ToByteArray());
            }
        }

        public static EntryId Create()
        {
            return new EntryId(Create(Size));
        }
    }
}
