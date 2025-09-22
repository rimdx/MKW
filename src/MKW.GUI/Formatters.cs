namespace MKW.GUI
{
    internal static class Formatters
    {
        public static string FormatUserName(string userId, string displayName)
        {
            if (displayName.Length > 0)
            {
                return $"{displayName} <{userId}>";
            }
            else if (userId.Length > 0)
            {
                return userId;
            }
            else
            {
                return "<empty>";
            }
        }
    }
}
