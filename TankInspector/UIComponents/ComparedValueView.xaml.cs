using Smellyriver.TankInspector.DataAnalysis;
using Smellyriver.Wpf.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for ComparedValueView.xaml
    /// </summary>
    public partial class ComparedValueView : UserControl
    {
        public double Value
        {
            get => (double)GetValue(ValueProperty);
	        set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ComparedValueView), new PropertyMetadata(0.0, ComparedValueView.UpdateValue));

        public string ValueFormatString
        {
            get => (string)GetValue(ValueFormatStringProperty);
	        set => SetValue(ValueFormatStringProperty, value);
        }

        public static readonly DependencyProperty ValueFormatStringProperty =
            DependencyProperty.Register("ValueFormatString", typeof(string), typeof(ComparedValueView), new PropertyMetadata("F2", ComparedValueView.UpdateValue));

        public double? ComparationTarget
        {
            get => (double?)GetValue(ComparationTargetProperty);
	        set => SetValue(ComparationTargetProperty, value);
        }

        public static readonly DependencyProperty ComparationTargetProperty =
            DependencyProperty.Register("ComparationTarget", typeof(double?), typeof(ComparedValueView), new PropertyMetadata(null, ComparedValueView.UpdateValue));


        public double Deviation
        {
            get => (double)GetValue(DeviationProperty);
	        set => SetValue(DeviationProperty, value);
        }

        public static readonly DependencyProperty DeviationProperty =
            DependencyProperty.Register("Deviation", typeof(double), typeof(ComparedValueView), new PropertyMetadata(1.0, ComparedValueView.UpdateColor));

        public Color BetterColor
        {
            get => (Color)GetValue(BetterColorProperty);
	        set => SetValue(BetterColorProperty, value);
        }

        public static readonly DependencyProperty BetterColorProperty =
            DependencyProperty.Register("BetterColor", typeof(Color), typeof(ComparedValueView), new PropertyMetadata(Color.FromArgb(0xff, 0, 0xb2, 0), ComparedValueView.UpdateColor));

        public Color WorseColor
        {
            get => (Color)GetValue(WorseColorProperty);
	        set => SetValue(WorseColorProperty, value);
        }

        public static readonly DependencyProperty WorseColorProperty =
            DependencyProperty.Register("WorseColor", typeof(Color), typeof(ComparedValueView), new PropertyMetadata(Color.FromArgb(0xff, 0xb2, 0, 0), ComparedValueView.UpdateColor));

        public ComparisonMode ComparationMode
        {
            get => (ComparisonMode)GetValue(ComparationModeProperty);
	        set => SetValue(ComparationModeProperty, value);
        }

        public static readonly DependencyProperty ComparationModeProperty =
            DependencyProperty.Register("ComparationMode", typeof(ComparisonMode), typeof(ComparedValueView), new PropertyMetadata(ComparisonMode.HigherBetter, ComparedValueView.UpdateColor));



        private static void UpdateValue(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ComparedValueView)d).UpdateValue();
        }


        private static void UpdateColor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ComparedValueView)d).UpdateColor();
        }

        public ComparedValueView()
        {
            InitializeComponent();
        }

        private void UpdateValue()
        {
            this.Text.Text = this.Value.ToString(this.ValueFormatString);

            if (!this.ComparationTarget.HasValue)
                this.Comparation.Visibility = Visibility.Hidden;
            else
            {
                var delta = this.Value - this.ComparationTarget.Value;
                if (delta < double.Epsilon)
                    this.Comparation.Visibility = Visibility.Hidden;
                else
                {
                    this.Comparation.Visibility = Visibility.Visible;

                    if (delta > 0)
                        this.Comparation.Text = "+" + delta.ToString(this.ValueFormatString);
                    else
                        this.Comparation.Text = delta.ToString(this.ValueFormatString);
                }
            }
            this.UpdateColor();
        }

        private void UpdateColor()
        {
            if (this.Deviation < double.Epsilon)
                throw new InvalidOperationException("deviation must not be zero");

            if (this.ComparationTarget.HasValue)
            {

                var delta = this.Value - this.ComparationTarget.Value;
                var ratio = delta / this.Deviation;

                if (this.ComparationMode == ComparisonMode.LowerBetter)
                    ratio = -ratio;

                var normalizedRatio = (ratio + 1.0) / 2.0;

                Color color;
                if (normalizedRatio > 1.0)
                    color = this.BetterColor;
                else if (normalizedRatio < 0.0)
                    color = this.WorseColor;
                else
                    color = ColorEx.Interpolate(this.WorseColor, this.BetterColor, normalizedRatio);

                this.Comparation.Foreground = new SolidColorBrush(color);
            }
        }

    }
}
