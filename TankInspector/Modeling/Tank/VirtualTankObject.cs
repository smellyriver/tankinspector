using System;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal abstract class VirtualTankObject : VirtualCommodity, ITankObject
    {

        public string Name { get; internal set; }

        public virtual TankObjectKey ObjectKey => new TankObjectKey(this.ObjectType, this.Key);

	    public abstract TankObjectType ObjectType { get; }

        public string ShortName { get; internal set; }

        public int Tier { get; internal set; }
    }
}
