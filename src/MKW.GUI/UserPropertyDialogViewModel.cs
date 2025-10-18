// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.Model;
using MKW.GUI.Services;

namespace MKW.GUI
{
    public class UserPropertyDialogViewModel : ViewModelBase, IDisposable
    {
        private readonly DatabaseUnlockedModel database;
        private readonly UserEditorModel user;
        private readonly KeyFormatter keyFormatter;

        public UserPropertyDialogViewModel(DatabaseUnlockedModel database, UserEditorModel user /* move */)
        {
            this.database = database;
            this.user = user;
            keyFormatter = new KeyFormatter(50);
        }

        public string UserId => user.Id.ToString();

        public string PublicKey => keyFormatter.GetBase32String(user.PublicKey);

        public bool OnOK()
        {
            user.OnApply();
            return true;
        }

        public void Dispose()
        {
            user.Dispose();
        }
    }
}
