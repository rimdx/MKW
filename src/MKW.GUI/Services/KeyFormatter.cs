// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using System.Text;

namespace MKW.GUI.Services
{
    public class KeyFormatter
    {
        private readonly int lineLength;

        public KeyFormatter(int lineLength)
        {
            this.lineLength = lineLength;
        }

        public string GetBase64String(ReadOnlySpan<byte> data)
        {
            string encoded = Convert.ToBase64String(data.ToArray());
            return InsertLineBreaks(encoded);
        }

        public string GetBase32String(ReadOnlySpan<byte> data)
        {
            string encoded = Base32Convert.Encode(data);
            return InsertLineBreaks(encoded);
        }

        private string InsertLineBreaks(string str)
        {
            StringBuilder sb = new StringBuilder();

            int i = 0;
            foreach (char c in str)
            {
                if (i >= lineLength)
                {
                    sb.AppendLine();
                    i = 0;
                }

                sb.Append(c);

                i++;
            }

            return sb.ToString();
        }
    }
}
