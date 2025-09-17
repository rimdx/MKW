using System.Windows;
using System.Windows.Controls;

namespace MKW.GUI
{
    public class PasswordRepeatInput : PasswordInput
    {
        protected PasswordBox? _passwordRepeatBox;

        static PasswordRepeatInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PasswordRepeatInput),
                                                     new FrameworkPropertyMetadata(typeof(PasswordRepeatInput)));
        }

        public static readonly DependencyProperty IsPasswordMatchProperty =
            DependencyProperty.Register(nameof(IsPasswordMatch),
                                        typeof(bool),
                                        typeof(PasswordRepeatInput));

        public bool IsPasswordMatch
        {
            get => (bool)GetValue(IsPasswordMatchProperty);
            set => throw new NotSupportedException();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_passwordRepeatBox != null)
                _passwordRepeatBox.PasswordChanged -= PasswordChanged;

            _passwordRepeatBox = GetTemplateChild("PART_PasswordRepeatBox") as PasswordBox;

            if (_passwordRepeatBox != null)
            {
                _passwordRepeatBox.Password = Password;
                _passwordRepeatBox.PasswordChanged += PasswordChanged;
            }

            Update();
        }

        protected override void Update()
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
