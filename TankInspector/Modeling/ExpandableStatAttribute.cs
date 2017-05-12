namespace Smellyriver.TankInspector.Modeling
{
	internal class ExpandableStatAttribute : StatAttributeBase
    {
        public ExpandableStatAttribute(string nameL10N)
            : base(nameL10N)
        {

        }

        public ExpandableStatAttribute()
            : this(null)
        {

        }
    }
}
