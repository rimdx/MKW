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
        private readonly DatabaseUnlockedViewModel model;

        public PageEntries(DatabaseUnlockedViewModel model)
        {
            this.model = model;
            DataContext = model;
            InitializeComponent();

            foreach (EntryListColumn column in model.Columns)
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
                if (model.SelectedEntry != null)
                {
                    using EditEntryWindowViewModel viewModel = model.CreateEditEntryWindowViewModel(model.SelectedEntry.Id);
                    EditEntryWindow window = new EditEntryWindow(viewModel, Window.GetWindow(this));
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
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));

            factory.SetBinding(TextBlock.TextProperty, MakeDisplayMemberBinding(column));
            factory.SetValue(TextBlock.TextWrappingProperty, TextWrapping.NoWrap);
            factory.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);

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
                    Path = new PropertyPath($"{nameof(EntryListViewModel.EntryEditorModel)}.{nameof(EntryEditorModel.Properties)}[(0)].{nameof(EntryValueModel.DisplayValue)}", column.PropertyName),
                    Converter = SingleLineConverter.Instance,
                };
            }
        }
    }
}
