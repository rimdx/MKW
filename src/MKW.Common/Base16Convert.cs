// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Text;

namespace MKW.Common
{
    public static class Base16Convert
    {
        private static readonly string encodingTable = "0123456789ABCDEF";

        public static void GetBytes(string str, Span<byte> buf)
        {
            if (str.Length != buf.Length * 2)
            {
                throw new ArgumentException("Buffer length mismatch.");
            }

            for (int i = 0; i < buf.Length; i++)
            {
                byte b0 = DecodeChar(str[i * 2 + 0]);
                byte b1 = DecodeChar(str[i * 2 + 1]);

                buf[i] = (byte)(b0 << 4 | b1);
            }
        }

        public static byte[] GetBytes(string input)
        {
            byte[] buffer = new byte[input.Length / 2];
            GetBytes(input, buffer);
            return buffer;
        }

        public static void GetString(ReadOnlySpan<byte> buf, StringBuilder str)
        {
            for (int i = 0; i < buf.Length; i++)
            {
                var b0 = (byte)((buf[i] & 0xF0) >> 4);
                var b1 = (byte)(buf[i] & 0x0F);

                str.Append(EncodeChar(b0));
                str.Append(EncodeChar(b1));
            }
        }

        public static string GetString(ReadOnlySpan<byte> input)
        {
            StringBuilder sb = new StringBuilder(input.Length / 2);
            GetString(input, sb);
            return sb.ToString();
        }

        private static byte DecodeChar(char ch)
        {
            if ('0' <= ch && ch <= '9')
            {
                return (byte)(ch - '0');
            }
            else if ('A' <= ch && ch <= 'F')
            {
                return (byte)(ch - 'A' + 10);
            }
            else if ('a' <= ch && ch <= 'f')
            {
                return (byte)(ch - 'a' + 10);
            }
            else
            {
                throw new Exception("Invalid Base16 character.");
            }
        }

        private static char EncodeChar(byte b)
        {
            return encodingTable[b];
        }
    }
}
