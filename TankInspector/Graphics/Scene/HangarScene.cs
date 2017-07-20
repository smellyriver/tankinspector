using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D9;
using Smellyriver.TankInspector.Graphics.Frameworks;
using System.ComponentModel;
using System.Windows;
using Smellyriver.TankInspector.Graphics.Smaa;
using Smellyriver.TankInspector.Modeling;
using System.Threading;
using Smellyriver.TankInspector.Design;
using log4net;
using System.Reflection;
using System.Drawing;


namespace Smellyriver.TankInspector.Graphics.Scene
{
	internal partial class HangarScene : SceneBase<D3D9>
    {
        private static readonly ILog Log = SafeLog.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

	    public static HangarScene Current { get; private set; }

	    private readonly ModelTextureManager _textureManager;

        public HangarScene()
        {
            HangarScene.Current = this;
            _textureManager = new ModelTextureManager();
            _armorColorMapDirty = true;
            _fps = new Fps();

            DependencyPropertyDescriptor textDescr = DependencyPropertyDescriptor.
                FromProperty(TurretProperty, typeof(HangarScene));

            if (textDescr != null)
            {
                textDescr.AddValueChanged(this, delegate
                {
                    UpdateTurret();
                });
            }
            textDescr = DependencyPropertyDescriptor.FromProperty(GunProperty, typeof(HangarScene));

            if (textDescr != null)
            {
                textDescr.AddValueChanged(this, delegate
                {
                    UpdateGun();
                });
            }
            textDescr = DependencyPropertyDescriptor.FromProperty(ChassisProperty, typeof(HangarScene));

            if (textDescr != null)
            {
                textDescr.AddValueChanged(this, delegate
                {
                    UpdateChassis();
                });
            }
            textDescr = DependencyPropertyDescriptor.FromProperty(HullProperty, typeof(HangarScene));
            if (textDescr != null)
            {
                textDescr.AddValueChanged(this, delegate
                {
                    UpdateHull();
                });
            }
        }

	    private CancellationTokenSource _updateHullCancellationTokenSource = null;
        private void UpdateHull()
        {
            var model = Hull;
            if (model != null)
            {
                if (_updateHullCancellationTokenSource != null)
                {
                    _updateHullCancellationTokenSource.Cancel();

                }
                _updateHullCancellationTokenSource = new CancellationTokenSource();
                model.LoadingTask.ContinueWith((_) =>
                    {
                        var mesh = _hullMesh;          
                        _hullMesh = TankMesh.FromModel(model, Renderer.Device, _textureManager);

                        ClearTracerFrom(mesh);
                        Disposer.RemoveAndDispose(ref mesh);
                    },_updateHullCancellationTokenSource.Token);
            }
        }

	    private CancellationTokenSource _updateChassisCancellationTokenSource = null;
        private void UpdateChassis()
        {
            var model = Chassis;
            if (model != null)
            {
                if (_updateChassisCancellationTokenSource != null)
                {
                    _updateChassisCancellationTokenSource.Cancel();
                }
                _updateChassisCancellationTokenSource = new CancellationTokenSource();
                model.LoadingTask.ContinueWith((_) =>
                {
                    var mesh = _chassisMesh;
                    _chassisMesh = TankMesh.FromModel(model, Renderer.Device, _textureManager);

                    ClearTracerFrom(mesh);
                    Disposer.RemoveAndDispose(ref mesh);
                }, _updateChassisCancellationTokenSource.Token);
            }
        }

	    private CancellationTokenSource _updateGunCancellationTokenSource = null;
        private void UpdateGun()
        {
            var model = Gun;
            if (model != null)
            {
                if (_updateGunCancellationTokenSource != null)
                {
                    _updateGunCancellationTokenSource.Cancel();

                }
                _updateGunCancellationTokenSource = new CancellationTokenSource();
                model.LoadingTask.ContinueWith((_) =>
                {
                    var mesh = _gunMesh; 
                   _gunMesh = TankMesh.FromModel(model, Renderer.Device, _textureManager);

                    ClearTracerFrom(mesh);
                   Disposer.RemoveAndDispose(ref mesh);
                }, _updateGunCancellationTokenSource.Token);
            }
        }

	    private CancellationTokenSource _updateTurretCancellationTokenSource = null;
        private void UpdateTurret()
        {
            var model = Turret;
            if (model != null)
            {
                if (_updateTurretCancellationTokenSource != null)
                {
                    _updateTurretCancellationTokenSource.Cancel();                 
                }
                _updateTurretCancellationTokenSource = new CancellationTokenSource();
                
                model.LoadingTask.ContinueWith((_) =>
                {
                    var mesh = _turretMesh;            
                    _turretMesh = TankMesh.FromModel(model, Renderer.Device, _textureManager);

                        ClearTracerFrom(mesh);
                    Disposer.RemoveAndDispose(ref mesh);
                }, _updateTurretCancellationTokenSource.Token);
            }
        }

        private void ClearTracerFrom(TankMesh mesh)
        {
            Monitor.Enter(_projectileTracer);
            try
            {
                _projectileTracer.RemoveAll(t => t.Hit(mesh));
            }
            finally
            {
                Monitor.Exit(_projectileTracer);
            }
        }

        public Model Turret
        {
            get => (Model)GetValue(TurretProperty);
	        set => SetValue(TurretProperty, value);
        }

        public static readonly DependencyProperty TurretProperty =
            DependencyProperty.Register("Turret", typeof(Model), typeof(HangarScene), new PropertyMetadata(null));

        public Model Gun
        {
            get => (Model)GetValue(GunProperty);
	        set => SetValue(GunProperty, value);
        }

        public static readonly DependencyProperty GunProperty =
            DependencyProperty.Register("Gun", typeof(Model), typeof(HangarScene), new PropertyMetadata(null));

        public Model Chassis
        {
            get => (Model)GetValue(ChassisProperty);
	        set => SetValue(ChassisProperty, value);
        }

        public static readonly DependencyProperty ChassisProperty =
            DependencyProperty.Register("Chassis", typeof(Model), typeof(HangarScene), new PropertyMetadata(null));

        public Model Hull
        {
            get => (Model)GetValue(HullProperty);
	        set => SetValue(HullProperty, value);
        }

        public static readonly DependencyProperty HullProperty =
            DependencyProperty.Register("Hull", typeof(Model), typeof(HangarScene), new PropertyMetadata(null));

        public Matrix TrackBallTransform
        {
            get => (Matrix)GetValue(TrackBallTransformProperty);
	        set => SetValue(TrackBallTransformProperty, value);
        }

        // Using a DependencyProperty as the backing store for WorldTransform.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrackBallTransformProperty =
            DependencyProperty.Register("TrackBallTransform", typeof(Matrix), typeof(HangarScene), new PropertyMetadata(Matrix.Identity));

        public Vector3 HullPosition
        {
            get => (Vector3)GetValue(HullPositionProperty);
	        set => SetValue(HullPositionProperty, value);
        }

        public static readonly DependencyProperty HullPositionProperty =
            DependencyProperty.Register("HullPosition", typeof(Vector3), typeof(HangarScene), new PropertyMetadata(Vector3.Zero));

        public Vector3 TurretPosition
        {
            get => (Vector3)GetValue(TurretPositionProperty);
	        set => SetValue(TurretPositionProperty, value);
        }

        public static readonly DependencyProperty TurretPositionProperty =
            DependencyProperty.Register("TurretPosition", typeof(Vector3), typeof(HangarScene), new PropertyMetadata(Vector3.Zero));

