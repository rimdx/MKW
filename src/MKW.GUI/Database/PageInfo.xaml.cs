// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Windows.Controls;

namespace MKW.GUI.Database
{
    public partial class PageInfo : UserControl
    {
        private readonly DatabaseUnlockedViewModel model;

        public PageInfo(DatabaseUnlockedViewModel model)
        {
            this.model = model;
            DataContext = this;
            InitializeComponent();
        }
    }
}
