using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class MatchmakingWeightRules : ISectionDeserializable
    {
        internal Dictionary<string, double> SpecialTankMmWeights;

        internal List<double> TierWeights;
        internal Dictionary<TankClass, double> ClassWeights;

        public MatchmakingWeightRules()
        {
            SpecialTankMmWeights = new Dictionary<string, double>();
            TierWeights = new List<double>();
            ClassWeights = new Dictionary<TankClass, double>();
        }


        public bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "byVehicleModule":
                    while (reader.IsStartElement("name"))
                    {
                        var tankKey = reader.ReadString();
                        reader.ReadStartElement("weight");
						reader.Read(out double weight);
						reader.ReadEndElement();
                        reader.ReadEndElement();

                        SpecialTankMmWeights[tankKey] = weight;
                    }
                    return true;

                case "byComponentLevels":
                    var weights = reader.ReadString();
                    var weightsArray = weights.Split(' ');
                    foreach (var weight in weights.Split(' '))
                        TierWeights.Add(double.Parse(weight, CultureInfo.InvariantCulture));

                    return true;

                case "byVehicleClasses":
                    while (reader.IsStartElement())
                    {
                        var className = reader.Name;
                        reader.ReadStartElement();
						reader.Read(out double weight);
						reader.ReadEndElement();

                        switch (className)
                        {
                            case "lightTank":
                                ClassWeights[TankClass.LightTank] = weight;
                                break;
                            case "mediumTank":
                                ClassWeights[TankClass.MediumTank] = weight;
                                break;
                            case "heavyTank":
                                ClassWeights[TankClass.HeavyTank] = weight;
                                break;
                            case "AT-SPG":
                                ClassWeights[TankClass.TankDestroyer] = weight;
                                break;
                            case "SPG":
                                ClassWeights[TankClass.SelfPropelledGun] = weight;
                                break;
                        }

                    }

                    return true;

                case "modulesWeightMultipliers":
                case "bySquadSize":
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

        internal double GetMatchmakingWeight(Tank tank)
        {
	        if (this.SpecialTankMmWeights.TryGetValue(tank.ColonFullKey, out double weight))
				return weight;
	        return this.TierWeights[tank.Tier - 1] * this.ClassWeights[tank.Class];
        }
    }
}
