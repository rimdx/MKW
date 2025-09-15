using System.Windows;

namespace MKW.GUI
{
    public static class ErrorReporter
    {
        public static void HandleException(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
