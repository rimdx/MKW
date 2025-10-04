namespace MKW.Core
{
    public sealed class EntryPayloadKey
    {
        public const string NamespaceSeparator = ":";

        private readonly string key;

        public EntryPayloadKey(string key)
        {
            this.key = key;
        }

        public EntryPayloadKey Branch(string subkey)
        {
            return new EntryPayloadKey(key + NamespaceSeparator + subkey);
        }

        public override bool Equals(object? obj)
        {
            return obj is EntryPayloadKey key &&
                   this.key == key.key;
        }

        public override int GetHashCode()
        {
            return key.GetHashCode();
        }

        public override string ToString()
        {
            return key;
        }
    }
}
