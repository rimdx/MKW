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

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(nameof(Password),
                                        typeof(string),
                                        typeof(PasswordInput));

        public virtual string? Password
        {
            get => (string?)GetValue(PasswordProperty);
            set
            {
                SetValue(PasswordProperty, value);

                if (passwordBox != null)
                    passwordBox.Password = value;
            }
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
