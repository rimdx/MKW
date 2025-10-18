// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Bcpg;
using System.Security.Cryptography;

namespace MKW.Core.Serialization.OpenPgp
{
    public sealed class Radix64Encoder : ICryptoTransform
    {
        private readonly Crc24 crc;

        public bool CanReuseTransform => false;
        public bool CanTransformMultipleBlocks => false;

        public int InputBlockSize => 3;
        public int OutputBlockSize => 4;

        public Radix64Encoder(Crc24 crc)
        {
            this.crc = crc;
        }

        public int TransformBlock(byte[] inputBuffer,
                                  int inputOffset,
                                  int inputCount,
                                  byte[] outputBuffer,
                                  int outputOffset)
        {
            ReadOnlySpan<byte> inputSpan = new ReadOnlySpan<byte>(inputBuffer, inputOffset, inputCount);
            Span<byte> outputSpan = new Span<byte>(outputBuffer, outputOffset, OutputBlockSize);

            Radix64BitConvert.EncodeFullBlock(inputSpan, outputSpan);
            crc.Update(inputSpan);

            return outputBuffer.Length;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer,
                                          int inputOffset,
                                          int inputCount)
        {
            ReadOnlySpan<byte> inputSpan = new ReadOnlySpan<byte>(inputBuffer, inputOffset, inputCount);

            if (inputSpan.Length == 0)
            {
                return [];
            }
            else
            {
                byte[] outputBuffer = new byte[4];

                Radix64BitConvert.EncodeFinalBlock(inputSpan, outputBuffer);
                crc.Update(inputSpan);

                return outputBuffer;
            }
        }

        public void Dispose()
        {
        }
    }
}
