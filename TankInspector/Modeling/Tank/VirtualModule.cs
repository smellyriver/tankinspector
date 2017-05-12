using System;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal abstract class VirtualModule : VirtualTankObject, IModule
    {

        public override TankObjectType ObjectType => (TankObjectType)this.Type;

	    public abstract ModuleType Type { get; }

        public double Weight { get; internal set; }

        public TankObjectKey OwnerKey { get; internal set; }

        public override TankObjectKey ObjectKey => TankObjectKey.Create(this.OwnerKey, this.ObjectType, this.Key);
    }
}
