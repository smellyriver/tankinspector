namespace Smellyriver.TankInspector.Modeling
{
	internal class SixthSenseSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:sixthSense";
        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;

	    protected override CrewSkillType Type => CrewSkillType.Perk;

	    public override CrewRoleType CrewRole => CrewRoleType.Commander;

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    public SixthSenseSkill(Database database)
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
