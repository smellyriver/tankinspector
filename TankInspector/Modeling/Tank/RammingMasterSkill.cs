namespace Smellyriver.TankInspector.Modeling
{
	internal class RammingMasterSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:rammingMaster";
        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;

	    protected override CrewSkillType Type => CrewSkillType.Skill;

	    public override CrewRoleType CrewRole => CrewRoleType.Driver;
	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    public RammingMasterSkill(Database database)
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
