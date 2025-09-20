using MKW.Common;
using MKW.GUI.Images;

namespace MKW.GUI
{
    public class PasswordViewModel : ViewModelBase
    {
        private string password;
        private string passwordRepeat;
        private double passwordEntropy;
        private int passwordLength;
        private bool passwordMismatch;
        private ImageMoniker passwordQualityIcon;

        public PasswordViewModel()
        {
            password = "";
            passwordRepeat = "";
            passwordEntropy = GetPasswordEntropy(password);
            passwordQualityIcon = GetPasswordQualityIcon(passwordEntropy);
            passwordLength = GetPasswordLength(password);
            passwordMismatch = GetPasswordMismatch(password, passwordRepeat);
        }

        public string Password
        {
            get => password;
            set
            {
                if (SetProperty(ref password, value))
                {
                    PasswordEntropy = GetPasswordEntropy(password);
                    PasswordLength = GetPasswordLength(password);
                    PasswordMismatch = GetPasswordMismatch(password, passwordRepeat);
                }
            }
        }

        public string PasswordRepeat
        {
            get => passwordRepeat;
            set
            {
                if (SetProperty(ref passwordRepeat, value))
                {
                    PasswordMismatch = GetPasswordMismatch(password, passwordRepeat);
                }
            }
        }

        public bool PasswordMismatch
        {
            get => passwordMismatch;
            private set => SetProperty(ref passwordMismatch, value);
        }

        public double PasswordEntropy
        {
            get => passwordEntropy;
            private set
            {
                if (SetProperty(ref passwordEntropy, value))
                {
                    PasswordQualityIcon = GetPasswordQualityIcon(value);
                }
            }
        }

        public int PasswordLength
        {
            get => passwordLength;
            private set => SetProperty(ref passwordLength, value);
        }

        private static int GetPasswordLength(string password)
        {
            return password.Length;
        }

        private static double GetPasswordEntropy(string password)
        {
            return PasswordUtils.MeasurePasswordEntropy(password);
        }

        private static bool GetPasswordMismatch(string password, string passwordRepeat)
        {
            return password != passwordRepeat;
        }

        private static ImageMoniker GetPasswordQualityIcon(double passwordEntropy)
        {
            if (passwordEntropy >= 75)
            {
                return ImageMoniker.StatusOK;
            }
            else
            {
                return ImageMoniker.StatusWarning;
            }
        }

        public ImageMoniker PasswordQualityIcon
        {
            get => passwordQualityIcon;
            private set => SetProperty(ref passwordQualityIcon, value);
        }
    }
}
