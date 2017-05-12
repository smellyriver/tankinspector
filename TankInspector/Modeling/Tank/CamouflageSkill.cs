namespace Smellyriver.TankInspector.Modeling
{
	internal class CamouflageSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:camouflage";
        public const string CamouflageFactorSkillKey = "camouflageFactor";

        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Average;

	    protected override CrewSkillType Type => CrewSkillType.Skill;

	    public override CrewRoleType CrewRole => CrewRoleType.All;

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    public CamouflageSkill(Database database)
            : base(database)
        {

        }

        protected override void Execute(ModificationContext context, double level)
        {
            context.SetValue(this.EffectiveDomains[0], CamouflageFactorSkillKey, 1.0 + 0.0075 * level);
        }

        protected override void Clear(ModificationContext context)
        {
            context.SetValue(this.EffectiveDomains[0], CamouflageFactorSkillKey, 1.0);
        }
    }
}
