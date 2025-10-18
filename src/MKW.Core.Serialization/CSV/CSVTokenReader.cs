// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Text;

namespace MKW.Core.Serialization.CSV
{
    public sealed class CSVTokenReader : IDisposable
    {
        private readonly TextReader reader;

        public CSVTokenReader(TextReader reader)
        {
            this.reader = reader;
        }

        public CSVToken? ReadToken()
        {
            int c = reader.Peek();

            if (c == -1)
            {
                return null;
            }
            else if (CSVHelpers.IsNewLine(c))
            {
                SkipNewLines();
                return CSVTokenNewLine.Instance;
            }
            else if (CSVHelpers.IsSeparator(c))
            {
                reader.Read();
                return CSVTokenSeparator.Instance;
            }
            else
            {
                StringBuilder str = new StringBuilder();

                if (CSVHelpers.IsQuote(c))
                {
                    // advance quote
                    reader.Read();
                    ReadQuotedLiteral(str);
                }
                else
                {
                    ReadStringLiteral(str);
                }

                return new CSVTokenString(str.ToString());
            }
        }

        private void SkipNewLines()
        {
            while (true)
            {
                int c = reader.Peek();

                if (CSVHelpers.IsSpace(c) || CSVHelpers.IsNewLine(c))
                {
                    reader.Read();
                }
                else
                {
                    break;
                }
            }
        }

        private void ReadStringLiteral(StringBuilder literal)
        {
            while (true)
            {
                int c = reader.Peek();

                if (c == -1 || CSVHelpers.IsSeparator(c) || CSVHelpers.IsNewLine(c))
                {
                    return;
                }
                else
                {
                    literal.Append((char)c);
                    reader.Read();
                }
            }
        }

        private void ReadCharLiteral(StringBuilder literal)
        {
            int c = reader.Peek();
            literal.Append((char)c);
            reader.Read();
        }

        private void ReadQuotedLiteral(StringBuilder literal)
        {
            while (true)
            {
                int c = reader.Peek();

                if (c == -1)
                {
                    return;
                }
                else if (CSVHelpers.IsQuote(c))
                {
                    // advance quote
                    reader.Read();

                    if (CSVHelpers.IsQuote(reader.Peek()))
                    {
                        ReadCharLiteral(literal);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ReadCharLiteral(literal);
                }
            }
        }

        public IEnumerable<CSVToken> EnumerateTokens()
        {
            while (true)
            {
                CSVToken? token = ReadToken();

                if (token == null)
                {
                    break;
                }
                else
                {
                    yield return token;
                }
            }
        }

        public void Dispose()
        {
            reader.Dispose();
        }
    }
}
