using System.Windows;

namespace MKW.GUI
{
    public static class ErrorReporter
    {
        public static void HandleException(Window owner, Exception ex)
        {
            MessageBox.Show(owner, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
