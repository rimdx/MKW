// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Diagnostics;
using System.Windows;

namespace MKW.GUI
{
    internal sealed partial class AboutDialog : DialogWindow
    {
        private readonly AboutDialogViewModel model;

        public AboutDialog(AboutDialogViewModel model, Window owner) : base(owner)
        {
            this.model = model;
            DataContext = model;
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GitHubLinkClick(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/rimdx/MKW",
                UseShellExecute = true
            });
        }
    }
}
