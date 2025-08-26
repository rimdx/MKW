using System.Windows;
using System.Windows.Controls;

namespace MKW.GUI
{
    public class PasswordInput : Control
    {
        private PasswordBox? _passwordBox;
        private PasswordBox? _passwordRepeatBox;

        static PasswordInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PasswordInput),
                                                     new FrameworkPropertyMetadata(typeof(PasswordInput)));
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(nameof(Password),
                                        typeof(string),
                                        typeof(PasswordInput));

        public string? Password
        {
            get => (string?)GetValue(PasswordProperty);
            set
            {
                SetValue(PasswordProperty, value);

                if (_passwordBox != null)
                    _passwordBox.Password = value;

                if (_passwordRepeatBox != null)
                    _passwordRepeatBox.Password = value;
            }
        }


        public static readonly DependencyProperty IsPasswordMatchProperty =
            DependencyProperty.Register(nameof(IsPasswordMatch),
                                        typeof(bool),
                                        typeof(PasswordInput));

        public bool IsPasswordMatch
        {
            get => (bool)GetValue(IsPasswordMatchProperty);
            set => throw new NotSupportedException();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_passwordBox != null)
                _passwordBox.PasswordChanged -= PasswordChanged;
            if (_passwordRepeatBox != null)
                _passwordRepeatBox.PasswordChanged -= PasswordChanged;

            _passwordBox = GetTemplateChild("PART_PasswordBox") as PasswordBox;
            _passwordRepeatBox = GetTemplateChild("PART_PasswordRepeatBox") as PasswordBox;

            if (_passwordBox != null)
            {
                _passwordBox.Password = Password;
                _passwordBox.PasswordChanged += PasswordChanged;
            }

            if (_passwordRepeatBox != null)
            {
                _passwordRepeatBox.Password = Password;
                _passwordRepeatBox.PasswordChanged += PasswordChanged;
            }
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_passwordBox != null)
            {
                if (_passwordRepeatBox == null || !_passwordRepeatBox.IsEnabled)
                {
                    SetValue(PasswordProperty, _passwordBox.Password);
                    SetValue(IsPasswordMatchProperty, true);
                }
                else
                {
                    if (_passwordBox.Password == _passwordRepeatBox.Password)
                    {
                        SetValue(PasswordProperty, _passwordBox.Password);
                        SetValue(IsPasswordMatchProperty, true);
                    }
                    else
                    {
                        SetValue(PasswordProperty, null);
                        SetValue(IsPasswordMatchProperty, false);
                    }
                }
            }
        }
    }
}
