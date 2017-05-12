using Smellyriver.Wpf.Effects;
using System;
using System.Windows;

namespace Smellyriver.TankInspector.Design
{
    internal class ContentButtonBrightnessEffect : BrightContrastEffect
    {
        public double HighlightBrightness
        {
            get => (double)GetValue(HighlightBrightnessProperty);
	        set => SetValue(HighlightBrightnessProperty, value);
        }

        public static readonly DependencyProperty HighlightBrightnessProperty =
            DependencyProperty.Register("HighlightBrightness", typeof(double), typeof(ContentButtonBrightnessEffect), new PropertyMetadata(0.1));

        public double ShadowBrightness
        {
            get => (double)GetValue(ShadowBrightnessProperty);
	        set => SetValue(ShadowBrightnessProperty, value);
        }

        public static readonly DependencyProperty ShadowBrightnessProperty =
            DependencyProperty.Register("ShadowBrightness", typeof(double), typeof(ContentButtonBrightnessEffect), new PropertyMetadata(-0.1));

        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
	        set => SetValue(ProgressProperty, value);
        }

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(ContentButtonBrightnessEffect), new PropertyMetadata(0.0, ContentButtonBrightnessEffect.OnProgressChanged));

        private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ContentButtonBrightnessEffect)d).OnPropertyChanged();
        }

        private void OnPropertyChanged()
        {
            if (this.Progress < 0)
                this.Brightness = Math.Abs(this.Progress) * this.ShadowBrightness;
            else
                this.Brightness = this.Progress * this.HighlightBrightness;
        }

    }
}
