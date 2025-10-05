using MKW.Core;
using MKW.GUI.Model;

namespace MKW.GUI.EntryEditor
{
    public class EditCustomPropertyViewModel : ViewModelBase
    {
        private readonly EntryPayloadEditorPropertiesViewModel properties;
        public CommonEntryPropertiesModel CommonPropertiesModel { get; }

        private readonly EntryPayloadKey key;

        public EditCustomPropertyViewModel(EntryPayloadEditorPropertiesViewModel properties,
                                           CommonEntryPropertiesModel commonPropertiesModel,
                                           EntryPayloadKey key, string content)
        {
            this.properties = properties;
            CommonPropertiesModel = commonPropertiesModel;

            this.key = key;
            this.content = content;

            name = EntryPayloadKey.RelativeName(CommonEntryPropertiesModel.CustomPropertyNamespace, key);
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
            properties.SetCustomProperty(key, name, content);
        }
    }
}
