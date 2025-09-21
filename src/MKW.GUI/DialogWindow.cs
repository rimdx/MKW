using System.Windows;

namespace MKW.GUI
{
    public partial class DialogWindow : Window
    {
        public DialogWindow(Window owner)
        {
            ShowInTaskbar = false;
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
    }
}
