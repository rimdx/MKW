namespace MKW.Core.Serialization.CSV
{
    public sealed record class CSVTokenString : CSVToken
    {
        public CSVTokenString(string data) : base(data)
        {
        }
    }
}