        public Vector3 GunPosition
        {
            get => (Vector3)GetValue(GunPositionProperty);
	        set => SetValue(GunPositionProperty, value);
        }

        public static readonly DependencyProperty GunPositionProperty =
            DependencyProperty.Register("GunPosition", typeof(Vector3), typeof(HangarScene), new PropertyMetadata(Vector3.Zero));


        public System.Windows.Media.GradientStopCollection RegularArmorSpectrumStops
        {
            get => (System.Windows.Media.GradientStopCollection)GetValue(RegularArmorSpectrumStopsProperty);
	        set => SetValue(RegularArmorSpectrumStopsProperty, value);
        }

        public static readonly DependencyProperty RegularArmorSpectrumStopsProperty =
            DependencyProperty.Register("RegularArmorSpectrumStops", typeof(System.Windows.Media.GradientStopCollection), typeof(HangarScene), new PropertyMetadata(null, (o, e) => { ((HangarScene)o).InvalidateArmorColorMap(); }));

        public System.Windows.Media.GradientStopCollection SpacingArmorSpectrumStops
        {
            get => (System.Windows.Media.GradientStopCollection)GetValue(SpacingArmorSpectrumStopsProperty);
	        set => SetValue(SpacingArmorSpectrumStopsProperty, value);
        }

        public static readonly DependencyProperty SpacingArmorSpectrumStopsProperty =
            DependencyProperty.Register("SpacingArmorSpectrumStops", typeof(System.Windows.Media.GradientStopCollection), typeof(HangarScene), new PropertyMetadata(null, (o, e) => { ((HangarScene)o).InvalidateArmorColorMap(); }));


        public double RegularArmorValueSelectionBegin
        {
            get => (double)GetValue(RegularArmorValueSelectionBeginProperty);
	        set => SetValue(RegularArmorValueSelectionBeginProperty, value);
        }

        public static readonly DependencyProperty RegularArmorValueSelectionBeginProperty =
            DependencyProperty.Register("RegularArmorValueSelectionBegin", typeof(double), typeof(HangarScene), new PropertyMetadata(0.0));


        public double RegularArmorValueSelectionEnd
        {
            get => (double)GetValue(RegularArmorValueSelectionEndProperty);
	        set => SetValue(RegularArmorValueSelectionEndProperty, value);
        }

        public static readonly DependencyProperty RegularArmorValueSelectionEndProperty =
            DependencyProperty.Register("RegularArmorValueSelectionEnd", typeof(double), typeof(HangarScene), new PropertyMetadata(0.0));

        public double SpacingArmorValueSelectionBegin
        {
            get => (double)GetValue(SpacingArmorValueSelectionBeginProperty);
	        set => SetValue(SpacingArmorValueSelectionBeginProperty, value);
        }

        public static readonly DependencyProperty SpacingArmorValueSelectionBeginProperty =
            DependencyProperty.Register("SpacingArmorValueSelectionBegin", typeof(double), typeof(HangarScene), new PropertyMetadata(0.0));

        public double SpacingArmorValueSelectionEnd
        {
            get => (double)GetValue(SpacingArmorValueSelectionEndProperty);
	        set => SetValue(SpacingArmorValueSelectionEndProperty, value);
        }

        public static readonly DependencyProperty SpacingArmorValueSelectionEndProperty =
            DependencyProperty.Register("SpacingArmorValueSelectionEnd", typeof(double), typeof(HangarScene), new PropertyMetadata(0.0));

        public double TankThickestArmor
        {
            get => (double)GetValue(TankThickestArmorProperty);
	        set => SetValue(TankThickestArmorProperty, value);
        }

        public static readonly DependencyProperty TankThickestArmorProperty =
            DependencyProperty.Register("TankThickestArmor", typeof(double), typeof(HangarScene), new PropertyMetadata(0.0));

        public double TankThinnestArmor
        {
            get => (double)GetValue(TankThinnestArmorProperty);
	        set => SetValue(TankThinnestArmorProperty, value);
        }

        public static readonly DependencyProperty TankThinnestArmorProperty =
            DependencyProperty.Register("TankThinnestArmor", typeof(double), typeof(HangarScene), new PropertyMetadata(0.0));



        public double TankThickestSpacingArmor
        {
            get => (double)GetValue(TankThickestSpacingArmorProperty);
	        set => SetValue(TankThickestSpacingArmorProperty, value);
        }

        public static readonly DependencyProperty TankThickestSpacingArmorProperty =
            DependencyProperty.Register("TankThickestSpacingArmor", typeof(double), typeof(HangarScene), new PropertyMetadata(0.0));

        public double TankThinnestSpacingArmor
        {
            get => (double)GetValue(TankThinnestSpacingArmorProperty);
	        set => SetValue(TankThinnestSpacingArmorProperty, value);
        }

        public static readonly DependencyProperty TankThinnestSpacingArmorProperty =
            DependencyProperty.Register("TankThinnestSpacingArmor", typeof(double), typeof(HangarScene), new PropertyMetadata(0.0));

        public Model.ModelType ModelType
        {
            get => (Model.ModelType)GetValue(ModelTypeProperty);
	        set => SetValue(ModelTypeProperty, value);
        }

        public static readonly DependencyProperty ModelTypeProperty =
            DependencyProperty.Register("ModelType", typeof(Model.ModelType), typeof(HangarScene), new PropertyMetadata(Model.ModelType.Undamaged));

        public bool HasRegularArmorHintValue
        {
            get => (bool)GetValue(HasRegularArmorHintValueProperty);
	        set => SetValue(HasRegularArmorHintValueProperty, value);
        }

        public static readonly DependencyProperty HasRegularArmorHintValueProperty =
            DependencyProperty.Register("HasRegularArmorHintValue", typeof(bool), typeof(HangarScene), new PropertyMetadata(false));

        public bool HasSpacingArmorHintValue
        {
            get => (bool)GetValue(HasSpacingArmorHintValueProperty);
	        set => SetValue(HasSpacingArmorHintValueProperty, value);
        }


        public static readonly DependencyProperty HasSpacingArmorHintValueProperty =
            DependencyProperty.Register("HasSpacingArmorHintValue", typeof(bool), typeof(HangarScene), new PropertyMetadata(false));

        public double RegularArmorHintValue
        {
            get => (double)GetValue(RegularArmorHintValueProperty);
	        set => SetValue(RegularArmorHintValueProperty, value);
        }

        public static readonly DependencyProperty RegularArmorHintValueProperty =
            DependencyProperty.Register("RegularArmorHintValue", typeof(double), typeof(HangarScene), new PropertyMetadata(0.0));

