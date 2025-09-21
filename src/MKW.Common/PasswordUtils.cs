using System.Collections;

namespace MKW.Common
{
    public class PasswordUtils
    {
        private static readonly string[] categories = 
            [
                "0123456789",
                "abcdefghijklmnopqrstuvwxyz",
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
                " !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"
            ];

        public static double MeasurePasswordEntropy(string password)
        {
            BitArray mask = new BitArray(categories.Length + 1);
            int charRange = 0;

            foreach (char ch in password)
            {
                int category = GetCharCategory(ch);

                if (!mask.Get(category + 1))
                {
                    if (category >= 0)
                    {
                        charRange += categories[category].Length;
                    }
                    else
                    {
                        charRange += 20;
                    }

                    mask.Set(category + 1, true);
                }
            }

            if (charRange == 0)
            {
                return 0;
            }

            double entropy = password.Length * Math.Log(charRange) / Math.Log(2);

            return entropy;

            static int GetCharCategory(char ch)
            {
                for (int i = 0; i < categories.Length; i++)
                {
                    if (categories[i].IndexOf(ch) >= 0)
                    {
                        return i;
                    }
                }

                return -1;
            }
        }
    }
}
