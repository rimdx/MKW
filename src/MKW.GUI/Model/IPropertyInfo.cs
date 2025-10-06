using MKW.Core;

namespace MKW.GUI.Model
{
    public interface IPropertyInfo
    {
        EntryPayloadKey Key { get; }
        string Name { get; }
    }
}
