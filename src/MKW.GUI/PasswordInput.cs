using System.Windows;
using System.Windows.Controls;

namespace MKW.GUI
{
    public class PasswordInput : Control
    {
        protected PasswordBox? passwordBox;

        static PasswordInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PasswordInput),
                                                     new FrameworkPropertyMetadata(typeof(PasswordInput)));
        }

        private static readonly FrameworkPropertyMetadata PasswordPropertyMetadata =
            new FrameworkPropertyMetadata("",
                                          FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                          OnPasswordPropertyChanged);

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(nameof(Password),
                                        typeof(string),
                                        typeof(PasswordInput),
                                        PasswordPropertyMetadata);

        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordInput input = (PasswordInput)d;

            string oldPassword = (string)e.OldValue;
            string newPassword = (string)e.NewValue;

            if (oldPassword != newPassword)
            {
                input.OnPasswordPropertyChanged(oldPassword, newPassword);
            }
        }

        private void OnPasswordPropertyChanged(string oldPassword, string newPassword)
        {
            if (passwordBox != null)
            {
                if (!passwordBox.IsFocused)
                {
                    passwordBox.Password = newPassword;
                }
            }
        }

        public virtual string? Password
        {
            get => (string?)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (passwordBox != null)
                passwordBox.PasswordChanged -= PasswordChanged;

            passwordBox = GetTemplateChild("PART_PasswordBox") as PasswordBox;

            if (passwordBox != null)
            {
                passwordBox.Password = Password;
                passwordBox.PasswordChanged += PasswordChanged;
            }

            Update();
        }

        protected virtual void Update()
        {
            if (passwordBox != null)
            {
                SetValue(PasswordProperty, passwordBox.Password);
            }
        }

        protected void PasswordChanged(object sender, RoutedEventArgs e)
        {
            Update();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (passwordBox != null)
            {
                passwordBox.Focus();
                e.Handled = true;
            }
        }
    }
}
