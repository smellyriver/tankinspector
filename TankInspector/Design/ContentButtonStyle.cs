using System.Windows;

namespace Smellyriver.TankInspector.Design
{
    internal static class ContentButtonStyle
    {

        public static readonly DependencyProperty HighlightBrightnessProperty =
            DependencyProperty.RegisterAttached("HighlightBrightness", typeof(double), typeof(ContentButtonStyle), new PropertyMetadata(0.2));

        public static double GetHighlightBrightness(DependencyObject target)
        {
            return (double)target.GetValue(HighlightBrightnessProperty);
        }

        public static void SetHighlightBrightness(DependencyObject target, double value)
        {
            target.SetValue(HighlightBrightnessProperty, value);
        }

        public static readonly DependencyProperty ShadowBrightnessProperty =
            DependencyProperty.RegisterAttached("ShadowBrightness", typeof(double), typeof(ContentButtonStyle), new PropertyMetadata(0.2));

        public static double GetShadowBrightness(DependencyObject target)
        {
            return (double)target.GetValue(ShadowBrightnessProperty);
        }

        public static void SetShadowBrightness(DependencyObject target, double value)
        {
            target.SetValue(ShadowBrightnessProperty, value);
        }

    }
}
