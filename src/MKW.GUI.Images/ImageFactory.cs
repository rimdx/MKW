using System.Windows.Controls;

namespace MKW.GUI.Images
{
    public static class ImageFactory
    {
        public static Viewbox? MakeImage(ImageMoniker moniker) => moniker switch
        {
            ImageMoniker.None => null,
            ImageMoniker.Add => new Add(),
            ImageMoniker.AddDatabase => new AddDatabase(),
            ImageMoniker.AddKey => new AddKey(),
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
            ImageMoniker.DeleteKey => new DeleteKey(),
            ImageMoniker.DeleteUser => new DeleteUser(),
            ImageMoniker.Edit => new Edit(),
            ImageMoniker.EditDatabase => new EditDatabase(),
            ImageMoniker.EditDocument => new EditDocument(),
            ImageMoniker.EditKey => new EditKey(),
            ImageMoniker.Export => new Export(),
            ImageMoniker.FolderClosed => new FolderClosed(),
            ImageMoniker.FolderOpened => new FolderOpened(),
            ImageMoniker.Image => new Image(),
            ImageMoniker.Key => new Key(),
            ImageMoniker.Lock => new Lock(),
            ImageMoniker.LoginUser => new LoginUser(),
            ImageMoniker.NewDocument => new NewDocument(),
            ImageMoniker.NewFolder => new NewFolder(),
            ImageMoniker.NewKey => new NewKey(),
            ImageMoniker.NewUser => new NewUser(),
            ImageMoniker.Next => new Next(),
            ImageMoniker.OpenFile => new OpenFile(),
            ImageMoniker.OpenFolder => new OpenFolder(),
            ImageMoniker.PasswordBox => new PasswordBox(),
            ImageMoniker.PasswordStrength => new PasswordStrength(),
            ImageMoniker.ReadOnlyDatabase => new ReadOnlyDatabase(),
            ImageMoniker.Save => new Save(),
            ImageMoniker.SaveAs => new SaveAs(),
            ImageMoniker.StatusHelp => new StatusHelp(),
            ImageMoniker.StatusInformation => new StatusInformation(),
            ImageMoniker.StatusOK => new StatusOK(),
            ImageMoniker.StatusWarning => new StatusWarning(),
            ImageMoniker.Team => new Team(),
            ImageMoniker.Unlock => new Unlock(),
            ImageMoniker.User => new User(),
        };
    }
}
