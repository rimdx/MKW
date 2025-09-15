namespace MKW.Core.Client.Exceptions
{
    public class InvalidPasswordException(Exception ex)
        : Exception("The password is not correct.", ex)
    {
    }
}
