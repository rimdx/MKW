// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Asn1;

namespace MKW.Core.Serialization
{
    internal sealed class Asn1SequenceReader
        : IDisposable
    {
        private int position;
        private readonly Asn1Sequence sequence;

        public static Asn1SequenceReader GetInstance(object? obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException();
            }
            else if (obj is Asn1SequenceReader reader)
            {
                return reader;
            }
            else
            {
                return new Asn1SequenceReader(Asn1Sequence.GetInstance(obj));
            }
        }

        public Asn1SequenceReader(Asn1Sequence sequence)
        {
            position = 0;
            this.sequence = sequence;
        }

        public Asn1Encodable Next()
        {
            if (position < sequence.Count)
            {
                Asn1Encodable value = sequence[position];
                position++;
                return value;
            }
            else
            {
                throw new Asn1BadSequenceLengthException(sequence.Count);
            }
        }

        public void Dispose()
        {
            if (position != sequence.Count)
            {
                throw new Asn1BadSequenceLengthException(sequence.Count);
            }
        }
    }
}
