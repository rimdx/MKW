// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.Wizard;

namespace MKW.GUI.ImportWizard
{
    public partial class PageWelcome : WizardPage
    {
        private readonly ImportWizardViewModel viewModel;

        public PageWelcome(ImportWizardViewModel viewModel)
            : base("Welcome")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
