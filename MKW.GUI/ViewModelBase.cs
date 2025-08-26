using System.ComponentModel;
using System.Windows;

namespace MKW.GUI
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void ReportError(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void RunAction(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }
    }
}
