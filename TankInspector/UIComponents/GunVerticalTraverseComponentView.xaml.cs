using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Smellyriver.TankInspector.UIComponents
{
    public partial class GunVerticalTraverseComponentView : UserControl
    {



        internal GunVerticalTraverseComponentViewModel VerticalTraverse
        {
            get => (GunVerticalTraverseComponentViewModel)GetValue(VerticalTraverseProperty);
	        set => SetValue(VerticalTraverseProperty, value);
        }

        // Using a DependencyProperty as the backing store for VerticalTraverse.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalTraverseProperty =
            DependencyProperty.Register("VerticalTraverse", typeof(object), typeof(GunVerticalTraverseComponentView), new PropertyMetadata(null, GunVerticalTraverseComponentView.OnVerticalTraverseChanged));

        private static void OnVerticalTraverseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GunVerticalTraverseComponentView)d).OnVerticalTraverseChanged((GunVerticalTraverseComponentViewModel)e.OldValue, (GunVerticalTraverseComponentViewModel)e.NewValue);
        }

        public GunVerticalTraverseComponentView()
        {
            InitializeComponent();
        }

        private static void SetVisibility(UIElement target, bool visible)
        {
            target.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }

        private static string FormatAngle(double angle)
        {
            return string.Format(App.GetLocalizedString("UnitDegrees"), angle.ToString("#,0.#"));
        }

        private void OnVerticalTraverseChanged(GunVerticalTraverseComponentViewModel oldValue, GunVerticalTraverseComponentViewModel newValue)
        {

            this.DrawTraverseFigure(newValue);

            if (newValue == null)
            {
                GunVerticalTraverseComponentView.SetVisibility(this.FrontLeftBound, false);
                GunVerticalTraverseComponentView.SetVisibility(this.FrontLeftBoundAngleDisplayContainer, false);
                GunVerticalTraverseComponentView.SetVisibility(this.FrontLeftTransitionBound, false);
                GunVerticalTraverseComponentView.SetVisibility(this.FrontRightBound, false);
                GunVerticalTraverseComponentView.SetVisibility(this.FrontRightBoundAngleDisplayContainer, false);
                GunVerticalTraverseComponentView.SetVisibility(this.FrontRightTransitionBound, false);
                GunVerticalTraverseComponentView.SetVisibility(this.BackLeftBound, false);
                GunVerticalTraverseComponentView.SetVisibility(this.BackLeftBoundAngleDisplayContainer, false);
                GunVerticalTraverseComponentView.SetVisibility(this.BackLeftTransitionBound, false);
                GunVerticalTraverseComponentView.SetVisibility(this.BackRightBound, false);
                GunVerticalTraverseComponentView.SetVisibility(this.BackRightBoundAngleDisplayContainer, false);
                GunVerticalTraverseComponentView.SetVisibility(this.BackRightTransitionBound, false);

                return;
            }

            GunVerticalTraverseComponentView.SetVisibility(this.FrontLeftBound, newValue.HasExtraFrontPitchLimits);
            GunVerticalTraverseComponentView.SetVisibility(this.FrontLeftBoundAngleDisplayContainer, newValue.HasExtraFrontPitchLimits);
            GunVerticalTraverseComponentView.SetVisibility(this.FrontLeftTransitionBound, newValue.HasExtraFrontPitchLimits && newValue.ExtraPitchLimitsTransition > 0);
            GunVerticalTraverseComponentView.SetVisibility(this.FrontRightBound, newValue.HasExtraFrontPitchLimits);
            GunVerticalTraverseComponentView.SetVisibility(this.FrontRightBoundAngleDisplayContainer, newValue.HasExtraFrontPitchLimits);
            GunVerticalTraverseComponentView.SetVisibility(this.FrontRightTransitionBound, newValue.HasExtraFrontPitchLimits && newValue.ExtraPitchLimitsTransition > 0);
            GunVerticalTraverseComponentView.SetVisibility(this.BackLeftBound, newValue.HasExtraBackPitchLimits);
            GunVerticalTraverseComponentView.SetVisibility(this.BackLeftBoundAngleDisplayContainer, newValue.HasExtraBackPitchLimits);
            GunVerticalTraverseComponentView.SetVisibility(this.BackLeftTransitionBound, newValue.HasExtraBackPitchLimits && newValue.ExtraPitchLimitsTransition > 0);
            GunVerticalTraverseComponentView.SetVisibility(this.BackRightBound, newValue.HasExtraBackPitchLimits);
            GunVerticalTraverseComponentView.SetVisibility(this.BackRightBoundAngleDisplayContainer, newValue.HasExtraBackPitchLimits);
            GunVerticalTraverseComponentView.SetVisibility(this.BackRightTransitionBound, newValue.HasExtraBackPitchLimits && newValue.ExtraPitchLimitsTransition > 0);

            if (newValue.HasExtraFrontPitchLimits)
            {
                var halfRange = newValue.ExtraFrontPitchLimitsRange / 2;
                var boundary = GunVerticalTraverseComponentView.FormatAngle(halfRange);
                this.FrontLeftBoundAngleDisplay.Text = boundary;
                this.FrontLeftBound.RenderTransform = new RotateTransform(360 - halfRange);
                this.FrontLeftBoundAngleDisplayContainer.RenderTransform = new RotateTransform(360 - halfRange);
                this.FrontRightBoundAngleDisplay.Text = boundary;
                this.FrontRightBoundAngleDisplayContainer.RenderTransform = new RotateTransform(halfRange);
                this.FrontRightBound.RenderTransform = new RotateTransform(halfRange);

                if (newValue.ExtraPitchLimitsTransition > 0)
                {
                    halfRange = newValue.ExtraFrontPitchLimitsRange / 2 + newValue.ExtraPitchLimitsTransition;
                    boundary = GunVerticalTraverseComponentView.FormatAngle(halfRange);
                    this.FrontLeftTransitionBound.RenderTransform = new RotateTransform(360 - halfRange);
                    this.FrontRightTransitionBound.RenderTransform = new RotateTransform(halfRange);
                }
            }

            if (newValue.HasExtraBackPitchLimits)
            {
                var halfRange = newValue.ExtraBackPitchLimitsRange / 2;
                var boundary = GunVerticalTraverseComponentView.FormatAngle(halfRange);
                this.BackLeftBoundAngleDisplay.Text = boundary;
                this.BackLeftBound.RenderTransform = new RotateTransform(180 + halfRange);
                this.BackLeftBoundAngleDisplayContainer.RenderTransform = new RotateTransform(180 + halfRange);
                this.BackRightBoundAngleDisplay.Text = boundary;
                this.BackRightBound.RenderTransform = new RotateTransform(180 - halfRange);
                this.BackRightBoundAngleDisplayContainer.RenderTransform = new RotateTransform(180 - halfRange);

                if (newValue.ExtraPitchLimitsTransition > 0)
                {
                    halfRange = newValue.ExtraBackPitchLimitsRange / 2 + newValue.ExtraPitchLimitsTransition;
                    boundary = GunVerticalTraverseComponentView.FormatAngle(halfRange);
                    this.BackLeftTransitionBound.RenderTransform = new RotateTransform(180 + halfRange);
                    this.BackRightTransitionBound.RenderTransform = new RotateTransform(180 - halfRange);
                }
            }

            var max = newValue.MaximumTraverse;
            var min = newValue.MinimumTraverse;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (max == min)
                this.TraverseRangeDisplay.Text = GunVerticalTraverseComponentView.FormatAngle(max);
            else
                this.TraverseRangeDisplay.Text =
		                $"{GunVerticalTraverseComponentView.FormatAngle(min)} - {GunVerticalTraverseComponentView.FormatAngle(max)}";
        }

        private void DrawTraverseFigure(GunVerticalTraverseComponentViewModel traverse)
        {
            this.CurveCanvas.Children.Clear();

            if (traverse == null)
                return;

            var traverseFigureStrokeStyle = this.FindResource("TraverseFigure") as Style;

            var maxRadius = traverse.MaximumTraverse;
            //maxRadius = Math.Ceiling(maxRadius / 5) * 5;
            const double margin = 1;

            var scale = 80 / (maxRadius + margin);
            var size = scale * maxRadius * 2;
            var center = new Point(100, 100);
            var figurePath = new Path();
            figurePath.Data = GunTraverseHelper.CreateGeometry(traverse, size, center, 90, scale * margin, verticalTraverseTransform: v => v + 1);
            figurePath.Style = traverseFigureStrokeStyle;

            this.CurveCanvas.Children.Add(figurePath);


            // draw references
            var thinReferenceGeometry = new GeometryGroup();
            var thickReferenceGeometry = new GeometryGroup();

            const double referenceCircleGap = 20;
            const double halfReferenceCircleGap = referenceCircleGap / 2;

            var sign = Math.Sign(maxRadius);
            var absMaxRadius = Math.Abs(maxRadius);

            var thickDivisor = absMaxRadius > 45 ? 20 : absMaxRadius > 25 ? 10 : 5;
            var thinDivisor = absMaxRadius > 25 ? 5 : 1;

            for (var i = thinDivisor; i <= absMaxRadius; i += thinDivisor)
            {
                var radius = scale * i * sign;

                Geometry geometry;

                var isThickRing = i % thickDivisor == 0;
                if (absMaxRadius < 10 || isThickRing)
                {
                    var gapHalfAngle = Math.Asin(halfReferenceCircleGap / radius) * 180 / Math.PI;
                    if (double.IsNaN(gapHalfAngle))
                        gapHalfAngle = 180;
                    var startPoint = GunTraverseHelper.PolarToCartesian(center, radius, gapHalfAngle);
                    var endPoint = GunTraverseHelper.PolarToCartesian(center, radius, 360 - gapHalfAngle);
                    var arc = new ArcSegment(endPoint, new Size(radius, radius), 360 - gapHalfAngle * 2, true, SweepDirection.Counterclockwise, true);
                    var figure = new PathFigure(startPoint, new[] { arc }, false);
                    geometry = new PathGeometry(new[] { figure });


                    var referenceTextContainer = new Grid
                    {
                        Width = 20,
                        Height = referenceCircleGap
                    };
                    var referenceTextContainerPosition = GunTraverseHelper.PolarToCartesian(center, radius, 0);
                    Canvas.SetLeft(referenceTextContainer, referenceTextContainerPosition.X - 10);
                    Canvas.SetTop(referenceTextContainer, referenceTextContainerPosition.Y - halfReferenceCircleGap);

                    var referenceText = new TextBlock
                    {
                        Text = (i * sign).ToString(),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = isThickRing ? 10 : 8,
                        Foreground = isThickRing ? Brushes.White : new SolidColorBrush(Color.FromArgb(0x80, 0xff, 0xff, 0xff))
                    };


                    referenceTextContainer.Children.Add(referenceText);

                    this.CurveCanvas.Children.Add(referenceTextContainer);
                }
                else
                {
                    geometry = new EllipseGeometry(center, radius, radius);
                }

                if (isThickRing)
                    thickReferenceGeometry.Children.Add(geometry);
                else
                    thinReferenceGeometry.Children.Add(geometry);
            }

            var thinReferencePath = new Path
            {
                Data = thinReferenceGeometry,
                Style = this.FindResource("ThinReferenceStroke") as Style
            };
            this.CurveCanvas.Children.Add(thinReferencePath);

            var thickReferencePath = new Path
            {
                Data = thickReferenceGeometry,
                Style = this.FindResource("ThickReferenceStroke") as Style
            };
            this.CurveCanvas.Children.Add(thickReferencePath);

        }

    }
}
