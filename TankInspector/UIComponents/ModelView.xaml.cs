using System.Windows.Controls;
using System.Windows.Input;
using Smellyriver.TankInspector.Design;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for ModelView.xaml
    /// </summary>
    public partial class ModelView : UserControl
    {
        internal ModelViewModel ViewModel
        {
            get => this.DataContext as ModelViewModel;
	        set => this.DataContext = value;
        }


        public ModelView()
        {
            InitializeComponent();
        }

        private void UpdateCursor()
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (this.ViewModel.IsMouseOverModel)
                    this.Cursor = Cursors.CrossHair;
                else
                    this.Cursor = Cursors.Drag;
            }
        }

        private void TrackballBorder_MouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition(trackballBorder);
            if (ViewModel != null)
            {
                if (this.ViewModel.CameraMode == CameraMode.TrackBall)
                {
                    ViewModel.MousePosition = point;
                    if (e.LeftButton != MouseButtonState.Pressed)
                    {
                        this.ViewModel.IsHitTestEnabled = true;
                        if (this.ViewModel.IsMouseOverModel)
                            this.Cursor = Cursors.CrossHair;
                        else
                            this.Cursor = Cursors.Drag;
                    }
                    else
                    {
                        this.ViewModel.IsHitTestEnabled = false;
                    }
                }
                else if(this.ViewModel.CameraMode == CameraMode.Sniper)
                {
                    this.Cursor = System.Windows.Input.Cursors.None;
                    this.ViewModel.IsHitTestEnabled = true;
                }
            }
            
        }

        private void TrackballBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.ViewModel.CameraMode == CameraMode.TrackBall)
            {
                this.Cursor = Cursors.Rotate;
            }

            else if (this.ViewModel.CameraMode == CameraMode.Sniper)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    hangarScene.AddProjectileTracer();
                }
            }

            if (e.RightButton == MouseButtonState.Pressed)
            {
                hangarScene.ClearAllProjectileTracer();
            }
        }

        private void TrackballBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.ViewModel.CameraMode == CameraMode.TrackBall)
                this.Cursor = Cursors.Drag;
        }


        //private void captureBorder_MouseMove_1(object sender, MouseEventArgs args)
        //{
        //    Point mousePos = args.GetPosition(this);
        //    PointHitTestParameters hitParams = new PointHitTestParameters(mousePos);

        //    Model hoverModel = null;

        //    VisualTreeHelper.HitTest(this,null,
        //        (HitTestResult result) =>
        //        {
        //            // Did we hit 3D?
        //            RayHitTestResult rayResult = result as RayHitTestResult;
        //            if (rayResult != null)
        //            {
        //                // Did we hit a MeshGeometry3D?
        //                RayMeshGeometry3DHitTestResult rayMeshResult =
        //                    rayResult as RayMeshGeometry3DHitTestResult;

        //                if (rayMeshResult != null)
        //                {
        //                    var model3D = rayMeshResult.ModelHit as GeometryModel3D;
        //                    if (model3D != null)
        //                    {
        //                        hoverModel = ModelViewModel.GetModel(model3D);
        //                        return HitTestResultBehavior.Stop;
        //                    }
        //                }
        //            }
        //            return HitTestResultBehavior.Continue;
        //        },
        //    hitParams);

        //    ViewModel.HoverModel = hoverModel;
        //}
    }
}
