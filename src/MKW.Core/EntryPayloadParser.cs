using MKW.Core.Exceptions;

namespace MKW.Core
{
    internal static class EntryPayloadParser
    {
        public const char NamespaceSeparator = ':';

        private const string AllowedCharacters =
            "0123456789" +
            "abcdefghijklmnopqrstuvwxyz" +
            "_";

        public static void ValidateKeyComponent(string component)
        {
            if (component == string.Empty)
            {
                throw new InvalidEntryPayloadKey("empty component found.");
            }

            foreach (char c in component)
            {
                if (!AllowedCharacters.Contains(c))
                {
                    throw new InvalidEntryPayloadKey($"only Latin letters, numbers, and underscores are allowed.");
                }
            }
        }

        public static string ParseKey(string key)
        {
            string lowered = key.ToLower();

            string[] components = lowered.Split(NamespaceSeparator);

            foreach (string component in components)
            {
                ValidateKeyComponent(component);
            }

            return string.Join(NamespaceSeparator.ToString(), components);
        }
    }
}
