// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MKW.GUI.Database
{
    public partial class DatabasePage : UserControl
    {
        private readonly DatabaseViewModel model;

        public DatabasePage(DatabaseViewModel model)
        {
            this.model = model;
            DataContext = model;

            InitializeComponent();
        }

        private void LockDatabaseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = model.Database.UnlockedDatabase != null;
        }

        private void LockDatabaseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                model.LockDatabase();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void ReloadDatabase_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                model.ReloadDatabaseFile();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }
    }
}
