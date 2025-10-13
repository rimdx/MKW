namespace MKW.GUI.Database
{
    public class EntryListColumn : ViewModelBase
    {
        private string header;
        private double width;
        private string propertyName;
        private bool hideValue;

        public EntryListColumn(string header, int width, string propertyName)
        {
            this.header = header;
            this.width = width;
            this.propertyName = propertyName;
        }

        public string Header
        {
            get => header;
            set => SetProperty(ref header, value);
        }

        public double Width
        {
            get => width;
            set => SetProperty(ref width, value);
        }

        public string PropertyName
        {
            get => propertyName;
            set => SetProperty(ref propertyName, value);
        }

        public bool HideValue
        {
            get => hideValue;
            set => SetProperty(ref hideValue, value);
        }
    }
}
