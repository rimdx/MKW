// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.Model;
using System.Windows;
using System.Windows.Controls;

namespace MKW.GUI.Database
{
    public partial class EntryCellView : TextBlock
    {
        public static readonly DependencyProperty PropertyInfoProperty = 
            DependencyProperty.Register(nameof(PropertyInfo),
                                        typeof(PropertyInfo),
                                        typeof(EntryCellView));

        public EntryCellView()
        {
            InitializeComponent();
        }

        public PropertyInfo PropertyInfo
        {
            get => (PropertyInfo)GetValue(PropertyInfoProperty);
            set => SetValue(PropertyInfoProperty, value);
        }
    }
}
