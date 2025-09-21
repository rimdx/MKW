namespace MKW.Core.Exceptions
{
    public class InvalidPasswordException(Exception ex)
        : Exception("The password is not correct.", ex)
    {
    }
}
