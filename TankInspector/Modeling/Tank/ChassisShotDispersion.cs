using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class ChassisShotDispersion : ISectionDeserializable
    {
        public double _movement;
        [Stat("ChassisDispersionOnVehicleMoveFullName", DataAnalysis.ComparisonMode.LowerBetter)]
        public double Movement => _movement;

	    public double _rotation;
        [Stat("ChassisDispersionOnVehicleRotationFullName", DataAnalysis.ComparisonMode.LowerBetter)]
        public double Rotation => _rotation;


	    public ChassisShotDispersion(double movement, double rotation)
        {
            _movement = movement;
            _rotation = rotation;
        }

        public ChassisShotDispersion()
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
                case "vehicleMovement":
                    reader.Read(out _movement);
                    return true;
                case "vehicleRotation":
                    reader.Read(out _rotation);
                    return true;
                default:
                    return false;
            }
        }


        bool ISectionDeserializable.IsWrapped => false;
    }
}
