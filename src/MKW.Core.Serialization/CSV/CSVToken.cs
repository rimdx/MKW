namespace MKW.Core.Serialization.CSV
{
    public abstract record class CSVToken
    {
        public string Data { get; }

        protected CSVToken(string data)
        {
            Data = data;
        }
    }
}
