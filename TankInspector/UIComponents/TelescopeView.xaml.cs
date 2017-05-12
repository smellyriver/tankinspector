using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for TelescopeView.xaml
    /// </summary>
    public partial class TelescopeView : UserControl
    {

        private static LineGeometry CreateRadialLineSegment(double centerX, double centerY, double distance, double angle, double length)
        {
            var halfLength = length / 2;
            var d1 = distance - halfLength;
            var d2 = distance + halfLength;
            var x1 = centerX + d1 * Math.Sin(angle);
            var y1 = centerY + d1 * Math.Cos(angle);
            var x2 = centerX + d2 * Math.Sin(angle);
            var y2 = centerY + d2 * Math.Cos(angle); ;
            return new LineGeometry(new Point(x1, y1), new Point(x2, y2));
        }

        public double ZoomRatio
        {
            get => (double)GetValue(ZoomRatioProperty);
	        set => SetValue(ZoomRatioProperty, value);
        }

        public static readonly DependencyProperty ZoomRatioProperty =
            DependencyProperty.Register("ZoomRatio", typeof(double), typeof(TelescopeView), new PropertyMetadata(1.0, TelescopeView.OnZoomRatioChanged));

        private static void OnZoomRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TelescopeView)d).OnZoomRatioChanged((double)e.OldValue, (double)e.NewValue);
        }

        public double Distance
        {
            get => (double)GetValue(DistanceProperty);
	        set => SetValue(DistanceProperty, value);
        }

        public static readonly DependencyProperty DistanceProperty =
            DependencyProperty.Register("Distance", typeof(double), typeof(TelescopeView), new PropertyMetadata(0.0, TelescopeView.OnDistanceChanged));

        private static void OnDistanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TelescopeView)d).OnDistanceChanged((double)e.OldValue, (double)e.NewValue);
        }

        public TelescopeView()
        {
            InitializeComponent();
        }


        private void OnDistanceChanged(double oldDistance, double newDistance)
        {
            this.DistanceText.Text = string.Format(App.GetLocalizedString("UnitMeters"), newDistance.ToString("F1"));
        }

        private void OnZoomRatioChanged(double oldZoomRatio, double newZoomRatio)
        {
            this.ZoomRatioText.Text = string.Format(App.GetLocalizedString("UnitTimes"), newZoomRatio.ToString("0.#"));
        }

        private void AngleGaugesContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateAngleGauges();
        }

        private void UpdateAngleGauges()
        {
            var centerX = this.AngleGaugesContainer.ActualWidth / 2;
            var centerY = this.AngleGaugesContainer.ActualHeight / 2;

            var group = new GeometryGroup();

            var distance = this.AngleGaugesContainer.ActualWidth / 2 - 40;

            // large segements in quadrant 2 and 4
            for (int i = 1; i < 4; ++i)
            {
                var angle = Math.PI / 8 * i;
                group.Children.Add(TelescopeView.CreateRadialLineSegment(centerX, centerY, distance, angle, 20));
                group.Children.Add(TelescopeView.CreateRadialLineSegment(centerX, centerY, distance, angle + Math.PI, 20));
            }

            // small segements in quadrant 2 and 4
            for (int i = 1; i < 48; ++i)
            {
                if (i % 12 == 0)
                    continue;

                var angle = Math.PI / 96 * i;
                group.Children.Add(TelescopeView.CreateRadialLineSegment(centerX, centerY, distance, angle, 15));
                group.Children.Add(TelescopeView.CreateRadialLineSegment(centerX, centerY, distance, angle + Math.PI, 15));
            }

            //segements in quadrant 1 and 3
            for (int i = 0; i < 18; ++i)
            {
                var angle = Math.PI / 2 + Math.PI / 72 + Math.PI / 36 * i;
                group.Children.Add(TelescopeView.CreateRadialLineSegment(centerX, centerY, distance, angle, 5));
                group.Children.Add(TelescopeView.CreateRadialLineSegment(centerX, centerY, distance, angle + Math.PI, 5));
            }

            var path = new Path();
            path.Style = this.FindResource("ThinReticule") as Style;
            path.Data = group;
            path.HorizontalAlignment = HorizontalAlignment.Center;
            path.VerticalAlignment = VerticalAlignment.Center;
            path.Width = this.AngleGaugesContainer.ActualWidth;
            path.Height = this.AngleGaugesContainer.ActualHeight;

            this.AngleGaugesContainer.Children.Clear();
            this.AngleGaugesContainer.Children.Add(path);
        }

        public void PlayShootFlareEffect()
        {
            var storyboard = this.FindResource("ShootFlareStoryboard") as Storyboard;
            storyboard.Begin(this.ShootFlare);
        }

    }
}
