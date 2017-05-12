using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class ArmorGroup : ISectionDeserializable, ICloneable
    {
        private double _value;
        public double Value => _value;

	    private double _vehicleDamageFactor;
        public double VehicleDamageFactor => _vehicleDamageFactor;

	    public bool IsSpacingArmor => VehicleDamageFactor == 0.0;

	    private bool _useArmorHomogenization;
        public bool UseArmorHomogenization => _useArmorHomogenization;

	    private bool _useHitAngle;
        public bool UseHitAngle => _useHitAngle;

	    private bool _useAntifragmentationLining;
        public bool UseAntifragmentationLining => _useAntifragmentationLining;

	    private bool _mayRicochet;
        public bool MayRicochet => _mayRicochet;

	    private bool _collideOnceOnly;
        public bool CollideOnceOnly => _collideOnceOnly;

	    private double _chanceToHitByProjectile;
        public double ChanceToHitByProjectile => _chanceToHitByProjectile;

	    private double _chanceToHitByExplosion;
        public double ChanceToHitByExplosion => _chanceToHitByExplosion;

	    private bool _continueTraceIfNoHit;
        public bool ContinueTraceIfNoHit => _continueTraceIfNoHit;

	    public ArmorGroup()
        {

        }

        public bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case SectionDeserializableImpl.TitleToken:
                    if (reader.NodeType == XmlNodeType.Text)
                        reader.Read(out _value);
                    return true;
                case "vehicleDamageFactor":
                    reader.Read(out _vehicleDamageFactor);
                    return true;
                case "useArmorHomogenization":
                    reader.Read(out _useArmorHomogenization);
                    return true;
                case "useHitAngle":
                    reader.Read(out _useHitAngle);
                    return true;
                case "useAntifragmentationLining":
                    reader.Read(out _useAntifragmentationLining);
                    return true;
                case "mayRicochet":
                    reader.Read(out _mayRicochet);
                    return true;
                case "collideOnceOnly":
                    reader.Read(out _collideOnceOnly);
                    return true;
                case "chanceToHitByProjectile":
                    reader.Read(out _chanceToHitByProjectile);
                    return true;
                case "chanceToHitByExplosion":
                    reader.Read(out _chanceToHitByExplosion);
                    return true;
                case "continueTraceIfNoHit":
                    reader.Read(out _continueTraceIfNoHit);
                    return true;
                case "extra":
                case "damageKind":
                    return false;
                default:
                    return false;

            }
        }

        public bool IsWrapped => false;

	    public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
        
        public ArmorGroup Clone()
        {
            return (ArmorGroup)this.MemberwiseClone();
        }
    }
}
