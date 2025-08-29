using MKW.Core.Common;
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

            string base32string = Base32Convert.Encode(data);

            int i = 0;
            foreach (char c in base32string)
            {
                if (i >= lineLength)
                {
                    sb.AppendLine();
                    i = 0;
                }

                sb.Append(c);

                i++;
            }

            return sb.ToString();
        }
    }
}
