using System.ComponentModel;

namespace MKW.GUI
{
    internal static class PropertyChangedEventArgsExtensions
    {
        public static bool MatchProperty(this PropertyChangedEventArgs eventArgs, string propertyName)
        {
            return 
                eventArgs.PropertyName == null ||
                eventArgs.PropertyName.Length == 0 ||
                eventArgs.PropertyName == propertyName;
        }
    }
}
