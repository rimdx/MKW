// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MKW.GUI.Controls
{
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(SelectorBoxItem))]
    public sealed class SelectorBox : Selector
    {
        public SelectorBox()
            : base()
        {
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new SelectorBoxAutomationPeer(this);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new SelectorBoxItem();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
        }

        internal void SelectItemInternal(SelectorBoxItem control)
        {
            object item = ItemContainerGenerator.ItemFromContainer(control);
            SelectedItem = item;
        }
    }
}
