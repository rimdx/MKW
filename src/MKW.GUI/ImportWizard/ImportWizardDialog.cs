// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.ImportWizard
{
    public class ImportWizardDialog : WizardWindow
    {
        private readonly ImportWizardViewModel viewModel;

        public ImportWizardDialog(ImportWizardViewModel viewModel, Window owner)
            : base(viewModel, owner)
        {
            this.viewModel = viewModel;
        }
    }
}
