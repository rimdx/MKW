// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.GUI
{
    internal static class Formatters
    {
        public static string FormatUserName(string userId, string displayName)
        {
            if (displayName.Length > 0)
            {
                return $"{displayName} <{userId}>";
            }
            else if (userId.Length > 0)
            {
                return userId;
            }
            else
            {
                return "<empty>";
            }
        }

        public static string FormatLoginUserName(string userId, string displayName)
        {
            List<string> lines = [];

            if (displayName.Length > 0)
            {
                lines.Add(displayName);
            }

            if (userId.Length > 0)
            {
                lines.Add($"<{userId}>");
            }
            else
            {
                lines.Add("<no user id>");
            }

            return string.Join("\n", lines);
        }

        public static string FormatShortName(string userId, string displayName)
        {
            if (userId.Length > 0)
            {
                return $"{userId}";
            }
            else
            {
                return "<no user id>";
            }
        }
    }
}
