namespace Smellyriver.TankInspector.Modeling
{
	internal class RepairSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:repair";

        protected override CrewSkillType Type => CrewSkillType.Skill;

	    public override CrewRoleType CrewRole => CrewRoleType.All;

	    public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Average;

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    public RepairSkill(Database database)
            : base(database)
        {

        }

        protected override void Execute(ModificationContext context, double level)
        {
            // todo: repair speed formular unknown
        }

        protected override void Clear(ModificationContext context)
        {
            
        }
    }
}
