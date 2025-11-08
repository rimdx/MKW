// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.GUI.Images;
using MKW.GUI.Services;
using MKW.GUI.Wizard;
using System.IO;

namespace MKW.GUI.CreateDatabaseWizard
{
    public class CreateDatabaseWizardViewModel : WizardViewModel
    {
        public PasswordViewModel Password { get; }

        private readonly MainWindowViewModel mainWindowViewModel;
        private readonly RegistryService registry;

        public CreateDatabaseWizardViewModel(MainWindowViewModel mainWindowViewModel, RegistryService registry)
            : base("Create New Database", ImageMoniker.AddDatabase)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            this.registry = registry;

            databaseDirectory = registry.GetLastDatabaseDirectory();
            databaseName = "New Database";
            userName = "";
            userDisplayName = "";

            Password = new PasswordViewModel();

            AddPage(new PageLocation(this));
            AddPage(new PageMasterPassword(this));
            AddPage(new PageUserDetails(this));
            AddPage(new PageConfirmation(this));
            AddPage(new PageCompleted(this));
        }

        public string DatabasePath
        {
            get
            {
                string databaseFilename;

                if (string.Equals(Path.GetExtension(databaseName), ".mkw", StringComparison.InvariantCultureIgnoreCase))
                {
                    databaseFilename = databaseName;
                }
                else
                {
                    databaseFilename = databaseName + ".mkw";
                }

                return Path.Combine(DatabaseDirectory, databaseFilename);
            }
        }

        private string databaseName;
        public string DatabaseName
        {
            get => databaseName;
            set
            {
                if (SetProperty(ref databaseName, value))
                {
                    OnPropertyChanged(nameof(DatabasePath));
                }
            }
        }

        private string databaseDirectory;

        public string DatabaseDirectory
        {
            get => databaseDirectory;
            set
            {
                if (SetProperty(ref databaseDirectory, value))
                {
                    DirectoryInfo info = new DirectoryInfo(value);

                    if (info.Exists)
                    {
                        registry.SetLastDatabaseDirectory(info.FullName);
                    }

                    OnPropertyChanged(nameof(DatabasePath));
                }
            }
        }

        public bool Exists()
        {
            return File.Exists(DatabasePath);
        }

        public void DoCreate()
        {
            UserMetadata userMetadata = new UserMetadata
            {
                DisplayName = userDisplayName,
                UserId = userName
            };

            mainWindowViewModel.CreateDatabase(DatabasePath, Password.Password, userMetadata);
        }

        public void VerifyDetails()
        {
            if (UserName.Length == 0)
            {
                throw new Exception("User ID cannot be empty.");
            }
        }

        private string userName;
        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        private string userDisplayName;
        public string UserDisplayName
        {
            get => userDisplayName;
            set => SetProperty(ref userDisplayName, value);
        }
    }
}
