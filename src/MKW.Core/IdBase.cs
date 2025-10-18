// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using System.Security.Cryptography;

namespace MKW.Core
{
    public abstract class IdBase : IComparable<IdBase>
    {
        private static readonly Lazy<RandomNumberGenerator> rng = new Lazy<RandomNumberGenerator>(
            () => RandomNumberGenerator.Create());

        protected byte[] data;

        protected IdBase(byte[] data, int size)
        {
            if (data.Length != size)
            {
                throw new Exception("Bad id size.");
            }

            this.data = data;
        }

        protected static byte[] Create(int size)
        {
            byte[] buf = new byte[size];
            rng.Value.GetBytes(buf);
            return buf;
        }

        public ReadOnlyMemory<byte> GetBytes()
        {
            return data;
        }

        public string GetString()
        {
            return Base16Convert.GetString(data);
        }

        public override string ToString()
        {
            return GetString();
        }

        public int CompareTo(IdBase? other)
        {
            if (other == null)
            {
                return -1;
            }
            else
            {
                return data.AsSpan().SequenceCompareTo(other.data.AsSpan());
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is IdBase id &&
                   data.AsSpan().SequenceEqual(id.data.AsSpan());
        }

        public override int GetHashCode()
        {
            return 42;
        }
    }
}
