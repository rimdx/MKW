namespace MKW.Core
{
    public sealed class EntryPayloadKey
    {
        private readonly string key;

        public EntryPayloadKey(string key)
        {
            this.key = EntryPayloadParser.ParseKey(key);
        }

        public EntryPayloadKey Branch(string subkey)
        {
            string parsed = EntryPayloadParser.ParseKeyComponent(subkey);

            return new EntryPayloadKey(key + EntryPayloadParser.NamespaceSeparator + parsed);
        }

        public static bool IsInstance(EntryPayloadKey parent, EntryPayloadKey key)
        {
            // parent:  mkw:custom
            // dir:     mkw:custom:
            // key:     mkw:custom:mycustomproperty

            string dir = parent.key + EntryPayloadParser.NamespaceSeparator;

            return key.key.StartsWith(dir);
        }

        public static string RelativeName(EntryPayloadKey parent, EntryPayloadKey key)
        {
            // parent:  mkw:custom
            // dir:     mkw:custom:
            // key:     mkw:custom:mycustomproperty
            // result:  -----------mycustomproperty
            //          skip ^   result ^

            string dir = parent.key + EntryPayloadParser.NamespaceSeparator;

            if (!key.key.StartsWith(dir))
            {
                throw new Exception($"Key name is not relative.");
            }

            return key.key.Substring(dir.Length);
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
