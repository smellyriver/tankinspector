using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal abstract class ConsumableScript : DatabaseObject, IModifier, ISectionDeserializable
    {
        public static ConsumableScript Resolve(XmlReader reader, Database database)
        {
            var type = reader.ReadString().Trim();
            ConsumableScript script;
            switch (type)
            {
                case "Extinguisher":
                    script = new ExtinguisherScript(database);
                    break;
                case "Repairkit":
                    script = new RepairkitScript(database);
                    break;
                case "Fuel":
                    script = new FuelScript(database);
                    break;
                case "Stimulator":
                    script = new StimulatorScript(database);
                    break;
                case "RemovedRpmLimiter":
                    script = new RemovedRpmLimiterScript(database);
                    break;
                case "Artillery":
                    script = new ArtilleryScript(database);
                    break;
                case "Bomber":
                    script = new BomberScript(database);
                    break;
                case "Afterburning":
                    script = new AfterburningScript(database);
                    break;
                default:
                    throw new NotSupportedException();
            }

            script.Deserialize(reader);

            return script;
        }

        public abstract string[] EffectiveDomains { get; }
        public abstract void Execute(ModificationContext context, object args);

        public ConsumableScript(Database database)
            : base(database)
        {

        }

        public virtual bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                default:
                    return false;
            }
        }

        public bool IsWrapped => false;

	    public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }
    }
}
