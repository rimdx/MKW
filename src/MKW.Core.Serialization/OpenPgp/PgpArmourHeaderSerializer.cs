// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Text;

namespace MKW.Core.Serialization.OpenPgp
{
    public static class PgpArmourHeaderSerializer
    {
        public static string Serialize(PgpArmourHeader obj)
        {
            // TODO: verify
            return $"{obj.Key}: {obj.Value}";
        }

        public static PgpArmourHeader Deserialize(string str)
        {
            StringBuilder key = new StringBuilder();
            StringBuilder value = new StringBuilder();

            int i = 0;

            for (; i < str.Length; i++)
            {
                if (str[i] == ':')
                {
                    i++;
                    break;
                }
                else
                {
                    key.Append(str[i]);
                }
            }

            for (; i < str.Length; i++)
            {
                if (!char.IsWhiteSpace(str[i]))
                {
                    break;
                }
            }

            for (; i < str.Length; i++)
            {
                value.Append(str[i]);
            }

            if (key.Length == 0 || value.Length == 0)
            {
                throw new Exception("malformed header format");
            }

            return new PgpArmourHeader(key.ToString(), value.ToString());
        }
    }
}
