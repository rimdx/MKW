// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Exceptions;

namespace MKW.Core
{
    internal static class EntryPayloadKeyParser
    {
        public const char NamespaceSeparator = ':';

        private const string AllowedCharacters =
            "0123456789" +
            "abcdefghijklmnopqrstuvwxyz" +
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
            " !\"#$%&'()*+,-./;<=>?@[\\]^_`{|}~";

        public static string ParseKeyComponent(string component)
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

            return component;
        }

        public static string ParseKey(string key)
        {
            string[] oldComponents = key.Split(NamespaceSeparator);
            List<string> newComponents = new List<string>(oldComponents.Length);

            foreach (string component in oldComponents)
            {
                newComponents.Add(ParseKeyComponent(component));
            }

            return string.Join(NamespaceSeparator.ToString(), newComponents);
        }
    }
}
