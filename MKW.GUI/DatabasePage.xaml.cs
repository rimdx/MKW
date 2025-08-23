using MKW.Core.Client;
using System.Windows.Controls;

namespace MKW.GUI
{
    public partial class DatabasePage : UserControl
    {
        private readonly DatabaseModel model;

        public DatabasePage(ClientSession client, UserSession user)
        {
            model = new DatabaseModel(client, user);
            DataContext = model;
            InitializeComponent();
        }
    }
}
