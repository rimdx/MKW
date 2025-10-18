// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.Wizard;

namespace MKW.GUI.ExportWizard
{
    public partial class PageEntries : WizardPage
    {
        private readonly ExportWizardViewModel viewModel;

        public PageEntries(ExportWizardViewModel viewModel)
            : base("Pick Entries")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
