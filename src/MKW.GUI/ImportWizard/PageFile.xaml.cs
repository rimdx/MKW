// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Microsoft.Win32;
using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.ImportWizard
{
    public partial class PageFile : WizardPage
    {
        private readonly ImportWizardViewModel viewModel;

        public PageFile(ImportWizardViewModel viewModel)
            : base("Choose File")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void PickFile_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            try
            {
                FileDialog dialog = FileDialogUtils.CreateOpenBackupDialog(viewModel.BackupFormat);

                if (dialog.ShowDialog() == true)
                {
                    viewModel.Path = dialog.FileName;
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
                viewModel.OpenBackup();
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
