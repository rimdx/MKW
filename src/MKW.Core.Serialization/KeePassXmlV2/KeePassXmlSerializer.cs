using System.Xml.Serialization;

namespace MKW.Core.Serialization.KeePassXmlV2
{
    internal static class KeePassXmlSerializer
    {
        private static Lazy<XmlSerializer> serializer = new Lazy<XmlSerializer>(static () =>
        {
            return new XmlSerializer(typeof(KeePassFile));
        });

        public static KeePassFile Deserialize(Stream stream)
        {
            KeePassFile? file = (KeePassFile?)serializer.Value.Deserialize(stream);

            if (file == null)
            {
                throw new NullReferenceException();
            }

            return file;
        }
    }
}
