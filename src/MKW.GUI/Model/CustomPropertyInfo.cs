using MKW.Core;

namespace MKW.GUI.Model
{
    public sealed class CustomPropertyInfo : ViewModelBase
    {
        public EntryPayloadKey Key { get; }
        public string Name { get; }

        public CustomPropertyInfo(string name)
        {
            Name = name;
            Key = CommonEntryPropertiesModel.CustomPropertyNamespace.Branch(name);
        }

        public override bool Equals(object? obj)
        {
            return obj is CustomPropertyInfo info && Key == info.Key;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}
