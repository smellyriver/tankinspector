namespace Smellyriver.TankInspector.Modeling
{
	internal class LastEffortSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:lastEffort";
        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    protected override CrewSkillType Type => CrewSkillType.Perk;
	    public override CrewRoleType CrewRole => CrewRoleType.Radioman;

	    public LastEffortSkill(Database database)
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
