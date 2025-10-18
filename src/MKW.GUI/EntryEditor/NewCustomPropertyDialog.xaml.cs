// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Windows;

namespace MKW.GUI.EntryEditor
{
    public partial class NewCustomPropertyDialog : DialogWindow
    {
        private readonly NewCustomPropertyViewModel viewModel;

        public NewCustomPropertyDialog(Window owner, NewCustomPropertyViewModel viewModel)
            : base(owner)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                viewModel.OnOK();
                Close();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(this, ex);
            }
        }
    }
}
