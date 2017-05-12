using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal abstract class EquipmentScript : DatabaseObject, IModifier, ISectionDeserializable
    {

        public static EquipmentScript Resolve(XmlReader reader, Database database)
        {
            var type = reader.ReadString().Trim();
            EquipmentScript script;
            switch (type)
            {
                case "StaticFactorDevice":
                    script = new StaticFactorDeviceScript(database);
                    break;
                case "AntifragmentationLining":
                    script = new AntifragmentationLiningScript(database);
                    break;
                case "Stereoscope":
                    script = new StereoscopeScript(database);
                    break;
                case "CamouflageNet":
                    script = new CamouflageNetScript(database);
                    break;
                case "EnhancedSuspension":
                    script = new EnhancedSuspensionScript(database);
                    break;
                case "StaticAdditiveDevice":
                    script = new StaticAdditiveDeviceScript(database);
                    break;
                case "Grousers":
                    script = new GrousersScript(database);
                    break;
                default:
                    throw new NotSupportedException();
            }

            script.Deserialize(reader);

            return script;
        }

	    public double? Weight { get; private set; }

	    public double? VehicleWeightFraction { get; private set; }


	    public abstract string[] EffectiveDomains { get; }
        public abstract void Execute(ModificationContext context, object args);


        public EquipmentScript(Database database)
            : base(database)
        {

        }

        public virtual bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "weight":
                    double weight;
                    reader.Read(out weight);
                    this.Weight = weight;
                    return true;
                case "vehicleWeightFraction":
                    double vehicleWeightFraction;
                    reader.Read(out vehicleWeightFraction);
                    this.VehicleWeightFraction = vehicleWeightFraction;
                    return true;
                default:
                    return false;
            }
        }

        public double GetWeight(double tankWeight)
        {
	        if (this.Weight.HasValue)
                return this.Weight.Value;
	        if (this.VehicleWeightFraction.HasValue)
		        return tankWeight * this.VehicleWeightFraction.Value;
	        return 0.0;
        }

        public bool IsWrapped => false;

	    public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }
    }
}
