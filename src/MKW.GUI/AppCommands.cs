// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Windows.Input;

namespace MKW.GUI
{
    internal static class AppCommands
    {
        public static RoutedUICommand About { get; } = new RoutedUICommand();
        public static RoutedUICommand OpenRecent { get; } = new RoutedUICommand();
        public static RoutedUICommand Exit { get; } = new RoutedUICommand();
        public static RoutedUICommand LockDatabase { get; } = new RoutedUICommand();
        public static RoutedUICommand UnlockDatabase { get; } = new RoutedUICommand();
        public static RoutedUICommand ReloadDatabase { get; } = new RoutedUICommand();
        public static RoutedUICommand Login { get; } = new RoutedUICommand();
        public static RoutedUICommand RequestAccess { get; } = new RoutedUICommand();

        public static RoutedUICommand EntryAddCustomProperty { get; } = new RoutedUICommand();
        public static RoutedUICommand EntryEditCustomProperty { get; } = new RoutedUICommand();
        public static RoutedUICommand EntryDeleteCustomProperty { get; } = new RoutedUICommand();

        public static RoutedUICommand DataImport { get; } = new RoutedUICommand();
        public static RoutedUICommand DataExport { get; } = new RoutedUICommand();

        public static RoutedUICommand PickFile { get; } = new RoutedUICommand();
    }
}
