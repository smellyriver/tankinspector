namespace Smellyriver.TankInspector.Modeling
{
	internal class UniversalistSkill : CrewSkill
    {
        public const string EfficiencySkillKey = "efficiency";
        public const string SkillDomain = "crewSkill:universalist";

        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;

	    protected override CrewSkillType Type => CrewSkillType.Skill;

	    public override CrewRoleType CrewRole => CrewRoleType.Commander;

	    public override string[] EffectiveDomains => new[] { SkillDomain };
	    public override int Priority => -20;


	    public UniversalistSkill(Database database)
            : base(database)
        {

        }

        protected override void Execute(ModificationContext context, double level)
        {
            var effectiveLevel = level * this.Parameters["efficiency"];
            context.SetValue(this.EffectiveDomains[0], EfficiencySkillKey, effectiveLevel);
        }

        protected override void Clear(ModificationContext context)
        {
            context.SetValue(this.EffectiveDomains[0], EfficiencySkillKey, 0);
        }
    }
}
