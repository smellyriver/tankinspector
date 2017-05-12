using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class SpeedLimits : ISectionDeserializable
    {
        private double _forward;
        [Stat("ChassisForwardSpeedLimitFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double Forward
        {
            get => _forward;
	        set => _forward = value;
        }

        private double _backward;
        [Stat("ChassisBackwardSpeedLimitFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double Backward
        {
            get => _backward;
	        set => _backward = value;
        }


        public SpeedLimits(double forward, double backward)
        {
            _forward = forward;
            _backward = backward;
        }

        public SpeedLimits()
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
                case "forward":
                    reader.Read(out _forward);
                    return true;
                case "backward":
                    reader.Read(out _backward);
                    return true;
                default:
                    return false;
            }
        }


        bool ISectionDeserializable.IsWrapped => false;

	    public override string ToString()
        {
            return $"Speed Limits: Forward = {this.Forward}, Reverse = {this.Backward}";
        }
    }
}
