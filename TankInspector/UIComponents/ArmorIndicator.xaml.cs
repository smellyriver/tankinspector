using Smellyriver.TankInspector.Modeling;
using Smellyriver.Wpf.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for ArmorIndicator.xaml
    /// </summary>
    public partial class ArmorIndicator : UserControl
    {
        private static readonly GradientStopCollection DefaultSpectrumStops = new GradientStopCollection(
                new[]
                {
                    new GradientStop(Colors.Green, 0.0),
                    new GradientStop(Colors.Yellow, 0.5),
                    new GradientStop(Colors.Red, 1.0)
                });

        private static readonly DoubleCollection DefaultTicks = new DoubleCollection(
                new[]
                {
                    10.0,
                    30.0,
                    75.0,
                    85.0,
                    95.0,
                    97.0,
                    98.0,
                    100.0
                });

        private double[] _sortedTicks;

        public DoubleCollection Ticks
        {
            get => (DoubleCollection)GetValue(TicksProperty);
	        set => SetValue(TicksProperty, value);
        }

        public static readonly DependencyProperty TicksProperty =
            DependencyProperty.Register("Ticks", typeof(DoubleCollection), typeof(ArmorIndicator), new PropertyMetadata(DefaultTicks, ArmorIndicator.OnTicksChanged));

        private static void OnTicksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var armorIndicator = (ArmorIndicator)d;
            armorIndicator.UpdateSortedTicks();
            armorIndicator.UpdateArmorSpectrum();
            armorIndicator.UpdateHint();
            armorIndicator.SelectAll();
        }

        private GradientStop[] _sortedSpectrumStops;

        public GradientStopCollection SpectrumStops
        {
            get => (GradientStopCollection)GetValue(SpectrumStopsProperty);
	        set => SetValue(SpectrumStopsProperty, value);
        }

        public static readonly DependencyProperty SpectrumStopsProperty =
            DependencyProperty.Register("SpectrumStops", typeof(GradientStopCollection), typeof(ArmorIndicator), new PropertyMetadata(DefaultSpectrumStops, ArmorIndicator.OnSpectrumStopsChanged));

        private static void OnSpectrumStopsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var armorIndicator = (ArmorIndicator)d;
            armorIndicator.UpdateSortedSpectrumStops();
            armorIndicator.UpdateArmorSpectrum();
            armorIndicator.UpdateHint();
            armorIndicator.UpdateSelectionRange();
        }

        public double SelectionBegin
        {
            get => (double)GetValue(SelectionBeginProperty);
	        set => SetValue(SelectionBeginProperty, value);
        }

        public static readonly DependencyProperty SelectionBeginProperty =
            DependencyProperty.Register("SelectionBegin", typeof(double), typeof(ArmorIndicator), new PropertyMetadata(0.0, ArmorIndicator.UpdateSelectionRange));

        public double SelectionEnd
        {
            get => (double)GetValue(SelectionEndProperty);
	        set => SetValue(SelectionEndProperty, value);
        }


        public static readonly DependencyProperty SelectionEndProperty =
            DependencyProperty.Register("SelectionEnd", typeof(double), typeof(ArmorIndicator), new PropertyMetadata(100.0, ArmorIndicator.UpdateSelectionRange));



        public double HintValue
        {
            get => (double)GetValue(HintValueProperty);
	        set => SetValue(HintValueProperty, value);
        }

        public static readonly DependencyProperty HintValueProperty =
            DependencyProperty.Register("HintValue", typeof(double), typeof(ArmorIndicator), new PropertyMetadata(0.0, ArmorIndicator.UpdateHint));




        public bool HasHintValue
        {
            get => (bool)GetValue(HasHintValueProperty);
	        set => SetValue(HasHintValueProperty, value);
        }

        public static readonly DependencyProperty HasHintValueProperty =
            DependencyProperty.Register("HasHintValue", typeof(bool), typeof(ArmorIndicator), new PropertyMetadata(false, ArmorIndicator.UpdateHint));

        

        public HandSide TickPosition
        {
            get => (HandSide)GetValue(TickPositionProperty);
	        set => SetValue(TickPositionProperty, value);
        }

        public static readonly DependencyProperty TickPositionProperty =
            DependencyProperty.Register("TickPosition", typeof(HandSide), typeof(ArmorIndicator), new PropertyMetadata(HandSide.Right, ArmorIndicator.UpdateTickPosition));

        private static void UpdateTickPosition(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ArmorIndicator)d).UpdateTickPosition();
        }


        private static void UpdateHint(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ArmorIndicator)d).UpdateHint();
        }

        private static void UpdateSelectionRange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ArmorIndicator)d).UpdateSelectionRange();
        }

        private static void UpdateArmorSpectrum(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ArmorIndicator)d).UpdateArmorSpectrum();
        }


        private bool _updateHintPending;
        private bool _updateSelectionRangePending;
        private bool _preventUpdateSelectionRange;

        // double click event is fired before the second mouse down event
        private bool _doubleClickPending;
        public ArmorIndicator()
        {
            UpdateSortedTicks();
            UpdateSortedSpectrumStops();
            InitializeComponent();
            UpdateArmorSpectrum();
            UpdateSelectionRange();
            UpdateHint();
            UpdateTickPosition();
        }

        private void UpdateSortedTicks()
        {
            if (this.Ticks.Count < 1)
                throw new InvalidOperationException("at least 1 tick in Ticks is required");

            _sortedTicks = this.Ticks.Distinct().OrderByDescending(v => v).ToArray();
        }

        private void UpdateSortedSpectrumStops()
        {
            if (this.SpectrumStops.Count < 1)
                throw new InvalidOperationException("at least 1 stop in SpectrumStops is required");

            _sortedSpectrumStops = this.SpectrumStops.OrderBy(s => s.Offset).ToArray();
        }

        private void UpdateArmorSpectrum()
        {
            var finalSpectrumStops = new SortedDictionary<double, GradientStop>();
            foreach (var spectrumStop in _sortedSpectrumStops)
                finalSpectrumStops.Add(spectrumStop.Offset, spectrumStop.Clone());

            var minValue = _sortedTicks[_sortedTicks.Length - 1];
            var maxValue = _sortedTicks[0];
            
            var valueRange = maxValue - minValue;

            var tickOffsets = new double[_sortedTicks.Length];

            // create spectrum stops
            var spectrumStopStartIndex = 0;
            for (int tickIndex = 0; tickIndex < _sortedTicks.Length; ++tickIndex)
            {
                var tick = _sortedTicks[tickIndex];
                var valueOffset = 1 - (tick - minValue) / valueRange;
                tickOffsets[tickIndex] = valueOffset;

                if (finalSpectrumStops.ContainsKey(valueOffset))
                    continue;

                for (int spectrumStopIndex = spectrumStopStartIndex; spectrumStopIndex < _sortedSpectrumStops.Length; ++spectrumStopIndex)
                {
                    var spectrumStop = _sortedSpectrumStops[spectrumStopIndex];
                    if (spectrumStop.Offset > valueOffset)
                    {
                        // create a spectrum stop for this value
                        var previousSpectrumStopIndex = spectrumStopIndex - 1;
                        var previousSpectrumStop = _sortedSpectrumStops[previousSpectrumStopIndex];
                        var colorOffset = (valueOffset - previousSpectrumStop.Offset) / (spectrumStop.Offset - previousSpectrumStop.Offset);
                        var color = ColorEx.Interpolate(previousSpectrumStop.Color, spectrumStop.Color, colorOffset);
                        finalSpectrumStops.Add(valueOffset, new GradientStop(color, valueOffset));
                        spectrumStopStartIndex = previousSpectrumStopIndex;
                        break;
                    }
	                if (spectrumStop.Offset == valueOffset)
	                {
		                // this spectrum stop is already existed, no need to add again
		                spectrumStopStartIndex = spectrumStopIndex;
		                break;
	                }
                }
            }

            // remap spectrum offsets
            var mappedOffsetRangePerStop = 1.0 / (_sortedTicks.Length - 1);
            spectrumStopStartIndex = 0;
            var finalSpectrumStopArray = finalSpectrumStops.Values.ToArray();

            var headerAndFooterSize = 1.0 / _sortedTicks.Length / 2.0;

            for (int tickIndex = 0; tickIndex < _sortedTicks.Length; ++tickIndex)
            {
                var tickOffset = tickOffsets[tickIndex];
                var minMappedOffset = Math.Max(headerAndFooterSize, (tickIndex - 0.5) * mappedOffsetRangePerStop);
                var maxMappedOffset = Math.Min(1.0 - headerAndFooterSize, (tickIndex + 0.5) * mappedOffsetRangePerStop);
                var mappedOffsetRange = maxMappedOffset - minMappedOffset;
                var minOffset = tickIndex == 0 ? 0.0 : tickOffset - (tickOffset - tickOffsets[tickIndex - 1]) / 2;
                var maxOffset = tickIndex == _sortedTicks.Length - 1 ? 1.0 : tickOffset + (tickOffsets[tickIndex + 1] - tickOffset) / 2;
                var offsetRange = maxOffset - minOffset;

                for (int spectrumStopIndex = spectrumStopStartIndex; spectrumStopIndex < finalSpectrumStopArray.Length; ++spectrumStopIndex)
                {
                    var spectrumStop = finalSpectrumStopArray[spectrumStopIndex];
                    if (spectrumStop.Offset <= maxOffset && !spectrumStop.IsFrozen)
                    {
                        spectrumStop.Offset = (spectrumStop.Offset - minOffset) / offsetRange * mappedOffsetRange + minMappedOffset;
                        spectrumStop.Freeze();
                    }
                    else
                    {
                        spectrumStopStartIndex = spectrumStopIndex;
                        break;
                    }
                }
            }

            this.Spectrum.Fill = new LinearGradientBrush(new GradientStopCollection(finalSpectrumStopArray));

            // create tick labels


            this.TicksContainer.RowDefinitions.Clear();
            this.TicksContainer.Children.Clear();
            for (int tickIndex = 0; tickIndex < _sortedTicks.Length; ++tickIndex)
            {
                this.TicksContainer.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Star) });
                var tickLabel = new TextBlock(new Run(_sortedTicks[tickIndex].ToString("F0")));
                tickLabel.VerticalAlignment = VerticalAlignment.Center;
                tickLabel.HorizontalAlignment = HorizontalAlignment.Center;
                tickLabel.FontSize = 10;
                this.TicksContainer.Children.Add(tickLabel);
                Grid.SetRow(tickLabel, tickIndex);
            }


        }

        private void UpdateSelectionRange()
        {
            if (_preventUpdateSelectionRange)
                return;

            if (!this.TicksContainer.IsMeasureValid)
            {
                _updateSelectionRangePending = true;
                return;
            }

            var selectionBegin = Math.Max(this.SelectionBegin, this.SelectionEnd);
            var selectionEnd = Math.Min(this.SelectionBegin, this.SelectionEnd);

            var beginOffset = this.GetTickBoundary(selectionBegin);
            var endOffset = this.GetTickBoundary(selectionEnd, true);

            // Height might be NaN, which cannot be animated
            this.SelectionRangeBackground.Height = this.SelectionRangeBackground.ActualHeight;

            var storyboard = new Storyboard();
            var duration = new Duration(TimeSpan.FromSeconds(0.2));
            var heightAnimation = new DoubleAnimation(endOffset - beginOffset, duration);
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(HeightProperty));
            storyboard.Children.Add(heightAnimation);
            var marginAnimation = new ThicknessAnimation(new Thickness(0, beginOffset, 0, 0), duration);
            Storyboard.SetTargetProperty(marginAnimation, new PropertyPath(MarginProperty));
            storyboard.Children.Add(marginAnimation);
            storyboard.Begin(this.SelectionRangeBackground);

            _updateSelectionRangePending = false;
        }

        private void HideHint()
        {
            var storyboard = new Storyboard();
            var disappearAnimation = new DoubleAnimation(0.0, new Duration(TimeSpan.FromSeconds(0.1)));
            Storyboard.SetTargetProperty(disappearAnimation, new PropertyPath(OpacityProperty));
            storyboard.Children.Add(disappearAnimation);

            storyboard.Completed += this.HintAnimation_Completed;

            storyboard.Begin(this.Hint);
        }



        private void UpdateHint()
        {
            if (!this.TicksContainer.IsMeasureValid)
            {
                _updateHintPending = true;
                return;
            }

            if (this.HasHintValue)
                this.Hint.Visibility = Visibility.Visible;
            else
            {
                this.Hint.Visibility = Visibility.Hidden;
                return;
            }

            var storyboard = new Storyboard();
            var duration = new Duration(TimeSpan.FromSeconds(0.2));
            var marginAnimation = new ThicknessAnimation(new Thickness(0, this.GetTickOffset(this.HintValue), 0, 0), new Duration(TimeSpan.FromSeconds(0.2)));
            Storyboard.SetTargetProperty(marginAnimation, new PropertyPath(MarginProperty));
            storyboard.Children.Add(marginAnimation);

            var appearAnimation = new DoubleAnimation(1.0, new Duration(TimeSpan.FromSeconds(0.1)));
            Storyboard.SetTargetProperty(appearAnimation, new PropertyPath(OpacityProperty));
            storyboard.Children.Add(appearAnimation);

            //var disappearAnimation = new DoubleAnimation(0.0, new Duration(TimeSpan.FromSeconds(0.1)));
            //disappearAnimation.BeginTime = TimeSpan.FromSeconds(4.0);
            //Storyboard.SetTargetProperty(disappearAnimation, new PropertyPath(Grid.OpacityProperty));
            //storyboard.Children.Add(disappearAnimation);

            //storyboard.Completed += HintAnimation_Completed;

            storyboard.Begin(this.Hint);

            _updateHintPending = false;
        }

	    private void HintAnimation_Completed(object sender, EventArgs e)
        {
            this.HasHintValue = false;
        }

        private int GetNearestTickIndex(double tickValue)
        {
            var tickIndex = 0;
            if (tickValue <= _sortedTicks[_sortedTicks.Length - 1])
                tickIndex = _sortedTicks.Length - 1;
            else
            {
                for (; tickIndex < _sortedTicks.Length; ++tickIndex)
                    if (_sortedTicks[tickIndex] <= tickValue)
                        break;
            }

            return tickIndex;
        }

        private void UpdateTickPosition()
        {
            if (this.TickPosition == HandSide.Left)
            {
                this.SpectrumContainer.HorizontalAlignment = HorizontalAlignment.Left;
                this.Hint.RenderTransform = new ScaleTransform(1.0, 1.0);
                this.TicksContainer.Margin = new Thickness(17, 0, 5, 0);
            }
            else
            {
                this.SpectrumContainer.HorizontalAlignment = HorizontalAlignment.Right;
                this.Hint.RenderTransform = new ScaleTransform(-1.0, 1.0);
                this.TicksContainer.Margin = new Thickness(5, 0, 17, 0);
            }
        }


        private double GetTickBoundary(double tickValue, bool relativeToBottom = false)
        {
            var tickIndex = this.GetNearestTickIndex(tickValue);

            if (relativeToBottom && tickIndex == this.TicksContainer.Children.Count - 1)
                return this.TicksContainer.TranslatePoint(new Point(0, 0), this.RootContainer).Y + this.TicksContainer.ActualHeight;
	        if (!relativeToBottom && tickIndex == 0)
		        return this.TicksContainer.TranslatePoint(new Point(0, 0), this.RootContainer).Y;

	        TextBlock tickLabel, nextTickLabel;
            if (relativeToBottom)
            {
                tickLabel = (TextBlock)this.TicksContainer.Children[tickIndex];
                nextTickLabel = (TextBlock)this.TicksContainer.Children[tickIndex + 1];
            }
            else
            {
                tickLabel = (TextBlock)this.TicksContainer.Children[tickIndex - 1];
                nextTickLabel = (TextBlock)this.TicksContainer.Children[tickIndex];
            }

            var offset = tickLabel.TranslatePoint(new Point(0, 0), this.RootContainer);
            var nextOffset = nextTickLabel.TranslatePoint(new Point(0, 0), this.RootContainer);
            return (offset.Y + tickLabel.ActualHeight + nextOffset.Y) / 2;

        }

        private double GetTickOffset(double tickValue, bool relativeToBottom = false)
        {
            var tickLabel = (TextBlock)this.TicksContainer.Children[this.GetNearestTickIndex(tickValue)];

            var offset = tickLabel.TranslatePoint(new Point(0, 0), this.RootContainer);
            if (relativeToBottom)
                return offset.Y + tickLabel.ActualHeight;
	        return offset.Y;
        }

        private int TickLabelHitTest()
        {

            var cursorPos = Mouse.GetPosition(this.TicksContainer);
            var zeroPoint = new Point(0, 0);
            for (int i = this.TicksContainer.Children.Count - 1; i >= 0; --i)
            {
                if (this.TicksContainer.Children[i].TranslatePoint(zeroPoint, this.TicksContainer).Y <= cursorPos.Y)
                    return i;
            }

            return 0;
        }

        private void ArmorIndicator_LeftMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.ReleaseMouseCapture();

            var storyboard = new Storyboard();
            var duration = new Duration(TimeSpan.FromSeconds(0.1));
            var brightnessAnimation = new DoubleAnimation(0.0, duration);
            brightnessAnimation.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTargetProperty(brightnessAnimation, new PropertyPath("(Grid.Effect).(effects:BrightContrastEffect.Brightness)"));
            storyboard.Children.Add(brightnessAnimation);
            storyboard.Begin(this.SelectionRangeBackground);
        }

        private void ArmorIndicator_LeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_doubleClickPending)
            {
                var tickIndex = this.TickLabelHitTest();
                var tick = _sortedTicks[tickIndex];
                this.SetSelectionRange(tick, _sortedTicks[0]);
                _doubleClickPending = false;
            }
            else
            {

                this.CaptureMouse();
                var tickIndex = this.TickLabelHitTest();
                var tick = _sortedTicks[tickIndex];
                this.SetSelectionRange(tick, tick);

                _doubleClickPending = false;

                this.HideHint();

                var storyboard = new Storyboard();
                var duration = new Duration(TimeSpan.FromSeconds(0.1));
                var brightnessAnimation = new DoubleAnimation(0.2, duration);
                Storyboard.SetTargetProperty(brightnessAnimation, new PropertyPath("(Grid.Effect).(effects:BrightContrastEffect.Brightness)"));
                storyboard.Children.Add(brightnessAnimation);
                storyboard.Begin(this.SelectionRangeBackground);
            }
        }

        private void ArmorIndicator_MouseMove(object sender, MouseEventArgs e)
        {
            var tickIndex = this.TickLabelHitTest();
            var tick = _sortedTicks[tickIndex];

            if (e.LeftButton == MouseButtonState.Pressed && !_doubleClickPending)
            {
                this.SelectionEnd = tick;
            }
            else
            {
                this.HasHintValue = true;
                this.HintValue = tick;
            }
        }

        private void TicksContainer_LayoutUpdated(object sender, EventArgs e)
        {
            if (_updateHintPending)
                this.UpdateHint();

            if (_updateSelectionRangePending)
                this.UpdateSelectionRange();
        }

        private void ClearSelectionRangeButton_Click(object sender, RoutedEventArgs e)
        {
            this.SelectAll();
        }

        private void SetSelectionRange(double begin, double end)
        {
            _preventUpdateSelectionRange = true;
            this.SelectionBegin = begin;
            this.SelectionEnd = end;
            _preventUpdateSelectionRange = false;
            this.UpdateSelectionRange();
        }

        private void SelectAll()
        {
            this.SetSelectionRange(_sortedTicks[_sortedTicks.Length - 1], _sortedTicks[0]);
        }

        private void ArmorIndicator_MouseLeave(object sender, MouseEventArgs e)
        {
            this.HideHint();
        }

        private void ArmorIndicator_MouseEnter(object sender, MouseEventArgs e)
        {
            this.HasHintValue = true;
        }

        private void ArmorIndicator_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.SelectAll();
        }

        private void ArmorIndicator_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _doubleClickPending = true;
        }
    }
}
