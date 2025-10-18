// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.Wizard;

namespace MKW.GUI.ExportWizard
{
    public partial class PageFormat : WizardPage
    {
        private readonly ExportWizardViewModel viewModel;

        public PageFormat(ExportWizardViewModel viewModel)
            : base("Choose Format")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
