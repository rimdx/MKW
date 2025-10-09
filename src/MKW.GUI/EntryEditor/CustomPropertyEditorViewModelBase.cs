namespace MKW.GUI.EntryEditor
{
    public abstract class CustomPropertyEditorViewModelBase : ViewModelBase
    {
        public CustomPropertyEditorViewModelBase(string name, string content)
        {
            this.name = name;
            this.content = content;
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

        public abstract void OnOK();
    }
}
