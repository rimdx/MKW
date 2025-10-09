namespace MKW.Core.Serialization.KeePassXML
{
    public sealed class KeePassXmlReader
    {
        private readonly KeePassFile file;
        private readonly Stack<KeePassGroup> groupsStack;
        private readonly Stack<KeePassEntry> entriesStack;

        public KeePassXmlReader(Stream stream)
        {
            file = KeePassXmlSerializer.Deserialize(stream);

            groupsStack = [];
            entriesStack = [];

            Reset();
        }

        public BackupEntry? NextEntry()
        {
            while (entriesStack.Count == 0)
            {
                if (!StepGroups())
                {
                    return null;
                }
            }

            return ConvertEntry(entriesStack.Pop());
        }

        private static BackupEntry ConvertEntry(KeePassEntry entry)
        {
            Dictionary<string, string> fields = [];

            if (entry.Fields != null)
            {
                foreach (KeePassFieldString field in entry.Fields)
                {
                    fields[field.Key] = field.Value;
                }
            }

            return new BackupEntry(fields);
        }

        private bool StepGroups()
        {
            if (groupsStack.Count == 0)
            {
                return false;
            }
            else
            {
                KeePassGroup current = groupsStack.Pop();

                VisitGroupNode(current);
                VisitGroupChildren(current);

                return true;
            }
        }

        private void VisitGroupChildren(KeePassGroup current)
        {
            if (current.Groups != null)
            {
                foreach (KeePassGroup group in current.Groups)
                {
                    groupsStack.Push(group);
                }
            }
        }

        private void VisitGroupNode(KeePassGroup current)
        {
            if (current.Entries != null)
            {
                foreach (KeePassEntry entry in current.Entries)
                {
                    entriesStack.Push(entry);
                }
            }
        }

        public IEnumerable<BackupEntry> EnumerateEntries()
        {
            while (true)
            {
                BackupEntry? entry = NextEntry();

                if (entry == null)
                {
                    yield break;
                }
                else
                {
                    yield return entry;
                }
            }
        }

        public void Reset()
        {
            groupsStack.Clear();
            entriesStack.Clear();

            groupsStack.Push(file.Root.RootGroup);
        }
    }
}
