using MKW.Core.Serialization.Exceptions;

namespace MKW.Core.Serialization.CSV
{
    public sealed class CSVSerializer : IDisposable
    {
        private readonly CSVTokenReader reader;
        private readonly int columns;

        public CSVSerializer(CSVTokenReader reader, int columns = -1)
        {
            this.reader = reader;
            this.columns = columns;
        }

        public CSVRow? ReadRow()
        {
            List<CSVField> fields = [];

            while (true)
            {
                CSVToken? token = reader.ReadToken();

                if (token == null)
                {
                    break;
                }
                else if (token is CSVTokenNewLine)
                {
                    break;
                }
                else if (token is CSVTokenString literal)
                {
                    fields.Add(new CSVField(literal.Data));

                    CSVToken? separator = reader.ReadToken();

                    if (separator == null)
                    {
                        break;
                    }
                    else if (separator is CSVTokenNewLine)
                    {
                        break;
                    }
                    else if (separator is CSVTokenSeparator)
                    {
                        // we're fine
                    }
                    else
                    {
                        throw new CSVUnexpectedTokenException(separator.GetType());
                    }
                }
                else
                {
                    throw new CSVUnexpectedTokenException(token.GetType());
                }
            }

            if (fields.Count == 0)
            {
                return null;
            }
            else
            {
                return new CSVRow(fields);
            }
        }

        public IEnumerable<CSVRow> EnumerateRows()
        {
            while (true)
            {
                CSVRow? row = ReadRow();

                if (row == null)
                {
                    yield break;
                }
                else
                {
                    yield return row;
                }
            }
        }

        public void Dispose()
        {
            reader.Dispose();
        }
    }
}
