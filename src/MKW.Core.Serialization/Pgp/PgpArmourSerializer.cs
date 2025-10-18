using MKW.Common;
using Org.BouncyCastle.Bcpg;
using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace MKW.Core.Serialization.Pgp
{
    // When OpenPGP encodes data into ASCII Armor, it puts specific headers
    // around the Radix-64 encoded data, so OpenPGP can reconstruct the data
    // later.  An OpenPGP implementation MAY use ASCII armor to protect raw
    // binary data.  OpenPGP informs the user what kind of data is encoded
    // in the ASCII armor through the use of the headers.
    //
    // Concatenating the following data creates ASCII Armor:
    //   - An Armor Header Line, appropriate for the type of data
    //   - Armor Headers
    //   - A blank (zero-length, or containing only whitespace) line
    //   - The ASCII-Armored data
    //   - An Armor Checksum
    //   - The Armor Tail, which depends on the Armor Header Line
    //
    // An Armor Header Line consists of the appropriate header line text
    // surrounded by five (5) dashes ('-', 0x2D) on either side of the
    // header line text.  The header line text is chosen based upon the type
    // of data that is being encoded in Armor, and how it is being encoded.
    // Header line texts include the following strings:
    //
    // [...strip...]
    public static class PgpArmourSerializer
    {
        private static readonly string dashes = new string('-', 5);
        private static readonly string beginPrefix = "BEGIN ";
        private static readonly string endPrefix = "END ";
        private static readonly string checksumPrefix = "=";

        public static void Serialize(TextWriter writer,
                                     PgpArmouredMessage obj)
        {
            writer.WriteLine($"{dashes}{beginPrefix}{obj.MessageTypeHeader}{dashes}");

            WriteHeaders(writer, obj.Headers);

            {
                using ASCIIStream stream = new ASCIIStream(writer);

                using LineBreakTransform lineBreakTransform = new LineBreakTransform(64);
                using CryptoStream lineBreakStream = new CryptoStream(new StreamDisown(stream),
                                                                      lineBreakTransform,
                                                                      CryptoStreamMode.Write);

                using Radix64Encoder radixTransform = new Radix64Encoder();
                using CryptoStream radixStream = new CryptoStream(lineBreakStream,
                                                                  radixTransform,
                                                                  CryptoStreamMode.Write);

                radixStream.Write(obj.Data.Span);
            }

            WriteChecksum(writer, obj.Data.Span);

            writer.WriteLine($"{dashes}{endPrefix}{obj.MessageTypeHeader}{dashes}");
        }

        private static void WriteHeaders(TextWriter writer,
                                         IEnumerable<PgpArmourHeader> headers)
        {
            foreach (PgpArmourHeader header in headers)
            {
                writer.WriteLine(PgpArmourHeaderSerializer.Serialize(header));
            }

            writer.WriteLine();
        }

        private static void WriteChecksum(TextWriter writer,
                                          ReadOnlySpan<byte> data)
        {
            Crc24 crc = new Crc24();

            crc.Update(data);

            writer.Write(checksumPrefix);
            writer.Write(crc.Serialize());
            writer.WriteLine();
        }

        public static PgpArmouredMessage? Deserialize(TextReader reader)
        {
            string? type = ReadBeginHeader(reader);

            if (type == null)
            {
                return null;
            }

            var headers = ReadHeaders(reader).ToArray();

            using MemoryStream output = new MemoryStream();

            {
                using Radix64Decoder radixTransform = new Radix64Decoder();
                using CryptoStream radixStream = new CryptoStream(output,
                                                                  radixTransform,
                                                                  CryptoStreamMode.Write);

                using StreamWriter bodyWriter = new StreamWriter(radixStream, Encoding.ASCII);

                int checksum = ReadBody(reader, bodyWriter);
            }

            string endType = ReadEndHeader(reader);

            return new PgpArmouredMessage
            {
                MessageTypeHeader = type,
                Headers = headers,
                Data = output.ToArray(),
            };
        }

        private static int ReadBody(TextReader reader, TextWriter bodyWriter)
        {
            Span<byte> checksumBytes = stackalloc byte[3];

            while (true)
            {
                string? line = reader.ReadLine();

                if (line == null)
                {
                    throw new EndOfStreamException();
                }

                if (line.StartsWith(checksumPrefix))
                {
                    string sliced = line.Substring(checksumPrefix.Length);

                    if (sliced.Length != 4)
                    {
                        throw new Exception("malformed checksum format");
                    }

                    ReadOnlyMemory<byte> encoded = Encoding.ASCII.GetBytes(sliced);

                    Radix64BitConvert.DecodeBlock(encoded.Span, checksumBytes);

                    return checksumBytes[0] << 16 + checksumBytes[1] << 8 + checksumBytes[2];
                }
                else
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (char ch in line)
                    {
                        if (!char.IsWhiteSpace(ch))
                        {
                            sb.Append(ch);
                        }
                    }

                    bodyWriter.Write(sb);
                }
            }
        }

        private static IEnumerable<PgpArmourHeader> ReadHeaders(TextReader reader)
        {
            while (true)
            {
                string? line = reader.ReadLine();

                if (line == null)
                {
                    throw new EndOfStreamException();
                }

                string trimmed = line.Trim();

                if (trimmed == string.Empty)
                {
                    yield break;
                }
                else
                {
                    yield return PgpArmourHeaderSerializer.Deserialize(trimmed);
                }
            }
        }

        private static string? ReadBeginHeader(TextReader reader)
        {
            while (true)
            {
                string? line = reader.ReadLine();

                if (line == null)
                {
                    return null;
                }

                if (line.StartsWith(dashes))
                {
                    if (!line.EndsWith(dashes))
                    {
                        throw new Exception("malformed armour header");
                    }

                    if (line.Length < dashes.Length * 2)
                    {
                        throw new Exception("malformed armour header");
                    }

                    // -----BEGIN PGP MESSAGE  -----
                    //      ^               ^
                    string slice = line.Substring(dashes.Length, line.Length - dashes.Length * 2);
                    string trimmed = slice.Trim();

                    if (!trimmed.StartsWith(beginPrefix))
                    {
                        throw new Exception("malformed armour header");
                    }

                    string type = trimmed.Substring(beginPrefix.Length);

                    return type;
                }
            }
        }

        private static string ReadEndHeader(TextReader reader)
        {
            while (true)
            {
                string? line = reader.ReadLine();

                if (line == null)
                {
                    throw new EndOfStreamException();
                }

                if (line.StartsWith(dashes))
                {
                    if (!line.EndsWith(dashes))
                    {
                        throw new Exception("malformed armour footer");
                    }

                    if (line.Length < dashes.Length * 2)
                    {
                        throw new Exception("malformed armour footer");
                    }

                    // -----END PGP MESSAGE  -----
                    //      ^             ^
                    string slice = line.Substring(dashes.Length, line.Length - dashes.Length * 2);
                    string trimmed = slice.Trim();

                    if (!trimmed.StartsWith(endPrefix))
                    {
                        throw new Exception("malformed armour footer");
                    }

                    string type = trimmed.Substring(endPrefix.Length);

                    return type;
                }
            }
        }
    }
}
