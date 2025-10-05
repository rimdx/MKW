using MKW.GUI.Model;

namespace MKW.GUI.EntryEditor
{
    public class NewCustomPropertyViewModel : ViewModelBase
    {
        private readonly EntryPayloadEditorPropertiesViewModel properties;
        public CommonEntryPropertiesModel CommonPropertiesModel { get; }

        public NewCustomPropertyViewModel(EntryPayloadEditorPropertiesViewModel properties,
                                          CommonEntryPropertiesModel commonPropertiesModel)
        {
            this.properties = properties;
            CommonPropertiesModel = commonPropertiesModel;

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
