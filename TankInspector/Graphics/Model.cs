using System;
using System.IO;
using Smellyriver.TankInspector.Modeling;
using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.UIComponents;
using System.Threading.Tasks;
using log4net;
using System.Reflection;

namespace Smellyriver.TankInspector.Graphics
{
	internal class Model : NotificationObject
    {
        public Task LoadingTask { get; set; }

        public ModelVisual Visual { get; private set; }

        public ModelPrimitive Primitive { get; private set; }

        public ModelVisual CollisionVisual { get; private set; }

        public ModelPrimitive CollisionPrimitive { get; private set; }

        private readonly ITankObject _tankObject;

        private readonly ModelViewModel _modelViewerViewModel;

        public Database Database => _modelViewerViewModel.Database;

	    public string ModelName => _tankObject.ShortName;

	    public IHasArmor ArmorObject => (IHasArmor)_tankObject;

	    public IHasModel ModelObject => (IHasModel)_tankObject;

	    public ITank TankObject => (ITank)ModuleViewModel.Owner.Tank;


	    public ModuleViewModel ModuleViewModel { get; }

        public enum ModelType
        {
            Undamaged,
            Collision,
            Destroyed,
            Exploded,
        };

        private static readonly ILog Log = SafeLog.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

        private PackageStream OpenVisualFile(string modelFile)
        {
            var visualFile = Path.ChangeExtension(modelFile, ".visual");    // pre 10.0
            if (!PackageStream.IsFileExisted(this.Database.PackageDatabase, visualFile))
                visualFile = Path.ChangeExtension(modelFile, ".visual_processed");  // post 10.0

            return new PackageStream(this.Database.PackageDatabase, visualFile);
        }

        private PackageStream OpenPrimitiveFile(string modelFile)
        {
            var primitiveFile = Path.ChangeExtension(modelFile, ".primitives");    // pre 10.0
            if (!PackageStream.IsFileExisted(this.Database.PackageDatabase, primitiveFile))
                primitiveFile = Path.ChangeExtension(modelFile, ".primitives_processed");  // post 10.0

            return new PackageStream(this.Database.PackageDatabase, primitiveFile);
        }


        public Model(ModelViewModel modelViewerViewModel, ITankObject tankObject, ModuleViewModel moduleViewModel)
        {
            ModuleViewModel = moduleViewModel;
            _modelViewerViewModel = modelViewerViewModel;
            _tankObject = tankObject;
            DispatchUniqueProperty();

            var model = this.ModelObject.Model.Undamaged;

            LoadingTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    using (Diagnostics.PotentialExceptionRegion)
                    {
                        using (var visualStream = this.OpenVisualFile(model))
                        {
                            this.Visual = ModelVisual.ReadFrom(visualStream);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Info("exception occurred when load visual", e);
                }
            }).ContinueWith((_) =>
            {
                try
                {
                    using (Diagnostics.PotentialExceptionRegion)
                    {
                        using (var modelStream = this.OpenPrimitiveFile(model))
                        {
                            this.Primitive = ModelPrimitive.ReadFrom(modelStream, this.Visual, false);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Info("exception occurred when load primitive", e);
                }
            });


            var collisionModel = this.ModelObject.Model.Collision;

            LoadingTask = LoadingTask.ContinueWith((_) =>
            {
                try
                {
                    using (Diagnostics.PotentialExceptionRegion)
                    {

                        using (var visualStream = this.OpenVisualFile(collisionModel))
                        {
                            this.CollisionVisual = ModelVisual.ReadFrom(visualStream);
                            foreach (var rendetSet in CollisionVisual.RenderSets)
                            {
                                foreach (var groupKv in rendetSet.Geometry.ModelPrimitiveGroups)
                                {
                                    var material = groupKv.Value.Material;
                                    material.ShowArmor = true;

									if (ArmorObject.Armor.TryGetArmorValue(material.Identifier, out ArmorGroup armor))
									{
										material.Armor = armor;
									}
									else
									{
										material.Armor = new ArmorGroup();
									}
								}
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Info("exception occurred when load collision visual", e);
                }
            }).ContinueWith((_) =>
            {
                try
                {
                    using (Diagnostics.PotentialExceptionRegion)
                    {
                        using (var modelStream = OpenPrimitiveFile(collisionModel))
                        {
                            this.CollisionPrimitive = ModelPrimitive.ReadFrom(modelStream, CollisionVisual, true);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Info("exception occurred when load collision primitive", e);
                }
            });
        }

        private void DispatchUniqueProperty()
        {
            if (_tankObject is Chassis)
            {
                var chassis = (Chassis)_tankObject;
                _modelViewerViewModel.HullPosition = chassis.HullPosition.ToPoint3D();
            }
            else if (_tankObject is Hull)
            {
                var hull = (Hull)_tankObject;
                _modelViewerViewModel.TurretPosition = hull.TurretPosition.ToPoint3D();
            }
            else if (_tankObject is Turret)
            {
                var turret = (Turret)_tankObject;
                _modelViewerViewModel.GunPosition = turret.GunPosition.ToPoint3D();
            }
        }
    }
}
