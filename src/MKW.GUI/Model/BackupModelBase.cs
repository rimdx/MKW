// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Collections.ObjectModel;

namespace MKW.GUI.Model
{
    public abstract class BackupModelBase : ViewModelBase
    {
        public ObservableCollection<BackupModelEntry> Entries { get; }

        public BackupModelBase()
        {
            Entries = [];
        }

        public void SetSelectedAll(bool isSelected)
        {
            foreach (BackupModelEntry entry in Entries)
            {
                entry.IsSelected = isSelected;
            }
        }

        public bool? GetSelectedAll()
        {
            bool? isSelected = null;

            foreach (BackupModelEntry entry in Entries)
            {
                if (isSelected is null)
                {
                    isSelected = entry.IsSelected;
                }
                else if (isSelected != entry.IsSelected)
                {
                    return null;
                }
            }

            return isSelected;
        }
    }
}
