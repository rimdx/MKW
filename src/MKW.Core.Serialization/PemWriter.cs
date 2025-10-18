// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using Org.BouncyCastle.Asn1;
using System.Security.Cryptography;

namespace MKW.Core.Serialization
{
    internal sealed class PemWriter
        : IDisposable
    {
        private readonly TextWriter writer;

        private readonly string header;
        private readonly string footer;

        public PemWriter(TextWriter writer, string type)
        {
            this.writer = writer;

            header = $"-----BEGIN {type}-----";
            footer = $"-----END {type}-----";
        }

        public void AddObject(Asn1Encodable obj)
        {
            writer.WriteLine(header);

            {
                // output to writer

                // in ascii encoding->text
                using ASCIIStream encoder = new ASCIIStream(writer);
                // add a line breaks every N symbols
                using LineBreakTransform lineBreakTransform = new LineBreakTransform(52);
                using CryptoStream lineBreakStream = new CryptoStream(encoder, lineBreakTransform, CryptoStreamMode.Write);
                // convert bytes to ascii base64
                using ToBase64Transform base64transform = new ToBase64Transform();
                using CryptoStream base64stream = new CryptoStream(lineBreakStream, base64transform, CryptoStreamMode.Write);

                obj.EncodeTo(base64stream, Asn1Encodable.Der);
            }

            writer.WriteLine(footer);
        }

        public void Dispose()
        {
            writer.Dispose();
        }
    }
}
