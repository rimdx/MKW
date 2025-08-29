using System.Text;

namespace MKW.GUI.Services
{
    public class KeyFormatter
    {
        private readonly int lineLength;

        public KeyFormatter(int lineLength)
        {
            this.lineLength = lineLength;
        }

        public string GetString(ReadOnlySpan<byte> data)
        {
            StringBuilder sb = new StringBuilder();

            //string base32string = Convert.Base3(data);

            //foreach (var c in base64string)
            //{

            //}

            return sb.ToString();
        }
    }
}
