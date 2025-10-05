namespace MKW.GUI.EntryEditor
{
    public class NewCustomPropertyViewModel : ViewModelBase
    {
        private readonly EntryPayloadEditorPropertiesViewModel properties;

        public NewCustomPropertyViewModel(EntryPayloadEditorPropertiesViewModel properties)
        {
            this.properties = properties;

            content = string.Empty;
            name= string.Empty;
        }

        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string content;
        public string Content
        {
            get => content;
            set => SetProperty(ref content, value);
        }

        public void OnOK()
        {
            properties.SetCustomProperty(null, name, content);
        }
    }
}
