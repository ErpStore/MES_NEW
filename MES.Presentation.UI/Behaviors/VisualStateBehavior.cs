using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace MES.Presentation.UI.Behaviors
{
    public static class VisualStateBehavior
    {
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.RegisterAttached(
                "State",
                typeof(string),
                typeof(VisualStateBehavior),
                new PropertyMetadata(OnStateChanged));

        public static string GetState(DependencyObject obj)
            => (string)obj.GetValue(StateProperty);

        public static void SetState(DependencyObject obj, string value)
            => obj.SetValue(StateProperty, value);

        private static void OnStateChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(
        $"VisualStateBehavior fired. NewState = {e.NewValue}");
            if (d is Control control && e.NewValue != null)
            {
                VisualStateManager.GoToState(
                    control,
                    e.NewValue.ToString(),
                    true);
            }
        }
    }

}
