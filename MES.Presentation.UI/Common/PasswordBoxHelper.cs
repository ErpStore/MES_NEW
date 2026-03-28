using System.Windows;
using System.Windows.Controls;

namespace MES.Presentation.UI.Common
{
    public static class PasswordBoxHelper
    {
        // 1. Define the Attached Property "BoundPassword"
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxHelper),
                new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static string GetBoundPassword(DependencyObject d) => (string)d.GetValue(BoundPasswordProperty);
        public static void SetBoundPassword(DependencyObject d, string value) => d.SetValue(BoundPasswordProperty, value);

        // 2. Handle when the ViewModel updates the password (e.g. clearing it)
        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox box)
            {
                // Avoid infinite loops: only update if different
                if (box.Password != (string)e.NewValue)
                {
                    box.PasswordChanged -= PasswordChanged; // Temporarily detach to avoid re-triggering
                    box.Password = (string)e.NewValue ?? string.Empty;
                    box.PasswordChanged += PasswordChanged;
                }
            }
        }

        // 3. Define the "BindPassword" Flag to enable this behavior
        public static readonly DependencyProperty BindPasswordProperty =
            DependencyProperty.RegisterAttached("BindPassword", typeof(bool), typeof(PasswordBoxHelper),
                new PropertyMetadata(false, OnBindPasswordChanged));

        public static bool GetBindPassword(DependencyObject d) => (bool)d.GetValue(BindPasswordProperty);
        public static void SetBindPassword(DependencyObject d, bool value) => d.SetValue(BindPasswordProperty, value);

        private static void OnBindPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox box)
            {
                if ((bool)e.NewValue)
                {
                    box.PasswordChanged += PasswordChanged;
                }
                else
                {
                    box.PasswordChanged -= PasswordChanged;
                }
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox box)
            {
                SetBoundPassword(box, box.Password);
            }
        }
    }
}

