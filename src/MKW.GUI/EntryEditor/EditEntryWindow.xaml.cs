// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Windows;

namespace MKW.GUI.EntryEditor
{
    public partial class EditEntryWindow : DialogWindow
    {
        private readonly EditEntryWindowViewModel model;

        public EditEntryWindow(EditEntryWindowViewModel model, Window owner) : base(owner)
        {
            this.model = model;
            DataContext = model;
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                model.OnOK();
                Close();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }
    }
}
