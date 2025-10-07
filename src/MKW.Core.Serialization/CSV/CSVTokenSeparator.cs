namespace MKW.Core.Serialization.CSV
{
    public sealed record class CSVTokenSeparator : CSVToken
    {
        internal static CSVTokenSeparator Instance = new CSVTokenSeparator();

        public CSVTokenSeparator() : base(",")
        {
        }
    }
}
