// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Windows;

namespace MKW.GUI
{
    public static class ErrorReporter
    {
        public static void HandleException(Window owner, Exception ex)
        {
            MessageBox.Show(owner, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
