// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.GUI.AddUserWizard;
using MKW.GUI.EntryEditor;
using MKW.GUI.ExportWizard;
using MKW.GUI.Images;
using MKW.GUI.ImportWizard;
using MKW.GUI.Model;
using MKW.GUI.RequestAccessWizard;
using System.Collections.ObjectModel;

namespace MKW.GUI.Database
{
    public class DatabaseUnlockedViewModel : ViewModelBase
    {
        private readonly DatabaseUnlockedModel database;
        public IReadOnlyList<TreeItemViewModel> TreeViewItems { get; }
        private TreeItemViewModel treeRootItem;

        public DatabaseUnlockedViewModel(DatabaseUnlockedModel database)
        {
            this.database = database;

            Columns =
            [
                new EntryListColumn("Title", 175, CommonEntryPropertiesModel.Title)
                {
                    CopyOnDoubleClick = false
                },
                new EntryListColumn("User Name", 175, CommonEntryPropertiesModel.Username),
                new EntryListColumn("Password", 150, CommonEntryPropertiesModel.Password)
                {
                    HideValue = true,
                },
                new EntryListColumn("URL", 150, CommonEntryPropertiesModel.Url),
                new EntryListColumn("Notes", 225, CommonEntryPropertiesModel.Notes),
            ];

            Entries = new DatabaseEntryCollectionViewModel(database);
            Users = new DatabaseUserCollectionViewModel(database.Database);

            treeRootItem = new TreeItemViewModel("Entries", ImageMoniker.AsymmetricKey, new PageEntries(this))
            {
                IsSelected = true
            };

            treeRootItem.Children.Add(new TreeItemViewModel("Users", ImageMoniker.Team, new PageUsers(this)));

            TreeViewItems = [treeRootItem];
        }

        public DatabaseEntryCollectionViewModel Entries { get; }

        private EntryListViewModel? _selectedEntry;
        public EntryListViewModel? SelectedEntry
        {
            get => _selectedEntry;
            set
            {
                SetProperty(ref _selectedEntry, value);
            }
        }

        public DatabaseUserCollectionViewModel Users { get; }

        private DatabaseUserModel? _selectedUser;
        public DatabaseUserModel? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                SetProperty(ref _selectedUser, value);
                OnPropertyChanged(nameof(IsUserSelected));
            }
        }

        public bool IsUserSelected => _selectedUser != null;

        public ObservableCollection<EntryListColumn> Columns { get; }

        // Entry

        public NewEntryWindowViewModel CreateNewEntryWindowViewModel()
        {
            return new NewEntryWindowViewModel(database);
        }

        public EditEntryWindowViewModel CreateEditEntryWindowViewModel(EntryId id)
        {
            return new EditEntryWindowViewModel(database, id);
        }

        public void DeleteEntry(EntryListViewModel entry)
        {
            database.DeleteEntry(entry.Id);
        }

        // User

        public AddUserWizardViewModel CreateNewUserWindowViewModel()
        {
            return new AddUserWizardViewModel(database);
        }

        public RequestAccessWizardViewModel CreateRequestAccessViewModel()
        {
            return new RequestAccessWizardViewModel(database.Database);
        }

        public UserPropertyDialogViewModel CreateUserPropertiesWindowViewModel()
        {
            if (SelectedUser == null)
            {
                throw new Exception("No user was selected.");
            }

            UserEditorModel userEditor = database.CreateUserEditor(SelectedUser.Id);

            return new UserPropertyDialogViewModel(database, userEditor);
        }

        public void DeleteUser()
        {
            if (SelectedUser == null)
            {
                throw new Exception("No user was selected.");
            }

            database.DeleteUser(SelectedUser.Id);
        }

        public ImportWizardViewModel CreateImportViewModel()
        {
            return new ImportWizardViewModel(database);
        }

        public ExportWizardViewModel CreateExportViewModel()
        {
            return new ExportWizardViewModel(database);
        }

        public void Dispose()
        {
            Entries.Dispose();
            Users.Dispose();
        }
    }
}
