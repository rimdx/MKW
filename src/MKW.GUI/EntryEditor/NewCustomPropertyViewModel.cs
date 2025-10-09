using MKW.GUI.Model;

namespace MKW.GUI.EntryEditor
{
    public class NewCustomPropertyViewModel : CustomPropertyEditorViewModelBase
    {
        private readonly EntryPayloadEditorPropertiesModel properties;

        public CommonEntryPropertiesModel CommonPropertiesModel { get; }

        public NewCustomPropertyViewModel(EntryPayloadEditorPropertiesModel properties,
                                          CommonEntryPropertiesModel commonPropertiesModel)
            : base(string.Empty, string.Empty)
        {
            this.properties = properties;
            CommonPropertiesModel = commonPropertiesModel;
        }

        public override void OnOK()
        {
            properties.SetCustomProperty(null, Name, Content);
        }
    }
}
