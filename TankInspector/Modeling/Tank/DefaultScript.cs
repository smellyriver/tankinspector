using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class DefaultScript : ConsumableScript
    {
        public override string[] EffectiveDomains => new[] { "unknown" };

	    public DefaultScript(Database database,string type)
            : base(database)
        {
            EffectiveDomains.SetValue(type, 0);
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
