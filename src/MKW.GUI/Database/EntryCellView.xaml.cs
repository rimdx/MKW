// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MKW.GUI.Database
{
    public partial class EntryCellView : TextBlock
    {
        public static readonly DependencyProperty PropertyInfoProperty = 
            DependencyProperty.Register(nameof(PropertyInfo),
                                        typeof(PropertyInfo),
                                        typeof(EntryCellView));

        public static readonly DependencyProperty BindDoubleClickProperty =
            DependencyProperty.Register(nameof(BindDoubleClick),
                                        typeof(bool),
                                        typeof(EntryCellView),
                                        new PropertyMetadata(false, BindDoubleClickChanged));

        private static void BindDoubleClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EntryCellView entryCellView = (EntryCellView)d;

            var newValue = (bool)e.NewValue;

            entryCellView.InputBindings.Remove(entryCellView.mouseBinding);
            if (newValue)
            {
                entryCellView.InputBindings.Add(entryCellView.mouseBinding);
            }
        }

        private readonly MouseBinding mouseBinding;

        public EntryCellView()
        {
            InitializeComponent();

            mouseBinding = new MouseBinding
            {
                MouseAction = MouseAction.LeftDoubleClick,
                Command = EntryCommands.CopyProperty,
                CommandTarget = this,
            };

            BindingOperations.SetBinding(mouseBinding,
                                         MouseBinding.CommandParameterProperty,
                                         new Binding(nameof(PropertyInfo))
                                         {
                                             Source = this,
                                             Mode = BindingMode.OneWay
                                         });
        }

        public PropertyInfo PropertyInfo
        {
            get => (PropertyInfo)GetValue(PropertyInfoProperty);
            set => SetValue(PropertyInfoProperty, value);
        }

        public bool BindDoubleClick
        {
            get => (bool)GetValue(BindDoubleClickProperty);
            set => SetValue(BindDoubleClickProperty, value);
        }
    }
}
