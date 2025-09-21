using System.Windows;
using System.Windows.Controls;

namespace MKW.GUI
{
    public class PasswordInput : Control
    {
        protected PasswordBox? _passwordBox;

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

                if (_passwordBox != null)
                    _passwordBox.Password = value;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_passwordBox != null)
                _passwordBox.PasswordChanged -= PasswordChanged;

            _passwordBox = GetTemplateChild("PART_PasswordBox") as PasswordBox;

            if (_passwordBox != null)
            {
                _passwordBox.Password = Password;
                _passwordBox.PasswordChanged += PasswordChanged;
            }

            Update();
        }

        protected virtual void Update()
        {
            if (_passwordBox != null)
            {
                SetValue(PasswordProperty, _passwordBox.Password);
            }
        }

        protected void PasswordChanged(object sender, RoutedEventArgs e)
        {
            Update();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (_passwordBox != null)
            {
                _passwordBox.Focus();
                e.Handled = true;
            }
        }
    }
}
