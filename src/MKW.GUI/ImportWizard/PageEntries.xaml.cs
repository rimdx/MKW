// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.Wizard;

namespace MKW.GUI.ImportWizard
{
    public partial class PageEntries : WizardPage
    {
        private readonly ImportWizardViewModel viewModel;

        public PageEntries(ImportWizardViewModel viewModel)
            : base("Pick Entries")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
