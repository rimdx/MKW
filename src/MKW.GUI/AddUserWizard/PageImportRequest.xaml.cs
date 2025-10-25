// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Microsoft.Win32;
using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.AddUserWizard
{
    public partial class PageImportRequest : WizardPage
    {
        private readonly AddUserWizardViewModel viewModel;

        public PageImportRequest(AddUserWizardViewModel viewModel)
            : base("Import User Request")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void ImportFromFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    Filter = "MKW Access Request Files (*.mkwreq)|*.mkwreq|All files (*.*)|*",
                    DefaultExt = ".mkwreq"
                };

                if (dialog.ShowDialog() == true)
                {
                    viewModel.LoadFromFile(dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        public override bool Next()
        {
            try
            {
                viewModel.ParseAccessRequest();
                return true;
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
                return false;
            }
        }
    }
}
