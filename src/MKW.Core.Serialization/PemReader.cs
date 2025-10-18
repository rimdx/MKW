// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Asn1;
using System.Text;

namespace MKW.Core.Serialization
{
    internal sealed class PemReader
        : IDisposable
    {
        private readonly TextReader reader;

        private readonly string header;
        private readonly string footer;

        public PemReader(TextReader reader, string type)
        {
            this.reader = reader;

            header = $"-----BEGIN {type}-----";
            footer = $"-----END {type}-----";
        }

        private string? ReadLine()
        {
            while (true)
            {
                string? line = reader.ReadLine();

                if (line == null)
                {
                    return null;
                }
                else
                {
                    if (line.Length == 0)
                    {
                        continue;
                    }
                    else
                    {
                        return line.Trim();
                    }
                }
            }
        }

        public Asn1Encodable ReadObject()
        {
            while (true)
            {
                string? line = ReadLine();

                if (line == null || line.StartsWith(header))
                {
                    break;
                }
            }

            StringBuilder buffer = new StringBuilder();

            while (true)
            {
                string? line = ReadLine();

                if (line == null || line.StartsWith(footer))
                {
                    break;
                }

                buffer.Append(line);
            }

            byte[] bytes = Convert.FromBase64String(buffer.ToString());
            Asn1Object obj = Asn1Object.FromByteArray(bytes);

            return obj;
        }

        public void Dispose()
        {
            reader.Dispose();
        }
    }
}
