namespace MKW.Core.Serialization.CSV
{
    public sealed record class CSVField
    {
        public string Value { get; }

        public CSVField(string value)
        {
            Value = value;
        }
    }
}
