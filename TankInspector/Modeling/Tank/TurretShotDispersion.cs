using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class TurretShotDispersion : ISectionDeserializable
    {
        public double _afterShot;

        [Stat("GunDispersionAfterShotFullName", DataAnalysis.ComparisonMode.LowerBetter)]
        public double AfterShot => _afterShot;

	    public double _turretRotation;

        [Stat("GunDispersionOnTurretRotationFullName", DataAnalysis.ComparisonMode.LowerBetter)]
        public double TurretRotation => _turretRotation;

	    public double _gunDamaged;

        [Stat("GunDispersionWhileGunDamagedFullName", DataAnalysis.ComparisonMode.LowerBetter)]
        public double GunDamaged => _gunDamaged;

	    public TurretShotDispersion(double afterShot, double turretRotation, double gunDamaged)
        {
            _afterShot = afterShot;
            _turretRotation = turretRotation;
            _gunDamaged = gunDamaged;
        }

        public TurretShotDispersion()
        {

        }

        public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }

        bool ISectionDeserializable.DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "turretRotation":
                    reader.Read(out _turretRotation);
                    return true;
                case "afterShot":
                    reader.Read(out _afterShot);
                    return true;
                case "whileGunDamaged":
                    reader.Read(out _gunDamaged);
                    return true;
                default:
                    return false;
            }
        }


        bool ISectionDeserializable.IsWrapped => false;
    }
}
