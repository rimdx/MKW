// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.GUI.AddUserWizard;
using MKW.GUI.EntryEditor;
using MKW.GUI.ExportWizard;
using MKW.GUI.ImportWizard;
using MKW.GUI.Model;
using MKW.GUI.RequestAccessWizard;
using System.Collections.ObjectModel;

namespace MKW.GUI.Database
{
    public class DatabaseUnlockedViewModel : ViewModelBase
    {
        private readonly DatabaseUnlockedModel database;

        public DatabaseUnlockedViewModel(DatabaseUnlockedModel database)
        {
            this.database = database;

            Columns =
            [
                new EntryListColumn("Title", 175, "mkw:title"),
                new EntryListColumn("User Name", 175, "mkw:username"),
                new EntryListColumn("Password", 150, "mkw:password")
                {
                    HideValue = true,
                },
                new EntryListColumn("URL", 150, "mkw:url"),
                new EntryListColumn("Notes", 225, "mkw:notes"),
            ];

            Entries = new DatabaseEntryCollectionViewModel(database);
            Users = new DatabaseUserCollectionViewModel(database.Database);
        }

        private enum PageType
        {
            Database,
            Entries,
            Users,
        }

        private PageType selectedPage = PageType.Entries;
        private void SetPage(bool value, PageType type)
        {
            if (value)
            {
                selectedPage = type;
                OnPropertyChanged(nameof(IsPageDatabase));
                OnPropertyChanged(nameof(IsPageEntries));
                OnPropertyChanged(nameof(IsPageUsers));
            }
        }

        public bool IsPageDatabase
        {
            get => selectedPage == PageType.Database;
            set => SetPage(value, PageType.Database);
        }

        public bool IsPageEntries
        {
            get => selectedPage == PageType.Entries;
            set => SetPage(value, PageType.Entries);
        }

        public bool IsPageUsers
        {
            get => selectedPage == PageType.Users;
            set => SetPage(value, PageType.Users);
        }

        public DatabaseEntryCollectionViewModel Entries { get; }

        private EntryListViewModel? _selectedEntry;
        public EntryListViewModel? SelectedEntry
        {
            get => _selectedEntry;
            set
            {
                SetProperty(ref _selectedEntry, value);
                OnPropertyChanged(nameof(IsEntrySelected));
            }
        }

        public bool IsEntrySelected => _selectedEntry != null;

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

        private DatabaseUnlockedView unlockedViewNeededToBindTheCommand;
        public DatabaseUnlockedView UnlockedViewNeededToBindTheCommand
        {
            get => unlockedViewNeededToBindTheCommand;
            set => SetProperty(ref unlockedViewNeededToBindTheCommand, value);
        }

        public void Dispose()
        {
            Entries.Dispose();
            Users.Dispose();
        }
    }
}
