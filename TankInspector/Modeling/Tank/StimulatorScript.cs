using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class StimulatorScript : ConsumableScript
    {
        private int _crewLevelIncrease = 0;
        public int CrewLevelIncrease => _crewLevelIncrease;

	    public override string[] EffectiveDomains => new[] { "stimulator" };

	    public StimulatorScript(Database database)
            : base(database)
        {

        }

        public override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "crewLevelIncrease":
                    reader.Read(out _crewLevelIncrease);
                    return true;
                default:
                    return base.DeserializeSection(name, reader);
            }
        }

        public override void Execute(ModificationContext context, object args)
        {
            context.SetValue(this.EffectiveDomains[0], "crewLevelIncrease", _crewLevelIncrease);
        }


    }
}