// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.GUI.Backup
{
    public static class CommonBackupFormats
    {
        public static KeePassXmlV1BackupFormat KeePassXmlV1 { get; } = new KeePassXmlV1BackupFormat();
        public static KeePassXmlV2BackupFormat KeePassXmlV2 { get; } = new KeePassXmlV2BackupFormat();
        public static KeePassCSVBackupFormat KeePassCSV { get; } = new KeePassCSVBackupFormat();
    }
}
