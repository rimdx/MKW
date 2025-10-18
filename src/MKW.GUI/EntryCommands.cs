// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Windows.Input;

namespace MKW.GUI
{
    internal class EntryCommands
    {
        public static RoutedUICommand AddEntry { get; } = new RoutedUICommand();
        public static RoutedUICommand DeleteEntry { get; } = new RoutedUICommand();
        public static RoutedUICommand EditEntry { get; } = new RoutedUICommand();
    }
}
