namespace Smellyriver.TankInspector.Modeling
{
	internal class ExpertSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:expert";
        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;

	    protected override CrewSkillType Type => CrewSkillType.Perk;

	    public override CrewRoleType CrewRole => CrewRoleType.Commander;

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    public ExpertSkill(Database database)
            : base(database)
        {

        }

        protected override void Execute(ModificationContext context, double level)
        {
            // dummy
        }

        protected override void Clear(ModificationContext context)
        {

        }
    }
}
