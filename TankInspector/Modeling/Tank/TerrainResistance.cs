using System;
using System.Globalization;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class TerrainResistance : IDeserializable
    {
	    [Stat("ChassisHardTerrainResistanceFullName", DataAnalysis.ComparisonMode.LowerBetter)]
        public double HardTerrain { get; private set; }

	    [Stat("ChassisMediumTerrainResistanceFullName", DataAnalysis.ComparisonMode.LowerBetter)]
        public double MediumTerrain { get; private set; }

	    [Stat("ChassisSoftTerrainResistanceFullName", DataAnalysis.ComparisonMode.LowerBetter)]
        public double SoftTerrain { get; private set; }

	    public TerrainResistance(double hard, double medium, double soft)
        {
            this.HardTerrain = hard;
            this.MediumTerrain = medium;
            this.SoftTerrain = soft;
        }

        public TerrainResistance()
        {

        }


        public void Deserialize(XmlReader reader)
        {
            var values = reader.ReadString();
            var valuesArray = values.Split(' ');
            this.HardTerrain = double.Parse(valuesArray[0], CultureInfo.InvariantCulture);
            this.MediumTerrain = double.Parse(valuesArray[1], CultureInfo.InvariantCulture);
            this.SoftTerrain = double.Parse(valuesArray[2], CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return
	            $"Terrain Resistance: Soft = {this.SoftTerrain}, Medium = {this.MediumTerrain}, Hard = {this.HardTerrain}";
        }
    }
}
