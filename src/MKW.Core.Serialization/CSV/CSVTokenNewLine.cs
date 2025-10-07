namespace MKW.Core.Serialization.CSV
{
    public sealed record class CSVTokenNewLine : CSVToken
    {
        internal static CSVTokenNewLine Instance = new CSVTokenNewLine();

        public CSVTokenNewLine() : base("\n")
        {
        }
    }
}
