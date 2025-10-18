// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.Wizard;

namespace MKW.GUI.AddUserWizard
{
    public partial class PageCompleted : WizardPage
    {
        private readonly AddUserWizardViewModel viewModel;

        public PageCompleted(AddUserWizardViewModel viewModel)
            : base("Completed")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
