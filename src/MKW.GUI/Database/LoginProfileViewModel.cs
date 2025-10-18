// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.Model;
using MKW.GUI.RequestAccessWizard;

namespace MKW.GUI.Database
{
    public class LoginProfileViewModel : ViewModelBase
    {
        private readonly DatabaseLockedViewModel database;
        private readonly DatabaseUserModel user;

        public LoginProfileViewModel(DatabaseLockedViewModel database, DatabaseUserModel user)
        {
            this.database = database;
            this.user = user;
        }

        private string password = "";
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public void Login()
        {
            database.Database.Database.Unlock(user.Id, Password);
        }

        public RequestAccessWizardViewModel CreateRequestAccessViewModel()
        {
            return database.Database.CreateRequestAccessViewModel();
        }

        public string LoginUserName => user.LoginUserName;
    }
}
