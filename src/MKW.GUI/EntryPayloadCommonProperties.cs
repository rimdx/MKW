using MKW.Core;

namespace MKW.GUI
{
    public static class EntryPayloadCommonProperties
    {
        public readonly static EntryPayloadKey DefaultNamespace = new EntryPayloadKey("mkw");

        public readonly static EntryPayloadKey Title = DefaultNamespace.Branch("title");
        public readonly static EntryPayloadKey Username = DefaultNamespace.Branch("username");
        public readonly static EntryPayloadKey Password = DefaultNamespace.Branch("password");
        public readonly static EntryPayloadKey Url = DefaultNamespace.Branch("url");
        public readonly static EntryPayloadKey Notes = DefaultNamespace.Branch("notes");

        public readonly static EntryPayloadKey CustomPropertyNamespace = DefaultNamespace.Branch("custom");
    }
}
