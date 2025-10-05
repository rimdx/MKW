using MKW.Core.Exceptions;

namespace MKW.Core
{
    internal static class EntryPayloadKeyParser
    {
        public const char NamespaceSeparator = ':';

        private const string AllowedCharacters =
            "0123456789" +
            "abcdefghijklmnopqrstuvwxyz" +
            "_";

        public static string ParseKeyComponent(string component)
        {
            string lowered = component.ToLower();

            if (lowered == string.Empty)
            {
                throw new InvalidEntryPayloadKey("empty component found.");
            }

            foreach (char c in lowered)
            {
                if (!AllowedCharacters.Contains(c))
                {
                    throw new InvalidEntryPayloadKey($"only Latin letters, numbers, and underscores are allowed.");
                }
            }

            return lowered;
        }

        public static string ParseKey(string key)
        {
            string lowered = key.ToLower();

            string[] oldComponents = lowered.Split(NamespaceSeparator);
            List<string> newComponents = new List<string>(oldComponents.Length);

            foreach (string component in oldComponents)
            {
                newComponents.Add(ParseKeyComponent(component));
            }

            return string.Join(NamespaceSeparator.ToString(), newComponents);
        }
    }
}
