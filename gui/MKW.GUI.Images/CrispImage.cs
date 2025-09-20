using System.Windows;
using System.Windows.Controls;

namespace MKW.GUI.Images
{
    public class CrispImage : Viewbox
    {
        public static readonly DependencyProperty MonikerProperty = DependencyProperty.Register(
            nameof(Moniker),
            typeof(ImageMoniker),
            typeof(CrispImage),
            new PropertyMetadata(ImageMoniker.None, OnMonikerChanged));

        public ImageMoniker Moniker
        {
            get => (ImageMoniker)GetValue(MonikerProperty);
            set => SetValue(MonikerProperty, value);
        }

        public CrispImage()
            : base()
        {
        }

        private static void OnMonikerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CrispImage image = (CrispImage)d;
            ImageMoniker oldValue = (ImageMoniker)e.OldValue;
            ImageMoniker newValue = (ImageMoniker)e.NewValue;

            if (oldValue != newValue)
            {
                image.Child = MakeImage(newValue);
            }
        }

        private static Viewbox? MakeImage(ImageMoniker moniker) => moniker switch
        {
            ImageMoniker.None => null,
            ImageMoniker.AddDatabase => new AddDatabase(),
            ImageMoniker.AddUser => new AddUser(),
            ImageMoniker.Admin => new Admin(),
            ImageMoniker.AsymmetricKey => new AsymmetricKey(),
            ImageMoniker.Close => new Close(),
            ImageMoniker.Database => new Database(),
            ImageMoniker.DatabaseFile => new DatabaseFile(),
            ImageMoniker.DataList => new DataList(),
            ImageMoniker.Delete => new Delete(),
            ImageMoniker.DeleteDocument => new DeleteDocument(),
            ImageMoniker.DeleteFolder => new DeleteFolder(),
            ImageMoniker.Edit => new Edit(),
            ImageMoniker.EditDatabase => new EditDatabase(),
            ImageMoniker.EditDocument => new EditDocument(),
            ImageMoniker.EditKey => new EditKey(),
            ImageMoniker.FolderClosed => new FolderClosed(),
            ImageMoniker.FolderOpened => new FolderOpened(),
            ImageMoniker.Key => new Key(),
            ImageMoniker.LoginUser => new LoginUser(),
            ImageMoniker.NewDocument => new NewDocument(),
            ImageMoniker.NewFolder => new NewFolder(),
            ImageMoniker.NewUser => new NewUser(),
            ImageMoniker.OpenFile => new OpenFile(),
            ImageMoniker.OpenFolder => new OpenFolder(),
            ImageMoniker.PasswordBox => new PasswordBox(),
            ImageMoniker.PasswordStrength => new PasswordStrength(),
            ImageMoniker.Save => new Save(),
            ImageMoniker.SaveAs => new SaveAs(),
            ImageMoniker.StatusHelp => new StatusHelp(),
            ImageMoniker.StatusOK => new StatusOK(),
            ImageMoniker.StatusWarning => new StatusWarning(),
            ImageMoniker.Team => new Team(),
            ImageMoniker.User => new User(),
        };
    }
}
