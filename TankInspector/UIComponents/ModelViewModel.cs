using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using Smellyriver.TankInspector.Graphics;
using Smellyriver.TankInspector.Modeling;
using Smellyriver.Wpf;
using Smellyriver.Wpf.Converters;
using System.Windows.Input;
using Smellyriver.TankInspector.Graphics.Scene;
using Smellyriver.TankInspector.Design;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace Smellyriver.TankInspector.UIComponents
{
	internal partial class ModelViewModel : DependencyNotificationObject
    {
        public enum ModelStateEnum
        {
            Loading,
            Loaded,
        }

        private CameraMode _cameraMode;
        public CameraMode CameraMode
        {
            get => _cameraMode;
	        set
            {
                _cameraMode = value;
                this.RaisePropertyChanged(() => this.CameraMode);
            }
        }

        private ModelStateEnum _modelState;
        public ModelStateEnum ModelState
        {
            get => _modelState;
	        set
            {
                _modelState = value;
                this.RaisePropertyChanged(() => this.ModelState);
            }
        }
        private float _fov;
        public float Fov
        {
            get { return _fov; }
            set
            {
                if (value > 10.0f && value < 80.0f)
                {
                    _fov = value;
                    this.RaisePropertyChanged(() => this.Fov);
                }
            }
        }

        private Point3D _turretPosition;
        public Point3D TurretPosition
        {
            get => _turretPosition;
	        set
            {
                _turretPosition = value;
                this.RaisePropertyChanged(() => this.TurretPosition);
            }
        }

        private Point3D _gunPosition;
        public Point3D GunPosition
        {
            get => _gunPosition;
	        set
            {
                _gunPosition = value;
                this.RaisePropertyChanged(() => this.GunPosition);
            }
        }

        private Point3D _hullPosition;
        public Point3D HullPosition
        {
            get => _hullPosition;
	        set
            {
                _hullPosition = value;
                this.RaisePropertyChanged(() => this.HullPosition);
            }
        }

        private Model.ModelType _modelType;
        public Model.ModelType ModelType
        {
            get => _modelType;
	        set
            {
                _modelType = value;
                this.RaisePropertyChanged(() => this.ModelType);
                this.RaisePropertyChanged(() => this.IsCollisionModel);
            }
        }

        public bool IsCollisionModel => this.ModelType == Model.ModelType.Collision;

	    private string _nationName;
        public string NationName
        {
            get => _nationName;
	        set
            {
                _nationName = value;
                this.RaisePropertyChanged(() => this.NationName);
            }
        }

        public Database Database => this.Tank.Tank.Database;

	    private TankViewModel _tank;
        public TankViewModel Tank
        {
            get => _tank;
	        set
            {
                ModelName = value.Name;
                NationName = value.NationKey;
                _tank = value;

                TankRebind(value, HullProperty);
                TankRebind(value, TurretProperty);
                TankRebind(value, GunProperty);
                TankRebind(value, ChassisProperty);
                
                this.RaisePropertyChanged(() => this.Tank);
                this.RaisePropertyChanged(() => this.TankThickestArmor);
                this.RaisePropertyChanged(() => this.TankThinnestArmor);
                this.RaisePropertyChanged(() => this.TankThickestSpacingArmor);
                this.RaisePropertyChanged(() => this.TankThinnestSpacingArmor);
            }
        }



        public double Fps
        {
            get => (double)GetValue(FpsProperty);
	        set => SetValue(FpsProperty, value);
        }

        // Using a DependencyProperty as the backing store for FPS.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FpsProperty =
            DependencyProperty.Register("Fps", typeof(double), typeof(ModelViewModel), new PropertyMetadata(0.0));

        public int TriangleCount
        {
            get => (int)GetValue(TriangleCountProperty);
	        set => SetValue(TriangleCountProperty, value);
        }

        // Using a DependencyProperty as the backing store for TriangleCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TriangleCountProperty =
            DependencyProperty.Register("TriangleCount", typeof(int), typeof(ModelViewModel), new PropertyMetadata(0));


        private ShootTestResult _shootTestResult;
        public ShootTestResult ShootTestResult
        {
            get => _shootTestResult;
	        set
            {
                _shootTestResult = value;
                this.RaisePropertyChanged(() => this.ShootTestResult);
            }
        }

        private TestShellInfo _testShell;
        public TestShellInfo TestShell
        {
            get => _testShell;
	        set
            {
                _testShell = value;
                this.RaisePropertyChanged(() => this.TestShell);
            }
        }


        private bool _isMouseOverModel;
        public bool IsMouseOverModel
        {
            get => _isMouseOverModel;
	        set
            {
                _isMouseOverModel = value;
                this.RaisePropertyChanged(() => this.IsMouseOverModel);
            }
        }

        public double VehicleYaw
        {
            get => (double)GetValue(VehicleYawProperty);
	        set => SetValue(VehicleYawProperty, value);
        }

        public static readonly DependencyProperty VehicleYawProperty =
            DependencyProperty.Register("VehicleYaw", typeof(double), typeof(ModelViewModel), new PropertyMetadata(0.0));

        public double TurretYaw
        {
            get => (double)GetValue(TurretYawProperty);
	        set => SetValue(TurretYawProperty, value);
        }

        public static readonly DependencyProperty TurretYawProperty =
            DependencyProperty.Register("TurretYaw", typeof(double), typeof(ModelViewModel), new PropertyMetadata(0.0));

        public double GunPitch
        {
            get => (double)GetValue(GunPitchProperty);
	        set => SetValue(GunPitchProperty, value);
        }

        public static readonly DependencyProperty GunPitchProperty =
            DependencyProperty.Register("GunPitch", typeof(double), typeof(ModelViewModel), new PropertyMetadata(0.0));




        private void TankRebind(TankViewModelBase newTank, DependencyProperty modelProperty)
        {
            if (_tank != null)
            {
                BindingOperations.ClearBinding(this, modelProperty);
            }

            var binding = new Binding(modelProperty.Name);
            binding.Source = newTank.LoadedModules;
            binding.Converter = ValueConverter.Create(
                o => new Model(this, ((ModuleViewModel)o).Module, (ModuleViewModel)o)
                );
            BindingOperations.SetBinding(this, modelProperty, binding);
        }

        private bool _hasHoverModel;
        public bool HasHoverModel
        {
            get => _hasHoverModel;
	        set
            {
                _hasHoverModel = value;
                this.RaisePropertyChanged(() => this.HasHoverModel);
            }
        }

        private Model _hoverModel;
        public Model HoverModel
        {
            get => _hoverModel;
	        set
            {
                if (_hoverModel == value) return;

                _hoverModel = value;
                HasHoverModel = value != null;
                if (_hoverModel != null)
                {
                    ModuleViewModel = _hoverModel.ModuleViewModel;
                }

                this.RaisePropertyChanged(() => this.HoverModel);
            }
        }

        private ModuleViewModel _moduleViewModel;
        public ModuleViewModel ModuleViewModel
        {
            get => _moduleViewModel;
	        set
            {
                _moduleViewModel = value;
                this.RaisePropertyChanged(() => this.ModuleViewModel);
            }
        }

        public Model Turret
        {
            get => (Model)GetValue(TurretProperty);
	        set => SetValue(TurretProperty, value);
        }

        public static readonly DependencyProperty TurretProperty =
            DependencyProperty.Register("Turret", typeof(Model), typeof(ModelViewModel), new PropertyMetadata(null));

        public Model Gun
        {
            get => (Model)GetValue(GunProperty);
	        set => SetValue(GunProperty, value);
        }

        public static readonly DependencyProperty GunProperty =
            DependencyProperty.Register("Gun", typeof(Model), typeof(ModelViewModel), new PropertyMetadata(null));

        public Model Chassis
        {
            get => (Model)GetValue(ChassisProperty);
	        set => SetValue(ChassisProperty, value);
        }

        public static readonly DependencyProperty ChassisProperty =
            DependencyProperty.Register("Chassis", typeof(Model), typeof(ModelViewModel), new PropertyMetadata(null));

        public Model Hull
        {
            get => (Model)GetValue(HullProperty);
	        set => SetValue(HullProperty, value);
        }

        public static readonly DependencyProperty HullProperty =
            DependencyProperty.Register("Hull", typeof(Model), typeof(ModelViewModel), new PropertyMetadata(null));


        public static Model GetModel(DependencyObject obj)
        {
            return (Model)obj.GetValue(ModelProperty);
        }

        public static void SetModel(DependencyObject obj, Model value)
        {
            obj.SetValue(ModelProperty, value);
        }

        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.RegisterAttached("Model", typeof(Model), typeof(ModelViewModel), new PropertyMetadata(null));

        private string _modelName;
        public string ModelName
        {
            get => _modelName;
	        set
            {
                _modelName = value;
                this.RaisePropertyChanged(() => this.ModelName);
            }
        }

        private CommandBindingCollection _commandBindings;

        public Vector3D LookDirection
        {
            get => (Vector3D)GetValue(LookDirectionProperty);
	        set => SetValue(LookDirectionProperty, value);
        }

        // Using a DependencyProperty as the backing store for LookDirection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LookDirectionProperty =
            DependencyProperty.Register("LookDirection", typeof(Vector3D), typeof(ModelViewModel), new PropertyMetadata(new Vector3D(0,0,1)));

       
        public Transform3D TrackBallTransform
        {
            get => (Transform3D)GetValue(WorldTransformProperty);
	        set => SetValue(WorldTransformProperty, value);
        }

        // Using a DependencyProperty as the backing store for WorldTransform.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WorldTransformProperty =
            DependencyProperty.Register("TrackBallTransform", typeof(Transform3D), typeof(ModelViewModel), new PropertyMetadata(Transform3D.Identity));

        public Point MousePosition
        {
            get => (Point)GetValue(MousePositionProperty);
	        set => SetValue(MousePositionProperty, value);
        }

        // Using a DependencyProperty as the backing store for MousePosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MousePositionProperty =
            DependencyProperty.Register("MousePosition", typeof(Point), typeof(ModelViewModel), new PropertyMetadata(new Point(0.0,0.0)));



        public double Zoom
        {
            get => (double)GetValue(ZoomProperty);
	        set => SetValue(ZoomProperty, value);
        }

        // Using a DependencyProperty as the backing store for Zoom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register("Zoom", typeof(double), typeof(ModelViewModel), new PropertyMetadata(0.0));

        public double Distance
        {
            get => (double)GetValue(DistanceProperty);
	        set => SetValue(DistanceProperty, value);
        }

        public static readonly DependencyProperty DistanceProperty =
            DependencyProperty.Register("Distance", typeof(double), typeof(ModelViewModel), new PropertyMetadata(100.0));
        

        private RenderActivityLevel _renderActivityLevel;
        public RenderActivityLevel RenderActivityLevel
        {
            get => _renderActivityLevel;
	        set
            {
                _renderActivityLevel = value;
                this.RaisePropertyChanged(() => this.RenderActivityLevel);
            }
        }

        public ModelViewModel(CommandBindingCollection commandBindings)
        {
            _commandBindings = commandBindings;
            this.Fov = 75;
        }
    }
}