        public double SpacingArmorHintValue
        {
            get => (double)GetValue(SpacingArmorHintValueProperty);
	        set => SetValue(SpacingArmorHintValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for SpacingArmorHintValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpacingArmorHintValueProperty =
            DependencyProperty.Register("SpacingArmorHintValue", typeof(double), typeof(HangarScene), new PropertyMetadata(0.0));




        public bool IsHitTestEnabled
        {
            get => (bool)GetValue(IsHitTestEnabledProperty);
	        set => SetValue(IsHitTestEnabledProperty, value);
        }

        public static readonly DependencyProperty IsHitTestEnabledProperty =
            DependencyProperty.Register("IsHitTestEnabled", typeof(bool), typeof(HangarScene), new PropertyMetadata(true));

        public System.Windows.Point MousePoition
        {
            get => (System.Windows.Point)GetValue(MousePoitionProperty);
	        set => SetValue(MousePoitionProperty, value);
        }

        // Using a DependencyProperty as the backing store for MousePoition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MousePoitionProperty =
            DependencyProperty.Register("MousePoition", typeof(System.Windows.Point), typeof(HangarScene), new PropertyMetadata(new System.Windows.Point(),
                (d, e) => ((HangarScene)d).OnMousePoitionChanged((System.Windows.Point)e.OldValue, (System.Windows.Point)e.NewValue)));

        private void OnMousePoitionChanged(System.Windows.Point oldPoint,System.Windows.Point newPoint)
        {
            if (this.IsHitTestEnabled)
            {
                var x = (float)MousePoition.X;
                var y = (float)MousePoition.Y;

                var ray = GetPickRay(x, y);
                var invWorldTrans = Matrix.Invert(TrackBallTransform);
                ray.Position = invWorldTrans.TransformCoord(ray.Position);
                ray.Direction = invWorldTrans.TransformNormal(ray.Direction);
                ray.Direction.Normalize();

                if (!TryPickTracer(ray))
                    TryHitTestCollisionModel(ray);
            }
        }


        public int TriangleCount
        {
            get => (int)GetValue(TriangleCountProperty);
	        set => SetValue(TriangleCountProperty, value);
        }

        // Using a DependencyProperty as the backing store for TriangleCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TriangleCountProperty =
            DependencyProperty.Register("TriangleCount", typeof(int), typeof(HangarScene), new PropertyMetadata(0));

        public ShootTestResult ShootTestResult
        {
            get => (ShootTestResult)GetValue(ShootTestResultProperty);
	        set => SetValue(ShootTestResultProperty, value);
        }

        public static readonly DependencyProperty ShootTestResultProperty =
            DependencyProperty.Register("ShootTestResult", typeof(ShootTestResult), typeof(HangarScene), new PropertyMetadata(null));



        public TestShellInfo TestShell
        {
            get => (TestShellInfo)GetValue(TestShellProperty);
	        set => SetValue(TestShellProperty, value);
        }

        // Using a DependencyProperty as the backing store for TestShell.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TestShellProperty =
            DependencyProperty.Register("TestShell", typeof(TestShellInfo), typeof(HangarScene), new PropertyMetadata(new TestShellInfo(ShellType.AP, 0.0),
                (d, e) => ((HangarScene)d).OnTestShellChanged((TestShellInfo)e.OldValue, (TestShellInfo)e.NewValue)));

        private void OnTestShellChanged(TestShellInfo oldValue, TestShellInfo newValue)
        {

            Monitor.Enter(_projectileTracer);
            try
            {
                foreach (var tracer in _projectileTracer)
                {
                    tracer.Refresh(newValue);
                }
            }
            finally
            {
                Monitor.Exit(_projectileTracer);
            }

        }

        public bool IsMouseOverModel
        {
            get => (bool)GetValue(IsMouseOverModelProperty);
	        set => SetValue(IsMouseOverModelProperty, value);
        }

        public static readonly DependencyProperty IsMouseOverModelProperty =
            DependencyProperty.Register("IsMouseOverModel", typeof(bool), typeof(HangarScene), new PropertyMetadata(false));

        public double Fps
        {
            get => (double)GetValue(FpsProperty);
	        set => SetValue(FpsProperty, value);
        }

        public static readonly DependencyProperty FpsProperty =
            DependencyProperty.Register("Fps", typeof(double), typeof(HangarScene), new PropertyMetadata(0.0));



        public double TurretYaw
        {
            get => (double)GetValue(TurretYawProperty);
	        set => SetValue(TurretYawProperty, value);
        }

        // Using a DependencyProperty as the backing store for TurretYaw.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TurretYawProperty =
            DependencyProperty.Register("TurretYaw", typeof(double), typeof(HangarScene), new PropertyMetadata(0.0));



        public double GunPitch
        {
            get => (double)GetValue(GunPitchProperty);
	        set => SetValue(GunPitchProperty, value);
        }

        // Using a DependencyProperty as the backing store for GunPitch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GunPitchProperty =
            DependencyProperty.Register("GunPitch", typeof(double), typeof(HangarScene), new PropertyMetadata(0.0));

       


        public CameraMode CameraMode
        {
            get => (CameraMode)GetValue(CameraModeProperty);
	        set => SetValue(CameraModeProperty, value);
        }

        // Using a DependencyProperty as the backing store for CameraMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CameraModeProperty =
            DependencyProperty.Register("CameraMode", typeof(CameraMode), typeof(HangarScene), new PropertyMetadata(CameraMode.TrackBall,
                (d, e) => ((HangarScene)d).OnCameraModeChanged((CameraMode)e.OldValue, (CameraMode)e.NewValue)));



        public double ShotDistance
        {
            get => (double)GetValue(ShotDistanceProperty);
	        set => SetValue(ShotDistanceProperty, value);
        }

        // Using a DependencyProperty as the backing store for ShotDistance.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShotDistanceProperty =
            DependencyProperty.Register("ShotDistance", typeof(double), typeof(HangarScene), new PropertyMetadata(100.0,
                (d, e) => ((HangarScene)d).OnShotDistanceChanged((double)e.OldValue, (double)e.NewValue)));

        private void OnShotDistanceChanged(double oldValue, double newValue)
        {
            _shotPosition.Z = (float)-newValue;
            UpdateView();
        }

        public double Zoom
        {
            get => (double)GetValue(ZoomProperty);
	        set => SetValue(ZoomProperty, value);
        }

        // Using a DependencyProperty as the backing store for ZoomTransform.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register("Zoom", typeof(double), typeof(HangarScene), new PropertyMetadata(0.0,
                (d, e) => ((HangarScene)d).OnZoomTransformChanged((double)e.OldValue, (double)e.NewValue)));

        private void OnZoomTransformChanged(double oldZoom, double newZoom)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            _view = Matrix.LookAtLH(this.CameraPosition, _targetPosition, Vector3.UnitY);
            _invView = Matrix.Invert(_view);
            _viewProj = Matrix.Multiply(_view, _proj);
        }

        public Vector3 LookDirection
        {
            get => (Vector3)GetValue(LookDirectionProperty);
	        set => SetValue(LookDirectionProperty, value);
        }

        // Using a DependencyProperty as the backing store for LookDirection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LookDirectionProperty =
            DependencyProperty.Register("LookDirection", typeof(Vector3), typeof(HangarScene), new PropertyMetadata(Vector3.ForwardLH,
                (d, e) => ((HangarScene)d).OnLookDirectionChanged((Vector3)e.OldValue, (Vector3)e.NewValue)));

        private void OnLookDirectionChanged(Vector3 oldDirection, Vector3 newDirection)
        {
            var shot = Matrix.LookAtLH(_shotPosition, _targetPosition, Vector3.UnitY);
            var invShot = Matrix.Invert(shot);
            var newTarget  = shot.TransformCoord(_targetPosition);
            newTarget.X += newDirection.X;
            newTarget.Y -= newDirection.Y;
            newTarget = invShot.TransformCoord(newTarget);

            var targetDir = newTarget - _shotPosition;
            targetDir.Normalize();
            newTarget = _shotPosition + targetDir * (_targetPosition - _shotPosition).Length();

            //to do new target in tank~
            _targetPosition = newTarget;
            UpdateView();
        }

