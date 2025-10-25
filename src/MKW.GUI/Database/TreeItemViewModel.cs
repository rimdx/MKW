// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.Images;
using System.Collections.ObjectModel;

namespace MKW.GUI.Database
{
    public class TreeItemViewModel : ViewModelBase
    {
        public string Header { get; set; }
        public ObservableCollection<TreeItemViewModel> Children { get; }
        public ImageMoniker Icon { get; }
        public object ResultView { get; }
        private bool isExpanded;
        private bool isSelected;

        public TreeItemViewModel(string header, ImageMoniker icon, object resultView)
        {
            Header = header;
            Icon = icon;
            ResultView = resultView;
            Children = [];
            isExpanded = true;
            isSelected = false;
        }

        public bool IsExpanded 
        {
            get => isExpanded;
            set => SetProperty(ref isExpanded, value);
        }

        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }
    }
}
