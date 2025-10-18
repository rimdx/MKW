// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace MKW.GUI
{
    internal record class CommandLineArgs
    {
        public required string[] Paths { get; init; }

        public static CommandLineArgs Parse(string[] args)
        {
            List<string> paths = [];

            foreach (string arg in args)
            {
                if (arg.StartsWith("/") || arg.StartsWith("-"))
                {
                    // option, skip for now.
                }
                else
                {
                    paths.Add(Path.GetFullPath(arg));
                }
            }

            return new CommandLineArgs
            {
                Paths = paths.ToArray()
            };
        }
    }
}
