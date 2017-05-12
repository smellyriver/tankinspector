namespace Smellyriver.TankInspector.Modeling
{
	internal class InventorSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:inventor";
        public const string RadioDistanceFactorSkillKey = "radioDistanceFactor";

        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    public override CrewRoleType CrewRole => CrewRoleType.Radioman;
	    protected override CrewSkillType Type => CrewSkillType.Skill;

	    public InventorSkill(Database database)
            : base(database)
        {

        }

        protected override void Execute(ModificationContext context, double level)
        {
            context.SetValue(this.EffectiveDomains[0], RadioDistanceFactorSkillKey, this.Parameters["radioDistanceFactorPerLevel"] * level);
        }

        protected override void Clear(ModificationContext context)
        {
            context.SetValue(this.EffectiveDomains[0], RadioDistanceFactorSkillKey, 0);
        }
    }
}
