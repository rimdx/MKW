// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.Wizard;

namespace MKW.GUI.ExportWizard
{
    public partial class PageCompleted : WizardPage
    {
        private readonly ExportWizardViewModel viewModel;

        public PageCompleted(ExportWizardViewModel viewModel)
            : base("Completed")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
