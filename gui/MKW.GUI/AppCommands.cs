using System.Windows.Input;

namespace MKW.GUI
{
    internal static class AppCommands
    {
        public static RoutedUICommand OpenRecent { get; } = new RoutedUICommand();
        public static RoutedUICommand Exit { get; } = new RoutedUICommand();
    }
}
