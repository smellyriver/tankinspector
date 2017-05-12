namespace Smellyriver.TankInspector.Modeling
{
	internal class FireFightingSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:fireFighting";
        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Average;

	    protected override CrewSkillType Type => CrewSkillType.Skill;

	    public override CrewRoleType CrewRole => CrewRoleType.All;

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    public FireFightingSkill(Database database)
            : base(database)
        {

        }

        protected override void Execute(ModificationContext context, double level)
        {
            // todo: fire fighting speed formular unknown
        }

        protected override void Clear(ModificationContext context)
        {

        }
    }
}
