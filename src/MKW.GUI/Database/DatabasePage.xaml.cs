using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MKW.GUI.Database
{
    public partial class DatabasePage : UserControl
    {
        private readonly DatabaseViewModel model;

        public DatabasePage(DatabaseViewModel model)
        {
            this.model = model;
            DataContext = model;
            model.PropertyChanged += Model_PropertyChanged;

            InitializeComponent();

            ContentView.Content = CreateContentView();
        }

        private object CreateContentView()
        {
            if (model.ContentView is DatabaseLockedViewModel lockedViewModel)
            {
                return new DatabaseLockedView(lockedViewModel);
            }
            else if (model.ContentView is DatabaseUnlockedViewModel unlockedViewModel)
            {
                return new DatabaseUnlockedView(unlockedViewModel);
            }
            else
            {
                throw new Exception($"Unknown type: {model.ContentView}");
            }
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.MatchProperty(nameof(model.ContentView)))
            {
                ContentView.Content = CreateContentView();
            }
        }

        private void LockDatabaseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = model.Database.UnlockedDatabase != null;
        }

        private void LockDatabaseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                model.LockDatabase();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void ReloadDatabase_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                model.ReloadDatabaseFile();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }
    }
}
