// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.Images;
using System.Windows;
using System.Windows.Controls;

namespace MKW.GUI
{
    public class CrispImage : Viewbox
    {
        public static readonly DependencyProperty MonikerProperty = DependencyProperty.Register(
            nameof(Moniker),
            typeof(ImageMoniker),
            typeof(CrispImage),
            new PropertyMetadata(ImageMoniker.None, OnMonikerChanged));

        public ImageMoniker Moniker
        {
            get => (ImageMoniker)GetValue(MonikerProperty);
            set => SetValue(MonikerProperty, value);
        }

        public CrispImage()
            : base()
        {
        }

        private static void OnMonikerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CrispImage image = (CrispImage)d;
            ImageMoniker oldValue = (ImageMoniker)e.OldValue;
            ImageMoniker newValue = (ImageMoniker)e.NewValue;

            if (oldValue != newValue)
            {
                image.Child = ImageFactory.MakeImage(newValue);
            }
        }
    }
}
