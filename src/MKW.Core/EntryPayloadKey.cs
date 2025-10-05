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

        public static bool IsInstance(EntryPayloadKey parent, EntryPayloadKey key)
        {
            // parent:  mkw:custom
            // dir:     mkw:custom:
            // key:     mkw:custom:mycustomproperty

            string dir = parent.key + NamespaceSeparator;

            return key.key.StartsWith(dir);
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
