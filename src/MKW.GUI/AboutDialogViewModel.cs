namespace MKW.GUI
{
    internal sealed class AboutDialogViewModel : ViewModelBase
    {
        public string Version { get; }

        public AboutDialogViewModel()
        {
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!.ToString();
        }
    }
}
