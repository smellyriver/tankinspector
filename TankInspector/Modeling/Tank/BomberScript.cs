using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class BomberScript : ConsumableScript
    {
        public override string[] EffectiveDomains => new[] { "bomber" };

	    public BomberScript(Database database)
            : base(database)
        {

        }

        public override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                default:
                    return base.DeserializeSection(name, reader);
            }
        }

        public override void Execute(ModificationContext context, object args)
        {

        }
    }
}
