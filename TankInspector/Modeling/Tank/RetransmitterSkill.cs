namespace Smellyriver.TankInspector.Modeling
{
	internal class RetransmitterSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:retransmitter";
        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;

	    protected override CrewSkillType Type => CrewSkillType.Skill;
	    public override CrewRoleType CrewRole => CrewRoleType.Radioman;
	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    public RetransmitterSkill(Database database)
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
