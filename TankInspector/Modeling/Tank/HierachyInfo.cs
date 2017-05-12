namespace Smellyriver.TankInspector.Modeling
{
	internal class HierachyInfo<TTankObject>
        where TTankObject : TankObject
    {
        public TTankObject Target { get; }
        public int ExperiencePrice { get; }

        public HierachyInfo(TTankObject target, int experiencePrice)
        {
            this.Target = target;
            this.ExperiencePrice = experiencePrice;
        }
    }
}
