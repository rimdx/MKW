using MKW.GUI.SingleInstance;

namespace MKW.GUI
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            App app = new App();
            SingleInstanceApplicationManager manager = new SingleInstanceApplicationManager(app);
            manager.Run(args);
        }
    }
}
