// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.RequestAccessWizard
{
    public partial class PageResults : WizardPage
    {
        private readonly RequestAccessWizardViewModel viewModel;

        public PageResults(RequestAccessWizardViewModel viewModel)
            : base("Database Access Request is Ready!")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(viewModel.RequestString);
        }

        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
