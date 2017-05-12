using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class Radio : DamageableModule, IRadio
    {
        public override ModuleType Type => ModuleType.Radio;

	    private double _distance;

        [Stat("RadioSignalDistanceFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double Distance
        {
            get => _distance;
	        private set => _distance = value;
        }

        public Radio(Database database)
            : base(database)
        {

        }    

        protected override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "distance":
                    reader.Read(out _distance);
                    return true;
                case "tags":
                    return false;

                default:
                    if (base.DeserializeSection(name, reader))
                        return true;
                    else
                        return false;
            }

        }

    }
}
