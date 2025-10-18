// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace MKW.GUI
{
    public class RecentFileItemViewModel
    {
        public string FullPath { get; }
        public string FileName => Path.GetFileName(FullPath);

        public string FileNameWithFullPath => $"{FileName} ({FullPath})";

        public RecentFileItemViewModel(string fullPath)
        {
            FullPath = fullPath;
        }
    }
}
