using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Smellyriver.TankInspector.Graphics;
using Smellyriver.TankInspector.Design;
using System.Runtime.InteropServices;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for TrackballBorder.xaml
    /// </summary>
    public partial class Capture3DInputBorder : UserControl
    {
        private readonly Trackball _trackball;
        private DateTime _lastTime;

        public Transform3D Transform
        {
            get => (Transform3D)GetValue(TransformProperty);
	        set => SetValue(TransformProperty, value);
        }

        // Using a DependencyProperty as the backing store for Transform.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(Transform3D), typeof(Capture3DInputBorder), new PropertyMetadata(Transform3D.Identity));


        

        public double VehicleYaw
        {
            get => (double)GetValue(VehicleYawProperty);
	        set => SetValue(VehicleYawProperty, value);
        }

        // Using a DependencyProperty as the backing store for VehicleYaw.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VehicleYawProperty = 
            DependencyProperty.Register("VehicleYaw", typeof(double), typeof(Capture3DInputBorder), new PropertyMetadata(0.0));


        public double Zoom
        {
            get => (double)GetValue(ZoomProperty);
	        set => SetValue(ZoomProperty, value);
        }

        // Using a DependencyProperty as the backing store for Zoom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register("Zoom", typeof(double), typeof(Capture3DInputBorder), new PropertyMetadata(0.0));

        


        public CameraMode CameraMode
        {
            get => (CameraMode)GetValue(CarmeraModeProperty);
	        set => SetValue(CarmeraModeProperty, value);
        }

        // Using a DependencyProperty as the backing store for CarmeraMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CarmeraModeProperty =
            DependencyProperty.Register("CameraMode", typeof(CameraMode), typeof(Capture3DInputBorder), new PropertyMetadata(CameraMode.TrackBall,
                (d, e) => ((Capture3DInputBorder)d).OnCameraModeChanged((CameraMode)e.OldValue, (CameraMode)e.NewValue)));

        public Vector3D LookDirection
        {
            get => (Vector3D)GetValue(LookDirectionProperty);
	        set => SetValue(LookDirectionProperty, value);
        }

        // Using a DependencyProperty as the backing store for LookDirection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LookDirectionProperty =
            DependencyProperty.Register("LookDirection", typeof(Vector3D), typeof(Capture3DInputBorder), new PropertyMetadata(new Vector3D(0,0,1)));

        private Point _lastPoint;

        public Capture3DInputBorder()
        {
            InitializeComponent();
            _trackball = new Trackball();  
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

	    private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            var thisTime = DateTime.Now;
            _trackball.UpdateInertia((thisTime - _lastTime).TotalSeconds);
            _lastTime = thisTime;

            Transform = _trackball.Transform;
            LookDirection = _trackball.LookDirection;
            VehicleYaw = 90.0 - _trackball.YawFactor;

            if (CameraMode == CameraMode.Sniper)
            {
                Zoom = _trackball.ZoomFactor * 2;
            }
            else
            {
                Zoom = _trackball.ZoomFactor;
            }
        }

        private void captureBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CameraMode == CameraMode.TrackBall)
            {
                Mouse.Capture(captureBorder, CaptureMode.Element);
                _trackball.TrackStart(e.GetPosition(captureBorder), captureBorder.ActualWidth, captureBorder.ActualHeight);           
            }
        }

        private void captureBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (CameraMode == CameraMode.TrackBall)
            {
                Mouse.Capture(captureBorder, CaptureMode.None);
                _trackball.TrackEnd();
            }
        }

        private void captureBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (CameraMode == CameraMode.TrackBall)
            {    
                _trackball.Track(e.GetPosition(captureBorder), captureBorder.ActualWidth, captureBorder.ActualHeight);
            }
            else if (CameraMode == CameraMode.Sniper)
            {       
                var point = e.GetPosition(captureBorder);

                if (!this.IsfristSniperFrame)
                {
                    _trackball.TrackLook(new Point(point.X - _lastPoint.X, point.Y - _lastPoint.Y));
                }
                else
                {
                    this.IsfristSniperFrame = false;
                }

                _lastPoint = point;

                var center = PointToScreen(new Point(captureBorder.ActualWidth * 0.5 , captureBorder.ActualHeight*0.5));

                int x = (int)center.X;
                int y = (int)center.Y;

                var pointToScreen = PointToScreen(point);
                var dx = pointToScreen.X - center.X;
                var dy = pointToScreen.Y - center.Y;

                if (Math.Abs(dx) > 100 || Math.Abs(dy) > 100)
                {
                    Capture3DInputBorder.SetCursorPos(x, y);
                    _lastPoint.X -= dx;
                    _lastPoint.Y -= dy;
                }
            }
        }

        private void OnCameraModeChanged(CameraMode cameraMode1, CameraMode newMode)
        {
            if (newMode == CameraMode.Sniper)
            {
                _lastPoint = Mouse.GetPosition(captureBorder);
                this.IsfristSniperFrame = true;
            }
        }

        private void captureBorder_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            _trackball.Zoom(e.Delta);
     
        }

        /// <summary>   
        /// 设置鼠标的坐标   
        /// </summary>   
        /// <param name="x">横坐标</param>   
        /// <param name="y">纵坐标</param>   
        [DllImport("User32")]
        public extern static void SetCursorPos(int x, int y);


        public bool IsfristSniperFrame { get; set; }
    }
}