        private void OnCameraModeChanged(CameraMode oldMode,CameraMode newMode)
        {
            const float animationSpeed = 25.0f;
            if(newMode == CameraMode.TrackBall)
            {
                if (_targetPosition != Vector3.Zero)
                {
                    _animationDirection = Vector3.Normalize(_targetPosition) * animationSpeed;
                    _remainingTime = _targetPosition.Length() / animationSpeed;
                }
            }       
        }

	    private const double MaxHighlightDelta = 0.35;
	    private const double MaxLowlightDelta = 0.25;

        private double _highLightValue;


        private Matrix _proj;
        private Matrix _view;
        private Matrix _invView;
        private Matrix _viewProj;
        private Matrix _turretTrans;
        private Matrix _gunTrans;
        private Matrix _hullTrans;
        private Matrix _chassisTrans;

        private TankMesh _hullMesh;
        private TankMesh _turretMesh;
        private TankMesh _gunMesh;
        private TankMesh _chassisMesh;

        private Surface _positionSurface;
        private Surface _normalSurface;
        private Surface _colorSurface;
        private Surface _finalColorSurface;
        private Surface _depthStencil;

        private Texture _normalMap;
        private Texture _positionMap;
        private Texture _colorMap;
        private Texture _finalColorMap;
        private Texture _randomMap;
        private Texture _armorColorMap;
        private bool _armorColorMapDirty;
       
        private QuadRender _quadRender;

        private Effect _clearGBufferEffect;
        private Effect _renderGBufferEffect;
        private Effect _showGBufferEffect;
        private Effect _lightingTankEffect;
        private Effect _buildArmorColorEffect;

        private SMAA _smaa;
        private int _width;
        private int _height;
        private readonly Fps _fps;

        private CollisionModelHitTestResult _lastHitTestResult;

        private readonly List<ProjectileTracer> _projectileTracer = new List<ProjectileTracer>();

        private Vector3 _targetPosition = Vector3.Zero;
        private Vector3 _animationDirection = Vector3.Zero;
        private double _remainingTime = 0;

        private Vector3 _shotPosition = new Vector3(0, 0, -100);
        

        private Vector3 CameraPosition 
        {
            get 
            {
                var dir = _shotPosition-_targetPosition;
                dir.Normalize();
                return _targetPosition + dir*14.0f / (float)Zoom;
            }  
        }


        private void UpdateTargetPositionAnimation(double deltaTime)
        {
            if (_remainingTime > 0)
            {
                _targetPosition -= _animationDirection * (float)deltaTime;
                _remainingTime -= deltaTime;

                if (_remainingTime < 0)
                {
                    _targetPosition = Vector3.Zero;
                }

                UpdateView();
            }
        }

