namespace MKW.Core.Serialization.KeePassXmlV2
{
    public sealed class KeePassXmlV2Reader : IDisposable
    {
        private readonly KeePassFile file;
        private readonly Stack<KeePassGroup> groupsStack;
        private readonly Queue<KeePassEntry> entriesQueue;
        private readonly Stream stream;

        public KeePassXmlV2Reader(Stream stream)
        {
            file = KeePassXmlSerializer.Deserialize(stream);

            groupsStack = [];
            entriesQueue = [];

            Reset();
            this.stream = stream;
        }

        public BackupEntry? NextEntry()
        {
            while (entriesQueue.Count == 0)
            {
                if (!StepGroups())
                {
                    return null;
                }
            }

            return ConvertEntry(entriesQueue.Dequeue());
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
                    entriesQueue.Enqueue(entry);
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
            entriesQueue.Clear();

            groupsStack.Push(file.Root.RootGroup);
        }

        public void Dispose()
        {
            stream.Dispose();
        }
    }
}
