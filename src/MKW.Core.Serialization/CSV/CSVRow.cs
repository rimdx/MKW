using System.Collections;

namespace MKW.Core.Serialization.CSV
{
    public sealed record class CSVRow : IReadOnlyCollection<CSVField>
    {
        private readonly List<CSVField> values;

        internal CSVRow(List<CSVField> values)
        {
            this.values = values;
        }

        public int Count => values.Count;

        public IEnumerator<CSVField> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return values.GetEnumerator();
        }
    }
}
