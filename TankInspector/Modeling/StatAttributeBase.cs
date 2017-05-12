using System;

namespace Smellyriver.TankInspector.Modeling
{
	internal abstract class StatAttributeBase : Attribute
    {
        public string NameL10N { get; set; }

        public StatAttributeBase(string nameL10N)
        {
            this.NameL10N = nameL10N;
        }
    }
}