        private void SetViewProj(int width,int height)
        {
            _view = Matrix.LookAtLH(this.CameraPosition, _targetPosition, Vector3.UnitY);
            _invView = Matrix.Invert(_view);
            _proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f , (float)width / (float)height, 0.1f, 707.0f);
            _viewProj = Matrix.Multiply(_view, _proj);
        }

	    private void OnRenderResetted(object sender, DrawEventArgs e)
        {
            int w = (int)Math.Ceiling(e.RenderSize.Width);
            int h = (int)Math.Ceiling(e.RenderSize.Height);
            ResetScene(w, h);
        }

        private void ResetScene(int width, int height)
        {
            Log.InfoFormat("hangar scene reset to ({0},{1}) ", width, height);

            var device = Renderer.Device;
            _width = width;
            _height = height;
            SetViewProj(width, height);

            Disposer.RemoveAndDispose(ref _depthStencil);       
            Disposer.RemoveAndDispose(ref _quadRender);
            Disposer.RemoveAndDispose(ref _finalColorSurface);
            Disposer.RemoveAndDispose(ref _colorSurface);
            Disposer.RemoveAndDispose(ref _positionSurface);
            Disposer.RemoveAndDispose(ref _normalSurface);
            Disposer.RemoveAndDispose(ref _finalColorMap);
            Disposer.RemoveAndDispose(ref _colorMap);
            Disposer.RemoveAndDispose(ref _positionMap);
            Disposer.RemoveAndDispose(ref _normalMap);

            _normalMap = new Texture(device, width, height, 1, Usage.RenderTarget, Format.A16B16G16R16F, Pool.Default);
            _positionMap = new Texture(device, width, height, 1, Usage.RenderTarget, Format.A16B16G16R16F, Pool.Default);
            _colorMap = new Texture(device, width, height, 1, Usage.RenderTarget, Format.A16B16G16R16F, Pool.Default);
            _finalColorMap = new Texture(device, width, height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            _normalSurface = _normalMap.GetSurfaceLevel(0);
            _positionSurface = _positionMap.GetSurfaceLevel(0);
            _colorSurface = _colorMap.GetSurfaceLevel(0);
            _finalColorSurface = _finalColorMap.GetSurfaceLevel(0);
            _quadRender = new QuadRender(device, width, height);

            _smaa.Reset(width, height, _quadRender);

            this.ResetHud(width, height);

            _depthStencil = Surface.CreateDepthStencil(device, width, height, Format.D24S8, MultisampleType.None, 0, true);

            device.DepthStencilSurface = _depthStencil;
        }

        public void HdShot(int width, int height, string filename)
        {
            Log.InfoFormat("hangar scene shot to picture ({0},{1}) ", width, height);

            var device = Renderer.Device;

            var proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)width / (float)height, 0.1f, 707.0f);
            var viewProj = Matrix.Multiply(_view, proj);

            var normalMap = new Texture(device, width, height, 1, Usage.RenderTarget, Format.A16B16G16R16F, Pool.Default);
            var positionMap = new Texture(device, width, height, 1, Usage.RenderTarget, Format.A16B16G16R16F, Pool.Default);
            var colorMap = new Texture(device, width, height, 1, Usage.RenderTarget, Format.A16B16G16R16F, Pool.Default);
            var finalColorMap = new Texture(device, width, height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            var dstMap = new Texture(device, width, height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            var normalSurface = normalMap.GetSurfaceLevel(0);
            var positionSurface = positionMap.GetSurfaceLevel(0);
            var colorSurface = colorMap.GetSurfaceLevel(0);
            var finalColorSurface = finalColorMap.GetSurfaceLevel(0);
            var dstSurface = dstMap.GetSurfaceLevel(0);
            var quadRender = new QuadRender(device, width, height);    

            var depthStencil = Surface.CreateDepthStencil(device,width,height,Format.D24S8,MultisampleType.None,0,true);
            device.DepthStencilSurface = depthStencil;

            var smaa = new SMAA(device, 1, 1, SMAA.Preset.PresetUltra);

            smaa.Reset(width, height, quadRender);

            device.BeginScene();

            var backupSurface = device.GetRenderTarget(0);

            device.SetRenderTarget(0, colorSurface);
            device.SetRenderTarget(1, normalSurface);
            device.SetRenderTarget(2, positionSurface);

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer | ClearFlags.Stencil, new SharpDX.Color(0, 0, 0, 0), 1.0f, 0);

            _clearGBufferEffect.Technique = _clearGBufferEffect.GetTechnique(0);
            _clearGBufferEffect.Begin();
            _clearGBufferEffect.BeginPass(0);

            quadRender.Render(Renderer.Device);

            _clearGBufferEffect.EndPass();
            _clearGBufferEffect.End();

            if (ModelType == Model.ModelType.Collision)
            {
                device.SetRenderState(RenderState.CullMode, Cull.None);
            }

            int triangleCount = 0;

            _renderGBufferEffect.Technique = _renderGBufferEffect.GetTechnique(0);
            _renderGBufferEffect.SetValue("viewProj", viewProj);
            _renderGBufferEffect.SetValue("world", TrackBallTransform);
            _renderGBufferEffect.SetValue("maxAnisotropy", 16);

            if (ModelType == Model.ModelType.Collision)
            {
                _renderGBufferEffect.SetValue("useArmor", true);
            }
            else
            {
                _renderGBufferEffect.SetValue("useArmor", false);
            }

            _turretTrans = Matrix.Identity * Matrix.RotationY((float)DxUtils.ConvertDegreesToRadians(-TurretYaw));
            _renderGBufferEffect.SetValue("world", _turretTrans * TrackBallTransform);

            if (_turretMesh != null)
                _turretMesh.Render(_renderGBufferEffect, ref triangleCount, ModelType);

            _gunTrans = Matrix.RotationX((float)DxUtils.ConvertDegreesToRadians(-GunPitch)) * Matrix.Translation(GunPosition) * _turretTrans;
            _renderGBufferEffect.SetValue("world", _gunTrans * TrackBallTransform);

            if (_gunMesh != null)
                _gunMesh.Render(_renderGBufferEffect, ref triangleCount, ModelType);

            _hullTrans = Matrix.Translation(-TurretPosition);
            _renderGBufferEffect.SetValue("world", _hullTrans * TrackBallTransform);

            if (_hullMesh != null)
                _hullMesh.Render(_renderGBufferEffect, ref triangleCount, ModelType);

            _chassisTrans = _hullTrans * Matrix.Translation(-HullPosition);
            _renderGBufferEffect.SetValue("world", _chassisTrans * TrackBallTransform);

            if (_chassisMesh != null)
                _chassisMesh.Render(_renderGBufferEffect, ref triangleCount, ModelType);

            if (ModelType == Model.ModelType.Collision)
            {
                device.SetRenderState(RenderState.CullMode, Cull.Counterclockwise);
            }

            device.SetRenderTarget(0, finalColorSurface);      
            device.SetRenderTarget(1, null);
            device.SetRenderTarget(2, null);

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer , new SharpDX.Color(0, 0, 0, 0), 1.0f, 0);

            device.SetTexture(0, colorMap);
            device.SetTexture(1, normalMap);
            device.SetTexture(2, positionMap);
            device.SetTexture(3, _randomMap);

            if (ModelType == Model.ModelType.Collision)
            {
                device.SetTexture(4, _armorColorMap);

                _lightingTankEffect.SetValue("tankThickestArmor", (float)TankThickestArmor + 0.125f);
                _lightingTankEffect.SetValue("tankThinnestArmor", (float)TankThinnestArmor - 0.125f);
                _lightingTankEffect.SetValue("tankThickestSpacingArmor", (float)TankThickestSpacingArmor + 0.125f);
                _lightingTankEffect.SetValue("tankThinnestSpacingArmor", (float)TankThinnestSpacingArmor - 0.125f);

                var regularArmorValueSelectionMax = (float)Math.Max(RegularArmorValueSelectionBegin, RegularArmorValueSelectionEnd);
                var regularArmorValueSelectionMin = (float)Math.Min(RegularArmorValueSelectionBegin, RegularArmorValueSelectionEnd);

                _lightingTankEffect.SetValue("regularArmorValueSelectionMax", regularArmorValueSelectionMax + 0.4f);
                _lightingTankEffect.SetValue("regularArmorValueSelectionMin", regularArmorValueSelectionMin - 0.4f);

                var spacingArmorValueSelectionMax = (float)Math.Max(SpacingArmorValueSelectionBegin, SpacingArmorValueSelectionEnd);
                var spacingArmorValueSelectionMin = (float)Math.Min(SpacingArmorValueSelectionBegin, SpacingArmorValueSelectionEnd);

                _lightingTankEffect.SetValue("spacingArmorValueSelectionMax", spacingArmorValueSelectionMax + 0.4f);
                _lightingTankEffect.SetValue("spacingArmorValueSelectionMin", spacingArmorValueSelectionMin - 0.4f);

                _lightingTankEffect.SetValue("hasRegularArmorHintValue", false);
                _lightingTankEffect.SetValue("hasSpacingArmorHintValue", false);
                _lightingTankEffect.SetValue("useBlackEdge", ApplicationSettings.Default.CollisionModelStrokeEnabled);

                _lightingTankEffect.Technique = _lightingTankEffect.GetTechnique(1);
            }
            else
            {
                _lightingTankEffect.Technique = _lightingTankEffect.GetTechnique(0);
            }

            _lightingTankEffect.SetValue("useSSAO", ApplicationSettings.Default.SSAOEnabled);

            _lightingTankEffect.Begin();
            _lightingTankEffect.BeginPass(0);

            quadRender.Render(device);

            _lightingTankEffect.EndPass();
            _lightingTankEffect.End();

            device.SetRenderTarget(0, dstSurface);
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer | ClearFlags.Stencil, new SharpDX.Color(0, 0, 0, 0), 1.0f, 0);
            smaa.Apply(finalColorMap, finalColorMap, dstSurface);

            device.SetRenderTarget(0,backupSurface);

            device.EndScene();
            device.Present();

            var shootMap = new Texture(device, width, height, 1, Usage.None, Format.A8R8G8B8, Pool.SystemMemory);
            var shootMapSurface = shootMap.GetSurfaceLevel(0);

            device.GetRenderTargetData(dstSurface, shootMapSurface);


			shootMap.LockRectangle(0, LockFlags.ReadOnly, out DataStream data);

			using (Bitmap output = new Bitmap(width,height))
            {
                for (int h = 0; h != height; ++h)
                {
                    for (int w = 0; w != width; ++w)
                    {
						output.SetPixel(w, h, System.Drawing.Color.FromArgb(data.Read<int>()));
                    }
                }
                output.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
            }

            shootMap.UnlockRectangle(0);

            device.DepthStencilSurface = _depthStencil;

            Disposer.RemoveAndDispose(ref shootMapSurface);
            Disposer.RemoveAndDispose(ref shootMap);
            Disposer.RemoveAndDispose(ref depthStencil);
            Disposer.RemoveAndDispose(ref smaa);      
            Disposer.RemoveAndDispose(ref quadRender);
            Disposer.RemoveAndDispose(ref dstSurface);
            Disposer.RemoveAndDispose(ref finalColorSurface);
            Disposer.RemoveAndDispose(ref colorSurface);
            Disposer.RemoveAndDispose(ref positionSurface);
            Disposer.RemoveAndDispose(ref normalSurface);
            Disposer.RemoveAndDispose(ref dstMap);
            Disposer.RemoveAndDispose(ref finalColorMap);
            Disposer.RemoveAndDispose(ref colorMap);
            Disposer.RemoveAndDispose(ref positionMap);
            Disposer.RemoveAndDispose(ref normalMap);
        }

        private void RenderGBuffer()
        {
            SetGBuffer();
            ClearGBuffer();
            var device = Renderer.Device;

            if (ModelType == Model.ModelType.Collision)
            {
                device.SetRenderState(RenderState.CullMode, Cull.None);
            }

            if (ApplicationSettings.Default.WireframeMode)
            {
                
                device.SetRenderState(RenderState.FillMode, FillMode.Wireframe);
                RenderScene();
                device.SetRenderState(RenderState.FillMode, FillMode.Solid);
            }
            else
            {
                RenderScene();
            }

            if (ModelType == Model.ModelType.Collision)
            {
                device.SetRenderState(RenderState.CullMode, Cull.Counterclockwise);
            }

            ResolveGBuffer();
        }

        private void RenderScene()
        {
            var device = Renderer.Device;


            int triangleCount = 0;

            _renderGBufferEffect.Technique = _renderGBufferEffect.GetTechnique(0);
            _renderGBufferEffect.SetValue("viewProj", _viewProj);


            _renderGBufferEffect.SetValue("world", TrackBallTransform);

            if (ModelType == Model.ModelType.Collision)
            {
                _renderGBufferEffect.SetValue("useArmor", true);
            }
            else
            {
                _renderGBufferEffect.SetValue("useArmor", false);
            }

            _renderGBufferEffect.Begin();
            _renderGBufferEffect.BeginPass(1);

            if( Monitor.TryEnter(_projectileTracer))
            {
                try
                {
                    foreach (var tracer in _projectileTracer)
                    {
                        tracer.Render();
                    }
                }
                finally
                {
                    Monitor.Exit(_projectileTracer);
                }
            }
            _renderGBufferEffect.EndPass();
            _renderGBufferEffect.End();



            _turretTrans = Matrix.Identity * Matrix.RotationY((float)DxUtils.ConvertDegreesToRadians(-TurretYaw));
            _renderGBufferEffect.SetValue("world", _turretTrans * TrackBallTransform);
            _renderGBufferEffect.SetValue("maxAnisotropy", ApplicationSettings.Default.AnisotropicFilterLevel);

            if (_turretMesh != null)
                _turretMesh.Render(_renderGBufferEffect, ref triangleCount, ModelType);

            _gunTrans = Matrix.RotationX((float)DxUtils.ConvertDegreesToRadians(-GunPitch)) * Matrix.Translation(GunPosition) * _turretTrans;
            _renderGBufferEffect.SetValue("world", _gunTrans * TrackBallTransform);

            if (_gunMesh != null)
                _gunMesh.Render(_renderGBufferEffect, ref triangleCount, ModelType);

            _hullTrans = Matrix.Translation(-TurretPosition);
            _renderGBufferEffect.SetValue("world", _hullTrans * TrackBallTransform);

            if (_hullMesh != null)
                _hullMesh.Render(_renderGBufferEffect, ref triangleCount, ModelType);

            _chassisTrans = _hullTrans*Matrix.Translation(-HullPosition);
            _renderGBufferEffect.SetValue("world", _chassisTrans * TrackBallTransform);

            //device.SetRenderState(RenderState.AlphaTestEnable, true);
            //device.SetRenderState(RenderState.AlphaRef,1);
            //device.SetRenderState(RenderState.AlphaFunc, Compare.Greater);
            
            if (_chassisMesh != null)
                _chassisMesh.Render(_renderGBufferEffect, ref triangleCount, ModelType);

            TriangleCount = triangleCount;
            //device.SetRenderState(RenderState.AlphaTestEnable, false); 
        }


        private void ClearGBuffer()
        {
            _clearGBufferEffect.Technique = _clearGBufferEffect.GetTechnique(0);
            _clearGBufferEffect.Begin();
            _clearGBufferEffect.BeginPass(0);
            _quadRender.Render(Renderer.Device);
            _clearGBufferEffect.EndPass();
            _clearGBufferEffect.End();
        }

        private void ResolveGBuffer()
        {
            var device = Renderer.Device;
            //device.SetRenderTarget(0, null);
            device.SetRenderTarget(1, null);
            device.SetRenderTarget(2, null);
        }

        private void SetGBuffer()
        {
            var device = Renderer.Device;
            device.SetRenderTarget(0, _colorSurface);
            device.SetRenderTarget(1, _normalSurface);
            device.SetRenderTarget(2, _positionSurface);
        }


        protected override void Attach()
        {
            if (Renderer == null)
                return;

            this.Renderer.Resetted += OnRenderResetted;
            var device = Renderer.Device;


            // Compiles the effect
            _showGBufferEffect = Effect.FromFile(device, @"Graphics\Effect\ShowGBuffer.fx", ShaderFlags.None);
            
            _clearGBufferEffect = Effect.FromFile(device, @"Graphics\Effect\ClearGBuffer.fx", ShaderFlags.None);

            _renderGBufferEffect = Effect.FromFile(device, @"Graphics\Effect\RenderGBuffer.fx", ShaderFlags.None);

            _lightingTankEffect = Effect.FromFile(device, @"Graphics\Effect\LightingTank.fx", ShaderFlags.None);

            _buildArmorColorEffect = Effect.FromFile(device, @"Graphics\Effect\BuildArmorColor.fx", ShaderFlags.None);

            _randomMap = Texture.FromFile(device, @"Graphics\Texture\random.dds");

            _smaa = new SMAA(device, 1, 1, SMAA.Preset.PresetUltra);

            Log.Info("hangar scene attached to device");

            this.InitializeHud(device);

            ResetScene(1, 1);
        }


        public override void Dispose()
        {
            Detach();
            base.Dispose();
        }

        protected override void Detach()
        {
            //reset 
            Disposer.RemoveAndDispose(ref _depthStencil);
            Disposer.RemoveAndDispose(ref _quadRender);
            Disposer.RemoveAndDispose(ref _finalColorSurface);
            Disposer.RemoveAndDispose(ref _colorSurface);
            Disposer.RemoveAndDispose(ref _positionSurface);
            Disposer.RemoveAndDispose(ref _normalSurface);
            Disposer.RemoveAndDispose(ref _finalColorMap);
            Disposer.RemoveAndDispose(ref _colorMap);
            Disposer.RemoveAndDispose(ref _positionMap);
            Disposer.RemoveAndDispose(ref _normalMap);

            //model
            Disposer.RemoveAndDispose(ref _hullMesh);
            Disposer.RemoveAndDispose(ref _chassisMesh);
            Disposer.RemoveAndDispose(ref _turretMesh);
            Disposer.RemoveAndDispose(ref _gunMesh);


            //effect
            Disposer.RemoveAndDispose(ref _showGBufferEffect);
            Disposer.RemoveAndDispose(ref _clearGBufferEffect);
            Disposer.RemoveAndDispose(ref _renderGBufferEffect);
            Disposer.RemoveAndDispose(ref _lightingTankEffect);
            Disposer.RemoveAndDispose(ref _buildArmorColorEffect);
            //other
            Disposer.RemoveAndDispose(ref _armorColorMap);
            Disposer.RemoveAndDispose(ref _randomMap);
            Disposer.RemoveAndDispose(ref _smaa);

            Log.Info("hangar scene detached");
        }

        public override void RenderScene(DrawEventArgs args)
        {
            UpdateTargetPositionAnimation(args.DeltaTime.TotalSeconds);

            TestUpdateModel(args.DeltaTime.TotalSeconds);


            var device = Renderer.Device;
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer | ClearFlags.Stencil, new SharpDX.Color(0, 0, 0, 0), 1.0f, 0);

            var dst = device.GetRenderTarget(0);

            if (ModelType == Model.ModelType.Collision && _armorColorMapDirty) 
            {
                UpdateArmorColorMap();
            }

            device.BeginScene();
            
            RenderGBuffer();

            if (ApplicationSettings.Default.SMAAEnabled)
            {
                device.SetRenderTarget(0, _finalColorSurface);             
            }
            else
            {
                device.SetRenderTarget(0, dst);
            }

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer , new SharpDX.Color(0, 0, 0, 0), 1.0f, 0);

            device.SetTexture(0, _colorMap);
            device.SetTexture(1, _normalMap);
            device.SetTexture(2, _positionMap);
            device.SetTexture(3, _randomMap);

            if (ModelType == Model.ModelType.Collision)
            {
                device.SetTexture(4, _armorColorMap);

                _lightingTankEffect.SetValue("tankThickestArmor", (float)TankThickestArmor + 0.125f);
                _lightingTankEffect.SetValue("tankThinnestArmor", (float)TankThinnestArmor - 0.125f);
                _lightingTankEffect.SetValue("tankThickestSpacingArmor", (float)TankThickestSpacingArmor + 0.125f);
                _lightingTankEffect.SetValue("tankThinnestSpacingArmor", (float)TankThinnestSpacingArmor - 0.125f);

                var regularArmorValueSelectionMax = (float)Math.Max(RegularArmorValueSelectionBegin, RegularArmorValueSelectionEnd);
                var regularArmorValueSelectionMin = (float)Math.Min(RegularArmorValueSelectionBegin, RegularArmorValueSelectionEnd);

                _lightingTankEffect.SetValue("regularArmorValueSelectionMax", regularArmorValueSelectionMax + 0.4f);
                _lightingTankEffect.SetValue("regularArmorValueSelectionMin", regularArmorValueSelectionMin - 0.4f);

                var spacingArmorValueSelectionMax = (float)Math.Max(SpacingArmorValueSelectionBegin, SpacingArmorValueSelectionEnd);
                var spacingArmorValueSelectionMin = (float)Math.Min(SpacingArmorValueSelectionBegin, SpacingArmorValueSelectionEnd);

                _lightingTankEffect.SetValue("spacingArmorValueSelectionMax", spacingArmorValueSelectionMax + 0.4f);
                _lightingTankEffect.SetValue("spacingArmorValueSelectionMin", spacingArmorValueSelectionMin - 0.4f);

                _lightingTankEffect.SetValue("hasRegularArmorHintValue", HasRegularArmorHintValue);
                _lightingTankEffect.SetValue("hasSpacingArmorHintValue", HasSpacingArmorHintValue);

                if (HasRegularArmorHintValue)
                {
                    _lightingTankEffect.SetValue("regularArmorHintValue", (float)RegularArmorHintValue);
                }

                if (HasSpacingArmorHintValue)
                {
                    _lightingTankEffect.SetValue("spacingArmorHintValue", (float)SpacingArmorHintValue);
                }

                _highLightValue = MaxHighlightDelta + MaxHighlightDelta * Math.Cos(args.TotalTime.TotalSeconds * 7.4);

                _lightingTankEffect.SetValue("highLightValue", (float)_highLightValue);

                _lightingTankEffect.SetValue("useBlackEdge", ApplicationSettings.Default.CollisionModelStrokeEnabled);
                
                _lightingTankEffect.Technique = _lightingTankEffect.GetTechnique(1);
            }
            else
            {
                _lightingTankEffect.Technique = _lightingTankEffect.GetTechnique(0);
            }

            _lightingTankEffect.SetValue("useSSAO",ApplicationSettings.Default.SSAOEnabled);

            _lightingTankEffect.Begin();
            _lightingTankEffect.BeginPass(0);

            _quadRender.Render(device);

            _lightingTankEffect.EndPass();
            _lightingTankEffect.End();

            this.RenderHud(args);


            if (ApplicationSettings.Default.SMAAEnabled)
            {
                device.SetRenderTarget(0, dst);
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer | ClearFlags.Stencil, new SharpDX.Color(0, 0, 0, 0), 1.0f, 0);
                _smaa.Apply(_finalColorMap, _finalColorMap, dst);
            }

            if (_fps != null)
            {
                _fps.AddFrame(args.TotalTime);
                this.Fps = _fps.Value;
            }

            UpdateHitTest();

            Renderer.Device.EndScene();
        }

        private void UpdateHitTest()
        {
            if (IsHitTestEnabled && CameraMode == CameraMode.Sniper)
            {
                var ray = new Ray(_shotPosition, _targetPosition - _shotPosition);
                var invWorldTrans = Matrix.Invert(TrackBallTransform);
                ray.Position = invWorldTrans.TransformCoord(ray.Position);
                ray.Direction = invWorldTrans.TransformNormal(ray.Direction);
                ray.Direction.Normalize();

                TryHitTestCollisionModel(ray);
            }
        }

        private void TestUpdateModel(double p)
        {
            //TurretRotationAngle = TurretRotationAngle + p ;
        }

        private bool TryPickTracer(Ray ray)
        {
            var result = new ProjectileTracerHitTestResult();
            foreach (var tracer in _projectileTracer)
            {
                tracer.HitTest(ray, ref result);
            }

            if(result.ProjectileTracerHits.Count == 0)
                return false;

            var closesetTracer = result.ClosesetHit.Value.Tracer;

            TryHitTestCollisionModel(closesetTracer.TracerRay);

            return true;
        }

        private void TryHitTestCollisionModel(Ray ray)
        {
            var result = new CollisionModelHitTestResult(ray);

            TryHitTestTankMeshCollisionModel(ray, _hullMesh, _hullTrans, ref result);
            TryHitTestTankMeshCollisionModel(ray, _turretMesh, _turretTrans, ref result);
            TryHitTestTankMeshCollisionModel(ray, _gunMesh, _gunTrans, ref result);
            TryHitTestTankMeshCollisionModel(ray, _chassisMesh, _chassisTrans, ref result);

            _lastHitTestResult = result;

            var closesetArmorHit = result.ClosesetArmorHit;

            if (closesetArmorHit.HasValue)
            {
                if (closesetArmorHit.Value.Armor.IsSpacingArmor)
                {
                    var closesetSpacingArmorHit = closesetArmorHit.Value;

                    if (!HasSpacingArmorHintValue)
                        HasSpacingArmorHintValue = true;

                    if (SpacingArmorHintValue != closesetSpacingArmorHit.Armor.Value)
                        SpacingArmorHintValue = closesetSpacingArmorHit.Armor.Value;

                    var closesetRegularArmorHit = result.ClosesetRegularArmorHit;
                    if (closesetRegularArmorHit.HasValue)
                    {
                        if (!HasRegularArmorHintValue)
                            HasRegularArmorHintValue = true;

                        if (RegularArmorHintValue != closesetRegularArmorHit.Value.Armor.Value)
                            RegularArmorHintValue = closesetRegularArmorHit.Value.Armor.Value;
                    }
                    else
                    {
                        if (HasRegularArmorHintValue)
                            HasRegularArmorHintValue = false;
                    }
                }
                else
                {
                    if (HasSpacingArmorHintValue)
                        HasSpacingArmorHintValue = false;

                    var closesetRegularArmorHit = closesetArmorHit;
                    if (closesetRegularArmorHit.HasValue)
                    {
                        if (!HasRegularArmorHintValue)
                            HasRegularArmorHintValue = true;

                        if (RegularArmorHintValue != closesetRegularArmorHit.Value.Armor.Value)
                            RegularArmorHintValue = closesetRegularArmorHit.Value.Armor.Value;
                    }
                    else
                    {
                        if (HasRegularArmorHintValue)
                            HasRegularArmorHintValue = false;
                    }
                }

                if(!IsMouseOverModel)
                    IsMouseOverModel = true;
            }
            else
            {
                if (HasSpacingArmorHintValue)
                    HasSpacingArmorHintValue = false;
                if (HasRegularArmorHintValue)
                    HasRegularArmorHintValue = false;

                if (IsMouseOverModel)
                    IsMouseOverModel = false;
            }
            
                
           var shotTestResult = result.GetShootTestResult(TestShell);

           if (this.ShootTestResult != shotTestResult)
               ShootTestResult = shotTestResult;
        }

        private static Vector3 CalculateNormal(ref Vector3 p1, ref Vector3 p2, ref Vector3 p3)
        {
            float w0, w1, w2, v0, v1, v2, nx, ny, nz;
            w0 = p2.X - p1.X; w1 = p2.Y - p1.Y; w2 = p2.Z - p1.Z;
            v0 = p3.X - p1.X; v1 = p3.Y - p1.Y; v2 = p3.Z - p1.Z;
            nx = (w1 * v2 - w2 * v1);
            ny = (w2 * v0 - w0 * v2);
            nz = (w0 * v1 - w1 * v0);
            return new Vector3(nx, ny, nz);
        }

        private void TryHitTestTankMeshCollisionModel(Ray ray, TankMesh mesh, Matrix trans,ref CollisionModelHitTestResult result)
        {
            if (mesh == null)
                return;

            if (mesh.CollisionGroup == null)
                return;

            var invTrans = Matrix.Invert(trans);
            ray.Position = invTrans.TransformCoord(ray.Position);
            ray.Direction = invTrans.TransformNormal(ray.Direction);
            ray.Direction.Normalize();

            var collisionGroup = mesh.CollisionGroup.ToArray();

            foreach (var group in collisionGroup)
            {
                foreach (var theTriangle in group.Triangles)
                {
                    var triangle = theTriangle;
					if (ray.Intersects(ref triangle.V1, ref triangle.V2, ref triangle.V3, out float distance))
					{
						var normal = HangarScene.CalculateNormal(ref triangle.V1, ref triangle.V2, ref triangle.V3);
						normal.Normalize();
						var dot = Math.Abs(Vector3.Dot(ray.Direction, -normal));
						result.Hits.Add(new CollisionModelHit { Distance = distance, Armor = group.Armor, InjectionCosine = dot, Mesh = mesh });
					}
				}
            }
        }

        public void AddProjectileTracer()
        {
            if (_lastHitTestResult == null || _lastHitTestResult.Hits.Count == 0)
                return;
            
            var tracer = new ProjectileTracer(Renderer.Device,_lastHitTestResult.HitRay,_lastHitTestResult.OrderedHits,TestShell);

            _projectileTracer.Add(tracer);
        }

        public void AddProjectileTracers()
        {
            if (_lastHitTestResult == null || _lastHitTestResult.Hits.Count == 0)
                return;

            var random = new Random();

            for (int i = 0; i != 20; ++i)
            {
                var randomDir = _lastHitTestResult.HitRay.Direction +
                    new Vector3(random.NextFloat(-0.05f, 0.05f), random.NextFloat(-0.05f, 0.05f), random.NextFloat(-0.05f, 0.05f));
                randomDir.Normalize();
                var ray = new Ray(_lastHitTestResult.HitRay.Position, randomDir);

                var result = new CollisionModelHitTestResult(ray);

                TryHitTestTankMeshCollisionModel(ray, _hullMesh, _hullTrans, ref result);
                TryHitTestTankMeshCollisionModel(ray, _turretMesh, _turretTrans, ref result);
                TryHitTestTankMeshCollisionModel(ray, _gunMesh, _gunTrans, ref result);
                TryHitTestTankMeshCollisionModel(ray, _chassisMesh, _chassisTrans, ref result);

                if (result.Hits.Count != 0)
                {

                    var tracer = new ProjectileTracer(Renderer.Device, result.HitRay, result.OrderedHits, TestShell);

                    Monitor.Enter(_projectileTracer);
                    try
                    {
                        _projectileTracer.Add(tracer);
                    }
                    finally
                    {
                        Monitor.Exit(_projectileTracer);
                    }
                    
                }
            }
        }

        private Ray GetPickRay(float x, float y)
        {
            var halfViewportWidth = (float)_width / 2;
            var halfViewportHeight = (float)_height / 2;
            var viewportCenterX = halfViewportWidth;
            var viewportCenterY = halfViewportHeight;

            var xn = (x - viewportCenterX) / halfViewportWidth;
            var yn = -(y - viewportCenterY) / halfViewportHeight;


            var px = xn / _proj[0, 0];
            var py = yn / _proj[1, 1];

            var ray = new Ray(Vector3.Zero, new Vector3(px, py, 1.0f));

            ray.Position = _invView.TransformCoord(ray.Position);
            ray.Direction = _invView.TransformNormal(ray.Direction);
            ray.Direction.Normalize();

            return ray;
        }

	    private void InvalidateArmorColorMap()
        {
            _armorColorMapDirty = true;
        }


	    private struct StopPointVertex
        {
            public float Offset;
            public ColorBGRA Color;
        }

        private void UpdateArmorColorMap()
        {
            Log.Info("update armor color map."); 
            var device = Renderer.Device;

            Disposer.RemoveAndDispose(ref _armorColorMap);
            _armorColorMap = new Texture(device, 1024, 1, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);

            using (var armorColorSurface = _armorColorMap.GetSurfaceLevel(0))
            {
                device.SetRenderTarget(0, armorColorSurface);
                //

                using (var vertexDeclaration = new VertexDeclaration(device, new[]
                {
                    new VertexElement(0,0,DeclarationType.Float1,DeclarationMethod.Default,DeclarationUsage.Position,0),
                    new VertexElement(0,4,DeclarationType.Color,DeclarationMethod.Default,DeclarationUsage.Color,0),
                    VertexElement.VertexDeclarationEnd
                }))
                {
                    device.VertexDeclaration = vertexDeclaration;

                    var vertices = RegularArmorSpectrumStops.Select(
                        (p) =>
                        {
                            return new StopPointVertex
                            {
                                Offset = (float)p.Offset - 1.0f,
                                Color = new ColorBGRA(p.Color.R, p.Color.G, p.Color.B, p.Color.A)
                            };
                        }).ToArray();
                    var verticesSpacing = SpacingArmorSpectrumStops.Select(
                    (p) =>
                    {
                        return new StopPointVertex
                        {
                            Offset = (float)p.Offset,
                            Color = new ColorBGRA(p.Color.R, p.Color.G, p.Color.B, p.Color.A)
                        };
                    }).ToArray();

                    _buildArmorColorEffect.Technique = _buildArmorColorEffect.GetTechnique(0);
                    _buildArmorColorEffect.Begin();
                    _buildArmorColorEffect.BeginPass(0);
                    device.DrawUserPrimitives(PrimitiveType.LineStrip,0, vertices.Length - 1, vertices);
                    device.DrawUserPrimitives(PrimitiveType.LineStrip,0, verticesSpacing.Length - 1, verticesSpacing);
                    _buildArmorColorEffect.EndPass();
                    _buildArmorColorEffect.End();
                }          
            }
            _armorColorMapDirty = false;
        }



        public void ClearAllProjectileTracer()
        {
            Monitor.Enter(_projectileTracer);
            try
            {
                _projectileTracer.Clear();
            }
            finally
            {
                Monitor.Exit(_projectileTracer);
            }          
        }


    }
}
