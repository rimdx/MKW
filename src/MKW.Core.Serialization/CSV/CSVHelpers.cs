namespace MKW.Core.Serialization.CSV
{
    internal static class CSVHelpers
    {
        public static bool IsSpace(int c)
        {
            return c == ' ' || c == '\t';
        }

        public static bool IsNewLine(int c)
        {
            return c == '\n' || c == '\r';
        }

        public static bool IsSeparator(int c)
        {
            return c == ',';
        }

        public static bool IsQuote(int c)
        {
            return c == '"';
        }
    }
}
