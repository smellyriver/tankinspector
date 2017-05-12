namespace Smellyriver.TankInspector.Modeling
{
	internal class DesperadoSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:desperado";
        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;

	    protected override CrewSkillType Type => CrewSkillType.Perk;

	    public override CrewRoleType CrewRole => CrewRoleType.Loader;

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    public DesperadoSkill(Database database)
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
