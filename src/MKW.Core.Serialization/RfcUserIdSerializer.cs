// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Text;

namespace MKW.Core.Serialization
{
    // https://www.rfc-editor.org/rfc/rfc4880#section-5.11
    // 
    // Example:
    //   uwu-test-id (use for mkw testing) <test@uwu-mail.com>
    public static class RfcUserIdSerializer
    {
        public static string Serialize(RfcUserId obj)
        {
            StringBuilder result = new StringBuilder();

            result.Append(Verify(obj.Name));

            if (obj.Comment != null)
            {
                result.Append($" ({Verify(obj.Comment)})");
            }

            if (obj.Email != null)
            {
                result.Append($" <{Verify(obj.Email)}>");
            }

            return result.ToString();
        }

        private static string Verify(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];

                if (ch == '(' || ch == ')' ||
                    ch == '<' || ch == '>')
                {
                    throw new RfcUserIdContainsIllegalCharactersException();
                }
            }

            return str.Trim();
        }

        public static RfcUserId Deserialize(string str)
        {
            using StringReader reader = new StringReader(str);

            StringBuilder name = new StringBuilder();
            string? comment = null;
            string? email = null;

            while (true)
            {
                int b = reader.Read();

                if (b == -1)
                {
                    break;
                }

                char ch = (char)b;

                if (ch == '(')
                {
                    if (comment != null)
                    {
                        throw new RfcUserIdMalformedException();
                    }

                    // comment can't follow after email
                    if (email != null)
                    {
                        throw new RfcUserIdMalformedException();
                    }

                    comment = ReadUntil(reader, ')');
                }
                else if (ch == '<')
                {
                    if (email != null)
                    {
                        throw new RfcUserIdMalformedException();
                    }

                    email = ReadUntil(reader, '>');
                }
                else
                {
                    // name can't continue after comment or email
                    if (comment != null || email != null)
                    {
                        if (!char.IsWhiteSpace(ch))
                        {
                            throw new RfcUserIdMalformedException();
                        }
                    }
                    else
                    {
                        name.Append(ch);
                    }
                }
            }

            if (comment != null)
            {
                comment = comment.Trim();
            }

            if (email != null)
            {
                email = email.Trim();
            }

            return new RfcUserId
            {
                Name = name.ToString().Trim(),
                Comment = comment,
                Email = email,
            };
        }

        private static string ReadUntil(TextReader reader, char stopOn)
        {
            StringBuilder result = new StringBuilder();

            while (true)
            {
                int b = reader.Read();

                if (b == -1)
                {
                    throw new RfcUserIdMalformedException();
                }

                char ch = (char)b;

                if (ch == stopOn)
                {
                    return result.ToString();
                }
                else
                {
                    result.Append(ch);
                }
            }
        }
    }
}
