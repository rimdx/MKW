// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.EntryEditor;
using MKW.GUI.ExportWizard;
using MKW.GUI.ImportWizard;
using MKW.GUI.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MKW.GUI.Database
{
    public partial class DatabaseUnlockedView : UserControl
    {
        private readonly DatabaseUnlockedViewModel model;

        public DatabaseUnlockedView(DatabaseUnlockedViewModel model)
        {
            this.model = model;
            DataContext = model;

            InitializeComponent();

            //InfoPage.Content = new PageInfo(model);
            //EntriesPage.Content = ;
            //UsersPage.Content = ;
        }

        private void AddEntryCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                NewEntryWindowViewModel viewModel = model.CreateNewEntryWindowViewModel();
                NewEntryWindow window = new NewEntryWindow(viewModel, Window.GetWindow(this));
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void EditEntryCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = model.SelectedEntry != null;
        }

        private void EditEntryCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (model.SelectedEntry != null)
                {
                    EditEntryWindowViewModel viewModel = model.CreateEditEntryWindowViewModel(model.SelectedEntry.Id);
                    EditEntryWindow window = new EditEntryWindow(viewModel, Window.GetWindow(this));
                    window.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void DeleteEntryCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = model.SelectedEntry != null;
        }

        private void DeleteEntryCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (model.SelectedEntry != null)
                {
                    MessageBoxResult result = MessageBox.Show(Window.GetWindow(this),
                                                          "Are you sure you want to delete this entry?",
                                                          "Confirm Deletion",
                                                          MessageBoxButton.OKCancel);

                    if (result == MessageBoxResult.OK)
                    {
                        model.DeleteEntry(model.SelectedEntry);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void Import_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                using ImportWizardViewModel dialogModel = model.CreateImportViewModel();
                ImportWizardDialog dialog = new ImportWizardDialog(dialogModel, Window.GetWindow(this));
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void Export_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                using ExportWizardViewModel dialogModel = model.CreateExportViewModel();
                ExportWizardDialog dialog = new ExportWizardDialog(dialogModel, Window.GetWindow(this));
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private static bool CanCopyProperty(EntryListViewModel? entry, PropertyInfo property)
        {
            if (entry == null)
            {
                return false;
            }

            EntryValueModel entryValue = entry.EntryEditorModel.Properties[property.Key];
            if (entryValue.Value == "")
            {
                return false;
            }

            return true;
        }

        private static void CopyProperty(EntryListViewModel? entry, PropertyInfo property)
        {
            if (entry == null)
            {
                return;
            }

            EntryValueModel entryValue = entry.EntryEditorModel.Properties[property.Key];

            Clipboard.SetText(entryValue.Value);
        }

        private void CopyPropertyCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            PropertyInfo property = (PropertyInfo)e.Parameter;

            e.CanExecute = CanCopyProperty(model.SelectedEntry, property);
        }

        private void CopyPropertyCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PropertyInfo property = (PropertyInfo)e.Parameter;

            CopyProperty(model.SelectedEntry, property);
        }
    }
}
