using System.Windows.Input;

namespace MKW.GUI
{
    internal class EntryCommands
    {
        public static RoutedUICommand AddEntry { get; } = new RoutedUICommand();
        public static RoutedUICommand DeleteEntry { get; } = new RoutedUICommand();
        public static RoutedUICommand EditEntry { get; } = new RoutedUICommand();
    }
}
