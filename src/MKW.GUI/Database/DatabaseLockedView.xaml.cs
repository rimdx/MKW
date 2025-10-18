// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Windows.Controls;

namespace MKW.GUI.Database
{
    public partial class DatabaseLockedView : UserControl
    {
        private readonly DatabaseLockedViewModel model;

        public DatabaseLockedView(DatabaseLockedViewModel model)
        {
            this.model = model;
            DataContext = model;

            InitializeComponent();
        }
    }
}
