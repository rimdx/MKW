using System.Windows;
using System.Windows.Controls;

namespace MKW.GUI.Images
{
    public class CrispImage : Viewbox
    {
        public static readonly DependencyProperty MonikerProperty = DependencyProperty.Register(
            nameof(Moniker),
            typeof(Moniker),
            typeof(CrispImage),
            new PropertyMetadata(Moniker.None, OnMonikerChanged));

        public Moniker Moniker
        {
            get => (Moniker)GetValue(MonikerProperty);
            set => SetValue(MonikerProperty, value);
        }

        public CrispImage()
            : base()
        {
        }

        private static void OnMonikerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CrispImage image = (CrispImage)d;
            Moniker oldValue = (Moniker)e.OldValue;
            Moniker newValue = (Moniker)e.NewValue;

            if (oldValue != newValue)
            {
                image.Child = MakeImage(newValue);
            }
        }

        private static Viewbox? MakeImage(Moniker moniker) => moniker switch
        {
            Moniker.None => null,
            Moniker.AddDatabase => new AddDatabase(),
            Moniker.AddUser => new AddUser(),
            Moniker.Admin => new Admin(),
            Moniker.AsymmetricKey => new AsymmetricKey(),
            Moniker.Close => new Close(),
            Moniker.Database => new Database(),
            Moniker.DatabaseFile => new DatabaseFile(),
            Moniker.DataList => new DataList(),
            Moniker.Delete => new Delete(),
            Moniker.DeleteDocument => new DeleteDocument(),
            Moniker.DeleteFolder => new DeleteFolder(),
            Moniker.Edit => new Edit(),
            Moniker.EditDatabase => new EditDatabase(),
            Moniker.EditDocument => new EditDocument(),
            Moniker.EditKey => new EditKey(),
            Moniker.FolderClosed => new FolderClosed(),
            Moniker.FolderOpened => new FolderOpened(),
            Moniker.Key => new Key(),
            Moniker.LoginUser => new LoginUser(),
            Moniker.NewDocument => new NewDocument(),
            Moniker.NewFolder => new NewFolder(),
            Moniker.NewUser => new NewUser(),
            Moniker.OpenFile => new OpenFile(),
            Moniker.OpenFolder => new OpenFolder(),
            Moniker.PasswordBox => new PasswordBox(),
            Moniker.PasswordStrength => new PasswordStrength(),
            Moniker.Save => new Save(),
            Moniker.SaveAs => new SaveAs(),
            Moniker.StatusHelp => new StatusHelp(),
            Moniker.StatusOK => new StatusOK(),
            Moniker.StatusWarning => new StatusWarning(),
            Moniker.Team => new Team(),
            Moniker.User => new User(),
        };
    }
}
