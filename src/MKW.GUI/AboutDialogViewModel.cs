// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.GUI
{
    internal sealed class AboutDialogViewModel : ViewModelBase
    {
        public string Version { get; }

        public AboutDialogViewModel()
        {
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!.ToString();
        }
    }
}
