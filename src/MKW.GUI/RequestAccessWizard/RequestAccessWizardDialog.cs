// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.RequestAccessWizard
{
    public class RequestAccessWizardDialog : WizardWindow
    {
        public RequestAccessWizardDialog(RequestAccessWizardViewModel viewModel, Window owner)
            : base(viewModel, owner)
        {
        }
    }
}
