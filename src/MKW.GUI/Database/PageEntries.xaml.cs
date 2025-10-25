// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.EntryEditor;
using MKW.GUI.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MKW.GUI.Database
{
    public partial class PageEntries : UserControl
    {
        private readonly DatabaseUnlockedViewModel viewModel;

        public PageEntries(DatabaseUnlockedViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();

            // TODO: Factor-out into a separate object.
            foreach (EntryListColumn column in viewModel.Columns)
            {
                GridViewColumn gridViewColumn = new GridViewColumn();

                BindingOperations.SetBinding(gridViewColumn,
                                             GridViewColumn.HeaderProperty,
                                             new Binding(nameof(column.Header))
                                             {
                                                 Source = column,
                                                 Mode = BindingMode.OneWay
                                             });

                BindingOperations.SetBinding(gridViewColumn,
                                             GridViewColumn.WidthProperty,
                                             new Binding(nameof(column.Width))
                                             {
                                                 Source = column,
                                                 Mode = BindingMode.TwoWay
                                             });

                gridViewColumn.CellTemplate = MakeCellTemplate(column);

                gridView.Columns.Add(gridViewColumn);
            }
        }

        public string HiddenValueText => "********";

        private void EntryListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (viewModel.SelectedEntry != null)
                {
                    EditEntryWindowViewModel editEntryViewModel = viewModel.CreateEditEntryWindowViewModel(viewModel.SelectedEntry.Id);
                    EditEntryWindow window = new EditEntryWindow(editEntryViewModel, Window.GetWindow(this));
                    window.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }
        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
        }

        private DataTemplate MakeCellTemplate(EntryListColumn column)
        {
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(EntryCellView));

            factory.SetBinding(TextBlock.TextProperty, MakeDisplayMemberBinding(column));
            factory.SetValue(EntryCellView.PropertyInfoProperty, column.Property);

            return new DataTemplate()
            {
                VisualTree = factory
            };
        }

        private BindingBase MakeDisplayMemberBinding(EntryListColumn column)
        {
            if (column.HideValue)
            {
                return new Binding(nameof(HiddenValueText))
                {
                    Source = this
                };
            }
            else
            {
                return new Binding()
                {
                    Path = new PropertyPath($"{nameof(EntryListViewModel.EntryEditorModel)}.{nameof(EntryEditorModel.Properties)}[(0)].{nameof(EntryValueModel.DisplayValue)}", column.PropertyKey),
                    Converter = SingleLineConverter.Instance,
                };
            }
        }
    }
}
