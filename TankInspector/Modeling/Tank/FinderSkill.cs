namespace Smellyriver.TankInspector.Modeling
{
	internal class FinderSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:finder";
        public const string VisionRadiusFactorSkillKey = "visionRadiusFactor";

        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;

	    public override string[] EffectiveDomains => new[] { SkillDomain };
	    public override CrewRoleType CrewRole => CrewRoleType.Radioman;
	    protected override CrewSkillType Type => CrewSkillType.Skill;

	    public FinderSkill(Database database)
            : base(database)
        {

        }

        protected override void Execute(ModificationContext context, double level)
        {
            context.SetValue(this.EffectiveDomains[0], VisionRadiusFactorSkillKey, this.Parameters["visionRadiusFactorPerLevel"] * level);
        }



        protected override void Clear(ModificationContext context)
        {
            context.SetValue(this.EffectiveDomains[0], VisionRadiusFactorSkillKey, 0);
        }
    }
}
