using System.Collections;
using System.Text;

namespace MKW.Core
{
    public sealed class EntryPayload
        : IReadOnlyCollection<KeyValuePair<EntryPayloadKey, string>>
    {
        private readonly Dictionary<EntryPayloadKey, string> data;

        public int Count => data.Count;

        public EntryPayload()
        {
            data = [];
        }

        private EntryPayload(Dictionary<EntryPayloadKey, string> data)
        {
            this.data = data;
        }

        public static EntryPayload FromDictionary(Dictionary<EntryPayloadKey, string> items)
        {
            return new EntryPayload(items);
        }

        public string? GetProperty(EntryPayloadKey key)
        {
            if (data.TryGetValue(key, out string? value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        public string GetPropertyOrEmpty(EntryPayloadKey key)
        {
            return GetProperty(key) ?? string.Empty;
        }

        public void SetProperty(EntryPayloadKey key, string? value)
        {
            if (value == null)
            {
                data.Remove(key);
            }
            else
            {
                data[key] = value;
            }
        }

        public override bool Equals(object? obj)
        {
            // handle same key+value/different order?
            return obj is EntryPayload payload && data.SequenceEqual(payload.data);
        }

        public override int GetHashCode()
        {
            return data.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<EntryPayloadKey, string> pair in data)
            {
                sb.AppendLine($"{pair.Key}\t = {pair.Value}");
            }

            return sb.ToString();
        }

        public IEnumerator<KeyValuePair<EntryPayloadKey, string>> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }
    }
}
