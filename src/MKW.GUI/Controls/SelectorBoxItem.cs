// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MKW.GUI.Controls
{
    public class SelectorBoxItem : ButtonBase
    {
        public static readonly DependencyProperty IsSelectedProperty =
            Selector.IsSelectedProperty.AddOwner(
                typeof(SelectorBoxItem),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                    | FrameworkPropertyMetadataOptions.Journal,
                    OnIsSelectedChanged));

        private SelectorBox ParentSelectorBox
        {
            get
            {
                ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(this);
                return (SelectorBox)parent;
            }
        }

        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        public SelectorBoxItem()
        {
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectorBoxItem item = (SelectorBoxItem)d;
            item.IsSelected = (bool)e.NewValue;
        }

        protected virtual void OnSelected()
        {
            VisualStateManager.GoToState(this, "Selected", true);
        }

        protected virtual void OnUnselected()
        {
            VisualStateManager.GoToState(this, "Unselected", true);
        }

        protected override void OnClick()
        {
            base.OnClick();
            ParentSelectorBox.SelectItemInternal(this);
        }
    }
}
