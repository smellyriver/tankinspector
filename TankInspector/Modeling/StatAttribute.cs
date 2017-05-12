using Smellyriver.TankInspector.DataAnalysis;

namespace Smellyriver.TankInspector.Modeling
{
	internal class StatAttribute : StatAttributeBase
    {
        public ComparisonMode ComparisonMode { get; set; }
        

        public StatAttribute(string nameL10N, ComparisonMode comparisonMode)
            : base(nameL10N)
        {
            this.ComparisonMode = comparisonMode;
        }

        public StatAttribute(string nameL10N)
            : this(nameL10N, ComparisonMode.Plain)
        {

        }

        public StatAttribute()
            : this(null)
        {

        }
    }
}
