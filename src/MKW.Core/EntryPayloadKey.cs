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

        public bool IsInstance(EntryPayloadKey other)
        {
            // key:     mkw:custom
            // dir:     mkw:custom:
            // other:   mkw:custom:mycustomproperty

            string dir = key + NamespaceSeparator;

            return other.key.StartsWith(dir);
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
