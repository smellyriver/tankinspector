using Smellyriver.TankInspector.Modeling;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Smellyriver.Utilities;
using System.Runtime.InteropServices;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for TurretController.xaml
    /// </summary>
    public partial class TurretController : UserControl
    {

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int x, int y);


        private static readonly DependencyPropertyKey IsControllerShownPropertyKey =
            DependencyProperty.RegisterReadOnly("IsControllerShown", typeof(bool), typeof(TurretController), new PropertyMetadata(false));

        public static readonly DependencyProperty IsControllerShownProperty = IsControllerShownPropertyKey.DependencyProperty;

        public bool IsControllerShown
        {
            get => (bool)GetValue(IsControllerShownProperty);
	        protected set => SetValue(IsControllerShownPropertyKey, value);
        }

        internal HorizontalTraverse HorizontalTraverse
        {
            get => (HorizontalTraverse)GetValue(HorizontalTraverseProperty);
	        set => SetValue(HorizontalTraverseProperty, value);
        }

        public static readonly DependencyProperty HorizontalTraverseProperty =
            DependencyProperty.Register("HorizontalTraverse", typeof(object), typeof(TurretController), new PropertyMetadata(null, TurretController.OnTraverseChanged));

        internal GunVerticalTraverse VerticalTraverse
        {
            get => (GunVerticalTraverse)GetValue(VerticalTraverseProperty);
	        set => SetValue(VerticalTraverseProperty, value);
        }

        public static readonly DependencyProperty VerticalTraverseProperty =
            DependencyProperty.Register("VerticalTraverse", typeof(object), typeof(TurretController), new PropertyMetadata(null, TurretController.OnTraverseChanged));



        public double TurretTraverseSpeed
        {
            get => (double)GetValue(TurretTraverseSpeedProperty);
	        set => SetValue(TurretTraverseSpeedProperty, value);
        }

        public static readonly DependencyProperty TurretTraverseSpeedProperty =
            DependencyProperty.Register("TurretTraverseSpeed", typeof(double), typeof(TurretController), new PropertyMetadata(30.0));



        public double GunTraverseSpeed
        {
            get => (double)GetValue(GunTraverseSpeedProperty);
	        set => SetValue(GunTraverseSpeedProperty, value);
        }

        public static readonly DependencyProperty GunTraverseSpeedProperty =
            DependencyProperty.Register("GunTraverseSpeed", typeof(double), typeof(TurretController), new PropertyMetadata(30.0));




        public bool UseRealTraverseMode
        {
            get => (bool)GetValue(UseRealTraverseModeProperty);
	        set => SetValue(UseRealTraverseModeProperty, value);
        }

        public static readonly DependencyProperty UseRealTraverseModeProperty =
            DependencyProperty.Register("UseRealTraverseMode", typeof(bool), typeof(TurretController), new PropertyMetadata(true, TurretController.OnUseRealTraverseModeChanged));

        private static void OnUseRealTraverseModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue != (bool)e.OldValue)
                ((TurretController)d).OnUseRealTraverseModeChanged();
        }

        public double VehicleYaw
        {
            get => (double)GetValue(VehicleYawProperty);
	        set => SetValue(VehicleYawProperty, value);
        }

        public static readonly DependencyProperty VehicleYawProperty =
            DependencyProperty.Register("VehicleYaw", typeof(double), typeof(UserControl), new PropertyMetadata(0.0, TurretController.OnVehicleYawChanged));

        private static void OnVehicleYawChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TurretController)d).UpdateIndicators();
        }

        public TankFigureType TankFigureType
        {
            get => (TankFigureType)GetValue(TankFigureTypeProperty);
	        set => SetValue(TankFigureTypeProperty, value);
        }

        public static readonly DependencyProperty TankFigureTypeProperty =
            DependencyProperty.Register("TankFigureType", typeof(TankFigureType), typeof(TurretController), new PropertyMetadata(TankFigureType.Tank, TurretController.OnTankFigureTypeChanged));

        private static void OnTankFigureTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TurretController)d).UpdateTankFigure();
        }

        private static readonly DependencyProperty GunPitchProperty
            = DependencyProperty.Register("GunPitch", typeof(double), typeof(TurretController), new FrameworkPropertyMetadata(0.0, TurretController.OnGunPitchChanged));

        public double GunPitch
        {
            get => (double)GetValue(GunPitchProperty);
	        set => SetValue(GunPitchProperty, value);
        }

        public static readonly DependencyProperty TurretYawProperty
            = DependencyProperty.Register("TurretYaw", typeof(double), typeof(TurretController), new FrameworkPropertyMetadata(0.0, TurretController.OnTurretYawChanged));

        private static void OnTurretYawChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TurretController)d).UpdateIndicators();
        }

        private static void OnGunPitchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TurretController)d).UpdateIndicators();
        }

        public double TurretYaw
        {
            get => (double)GetValue(TurretYawProperty);
	        set => SetValue(TurretYawProperty, value);
        }

        private static void OnTraverseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TurretController)d).OnTraverseChanged();
        }


        private double _previousGunPitch;
        private double _previousTurretYaw;

        private double _destinationGunPitch;
        private double _destinationTurretYaw;

        private DateTime _previousUpdateTime;


	    private Func<double, double> _elevationRadiusConverter;
	    private Func<double, double> _depressionRadiusConverter;
	    private Func<double, double> _inverseVerticalTraverseConverter;

        private bool _isMouseInside = false;

        public TurretController()
        {
            InitializeComponent();
            this.VehicleYaw = 90;
            this.BoundaryCanvas.SizeChanged += BoundaryCanvas_SizeChanged;

            this.UpdateTankFigure();
            this.OnUseRealTraverseModeChanged();

        }

        private void UpdateTankFigure()
        {
            Uri turretUri, hullUri;
            switch (this.TankFigureType)
            {
                case TankFigureType.Tank:
                    turretUri = new Uri("pack://application:,,,/Smellyriver.TankInspector;component/Resources/Images/GameElements/TankTurret.png", UriKind.Absolute);
                    hullUri = new Uri("pack://application:,,,/Smellyriver.TankInspector;component/Resources/Images/GameElements/TankHull.png", UriKind.Absolute);
                    break;
                case TankFigureType.TankDestroyer:
                    turretUri = new Uri("pack://application:,,,/Smellyriver.TankInspector;component/Resources/Images/GameElements/TDGun.png", UriKind.Absolute);
                    hullUri = new Uri("pack://application:,,,/Smellyriver.TankInspector;component/Resources/Images/GameElements/TDHull.png", UriKind.Absolute);
                    break;
                case TankFigureType.SelfPropelledGun:
                    turretUri = new Uri("pack://application:,,,/Smellyriver.TankInspector;component/Resources/Images/GameElements/SPGGun.png", UriKind.Absolute);
                    hullUri = new Uri("pack://application:,,,/Smellyriver.TankInspector;component/Resources/Images/GameElements/SPGHull.png", UriKind.Absolute);
                    break;
                default:
                    throw new ArgumentException();
            }

            this.HullFigure.Source = new BitmapImage(hullUri);
            this.TurretFigure.Source = new BitmapImage(turretUri);
        }

	    private void BoundaryCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateTraverseBoundary();
        }

        private void OnUseRealTraverseModeChanged()
        {
            if (this.UseRealTraverseMode)
            {
                CompositionTarget.Rendering += UpdateTraverse;
                _previousUpdateTime = DateTime.Now;
            }
            else
            {
                CompositionTarget.Rendering -= UpdateTraverse;
                this.UpdateTraverse();
            }
        }

        private void UpdateTraverse(object sender, EventArgs e)
        {
            this.UpdateTraverse();
        }

        private void UpdateTraverse()
        {
            if (UseRealTraverseMode)
            {
                var now = DateTime.Now;
                var deltaTime = now - _previousUpdateTime;
                var turretYaw = GunTraverseHelper.RotateAngle(
                    this.TurretYaw,
                    _destinationTurretYaw,
                    deltaTime.TotalSeconds * this.TurretTraverseSpeed,
                    this.HorizontalTraverse == null ? 180.0 : this.HorizontalTraverse.Right,
                    this.HorizontalTraverse == null ? 180.0 : this.HorizontalTraverse.Left);

                double elevation, depression;
                if (this.VerticalTraverse == null)
                {
                    elevation = 0;
                    depression = 0;
                }
                else
                {
                    var pitchLimits = this.VerticalTraverse.GetPitchLimits(turretYaw);
                    elevation = pitchLimits.Elevation;
                    depression = pitchLimits.Depression;
                }

                var gunPitch = GunTraverseHelper.RotateAngle(
                    this.GunPitch + 180,
                    _destinationGunPitch + 180,
                    deltaTime.TotalSeconds * this.GunTraverseSpeed,
                    elevation,
                    depression) - 180;

                this.ClampRotation(ref turretYaw, ref gunPitch);
                this.TurretYaw = turretYaw;
                this.GunPitch = gunPitch;

                _previousUpdateTime = now;
            }
            else
            {
                this.GunPitch = _destinationGunPitch;
                this.TurretYaw = _destinationTurretYaw;
            }
        }


        private void OnTraverseChanged()
        {
            this.UpdateTraverseBoundary();
            this.SetRotation(_destinationTurretYaw, _destinationGunPitch, true); // limit the rotation within new traverse boundary

            if (this.HorizontalTraverse == null || this.HorizontalTraverse.Range >= 180)
                this.TankFigureType = TankFigureType.Tank;
            else
            {
                if (this.VerticalTraverse != null && this.VerticalTraverse.Elevation >= 45)
                    this.TankFigureType = TankFigureType.SelfPropelledGun;
                else
                    this.TankFigureType = TankFigureType.TankDestroyer;
            }
        }

        private void GetDimentions(out double size, out double radius, out Point center)
        {
            size = Math.Min(this.Root.ActualWidth, this.Root.ActualHeight);
            radius = size / 2;
            center = new Point(radius, radius);
        }

        private void UpdateTraverseBoundary()
        {
            if (this.VerticalTraverse == null)
            {
                this.BoundaryCanvas.Children.Clear();
                return;
            }

			this.GetDimentions(out double size, out double radius, out Point center);
			var oneThirdsRadius = radius / 3;

            var elevation = new GunElevationViewModel(this.VerticalTraverse, this.HorizontalTraverse);
            var depression = new GunDepressionViewModel(this.VerticalTraverse, this.HorizontalTraverse);

            var maxElevation = elevation.MaximumTraverse;
            var maxDepression = depression.MaximumTraverse;
            var maxTraverse = maxElevation + maxDepression;

            if (Math.Abs(maxTraverse) < 0.000001)
                return;

            // elevation/outer figure

            _elevationRadiusConverter = r => (r + maxDepression) / maxTraverse * oneThirdsRadius * 2 + oneThirdsRadius;
            _depressionRadiusConverter = r => (maxDepression - r) / maxTraverse * oneThirdsRadius * 2 + oneThirdsRadius;
            _inverseVerticalTraverseConverter = r => (r - oneThirdsRadius) / (oneThirdsRadius * 2) * maxTraverse - maxDepression;

            var elevationGeometry = GunTraverseHelper.CreateGeometry(elevation, center, _elevationRadiusConverter);
            var depressionGeometry = GunTraverseHelper.CreateGeometry(depression, center, _depressionRadiusConverter);

            var combinedGeometry = new CombinedGeometry(GeometryCombineMode.Exclude, elevationGeometry, depressionGeometry);

            var figure = new Path();
            figure.Data = combinedGeometry;

            var borderStyle = this.FindResource("Border") as Style;
            var delimiterStyle = this.FindResource("Delimiter") as Style;

            figure.Style = borderStyle;

            var delimiterCircle = new Ellipse();
            var delimiterCircleSize = _depressionRadiusConverter(0) * 2;

            delimiterCircle.Width = delimiterCircleSize;
            delimiterCircle.Height = delimiterCircleSize;
            delimiterCircle.Style = delimiterStyle;

            Canvas.SetLeft(delimiterCircle, (size - delimiterCircleSize) / 2);
            Canvas.SetTop(delimiterCircle, (size - delimiterCircleSize) / 2);

            this.BoundaryCanvas.Children.Clear();

            this.BoundaryCanvas.Children.Add(delimiterCircle);
            this.BoundaryCanvas.Children.Add(figure);

            Canvas.SetLeft(this.YawDirectionLine, center.X);
            Canvas.SetTop(this.YawDirectionLine, center.Y);
        }

        private void UpdateRotation()
        {
            var position = Mouse.GetPosition(this.Root);
			this.GetDimentions(out double size, out double radius, out Point center);
			GunTraverseHelper.CartesianToPolar(center, position, out double distance, out double degree);
            degree -= this.VehicleYaw;
            var turretYaw = GunTraverseHelper.NormalizeAngle(degree);
            var gunPitch = _inverseVerticalTraverseConverter(distance);

            this.SetRotation(turretYaw, gunPitch);
        }

        private void ClampRotation(ref double turretYaw, ref double gunPitch)
        {
            if (this.HorizontalTraverse != null)
            {
                if (turretYaw < 180)
                    turretYaw = Math.Min(turretYaw, this.HorizontalTraverse.Left);
                else
                    turretYaw = Math.Max(turretYaw, 360 - this.HorizontalTraverse.Right);
            }

            if (this.VerticalTraverse != null)
            {
                var verticalTraverseBound = this.VerticalTraverse.GetPitchLimits(turretYaw);
                gunPitch = gunPitch.Clamp(-verticalTraverseBound.Depression, verticalTraverseBound.Elevation);
            }
        }

        private void SetRotation(double turretYaw, double gunPitch, bool overrideRealTraverse = false)
        {
            this.ClampRotation(ref turretYaw, ref gunPitch);

            _destinationTurretYaw = turretYaw;
            _destinationGunPitch = gunPitch;

            if (overrideRealTraverse)
            {
                this.TurretYaw = _destinationTurretYaw;
                this.GunPitch = _destinationGunPitch;
            }
            else if (!UseRealTraverseMode)
                this.UpdateTraverse();

            // otherwise it will be automatically updated by the timer
        }


        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.IsControllerShown)
            {
                this.IsControllerShown = false;

                this.SetRotation(_previousTurretYaw, _previousGunPitch);
            }

            _isMouseInside = false;
        }

        private void TankFigure_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!_isMouseInside && !this.IsControllerShown)
            {
                this.IsControllerShown = true;
                this.SyncGunPointers();
                _isMouseInside = true;
            }
        }

        private void SyncGunPointers()
        {
            _previousGunPitch = _destinationGunPitch;
            _previousTurretYaw = _destinationTurretYaw;
            this.SetGunPointerPos(this.OriginalGunPointer, _previousGunPitch, _previousTurretYaw);
        }

        private void UpdateIndicators()
        {
			this.GetDimentions(out double _, out _, out Point center);

			this.SetGunPointerPos(this.GunPointer, center, _destinationGunPitch, _destinationTurretYaw);

            //this.YawDirectionLine.X1 = _elevationRadiusConverter(-verticalTraverseBound.Depression);
            //this.YawDirectionLine.X2 = _elevationRadiusConverter(verticalTraverseBound.Elevation);
            //this.YawDirectionLineRotateTransform.Angle = -this.TurretYaw - this.VehicleYaw;

            //this.PitchCircle.Width = this.PitchCircle.Height = distance * 2;
            //Canvas.SetLeft(this.PitchCircle, (size - this.PitchCircle.Width) / 2);
            //Canvas.SetTop(this.PitchCircle, (size - this.PitchCircle.Height) / 2);

            this.GunFigureRotateTransform.Angle = -this.TurretYaw - this.VehicleYaw;
        }

        private void SetGunPointerPos(FrameworkElement gunPointer, double pitch, double yaw)
        {
			this.GetDimentions(out double _, out _, out Point center);
			this.SetGunPointerPos(gunPointer, center, pitch, yaw);
        }


        private Point CalculateGunPointerPos(Point center, double pitch, double yaw)
        {
            if (_elevationRadiusConverter == null)
                return new Point();

            var distance = this.GunPitch > 0 ? _elevationRadiusConverter(pitch) : _depressionRadiusConverter(-pitch);

            return GunTraverseHelper.PolarToCartesian(center, distance, yaw + this.VehicleYaw);
        }

        private void SetGunPointerPos(FrameworkElement gunPointer, Point center, double pitch, double yaw)
        {
            var gunPointerPosition = this.CalculateGunPointerPos(center, pitch, yaw);
            Canvas.SetLeft(gunPointer, gunPointerPosition.X - gunPointer.ActualWidth / 2);
            Canvas.SetTop(gunPointer, gunPointerPosition.Y - gunPointer.ActualHeight / 2);
        }

        private void HitTestPlaceHolder_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsControllerShown && e.LeftButton != MouseButtonState.Pressed)
                this.UpdateRotation();
        }

        private void HitTestPlaceHolder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsControllerShown && e.ChangedButton == MouseButton.Left)
                this.SyncGunPointers();

            this.HitTestPlaceHolder.CaptureMouse();
        }

        private void HitTestPlaceHolder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.HitTestPlaceHolder.IsMouseCaptured)
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.IsControllerShown = !this.IsControllerShown;
                else if (e.ChangedButton == MouseButton.Right)
                {
                    this.SetRotation(0, 0);
                    this.IsControllerShown = false;
                }

                this.HitTestPlaceHolder.ReleaseMouseCapture();
            }
        }

        private void HitTestPlaceHolder_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var delta = e.Delta / 120.0;
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                delta *= 360;

            var gunPitch = _destinationGunPitch + delta;
            this.ClampRotation(ref _destinationTurretYaw, ref gunPitch);

			this.GetDimentions(out double _, out _, out Point center);

			var destinationGunPointerPos = this.CalculateGunPointerPos(center, gunPitch, _destinationTurretYaw);
            var cursorPos = this.DynamicObjectContainer.PointToScreen(destinationGunPointerPos);
            TurretController.SetCursorPos((int)Math.Round(cursorPos.X), (int)Math.Round(cursorPos.Y));
        }

    }
}
