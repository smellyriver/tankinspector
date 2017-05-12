namespace Smellyriver.TankInspector.Modeling
{
	internal class IntuitionSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:intuition";
        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;

	    protected override CrewSkillType Type => CrewSkillType.Perk;

	    public override CrewRoleType CrewRole => CrewRoleType.Loader;

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    public IntuitionSkill(Database database)
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
