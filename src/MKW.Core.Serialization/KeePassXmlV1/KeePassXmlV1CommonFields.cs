// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.KeePassXmlV1
{
    public static class KeePassXmlV1CommonFields
    {
        public const string Title = "title";
        public const string Username = "username";
        public const string Password = "password";
        public const string Url = "url";
        public const string Notes = "notes";

        internal static HashSet<string> KeywordFields = [
            "group",
            "title",
            "username",
            "url",
            "password",
            "notes",
            "uuid",
            "image",
            "creationtime",
            "lastmodtime",
            "lastaccesstime",
            "expiretime",
        ];
    }
}
