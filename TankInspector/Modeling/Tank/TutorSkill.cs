namespace Smellyriver.TankInspector.Modeling
{
	internal class TutorSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:tutor";
        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;

	    protected override CrewSkillType Type => CrewSkillType.Skill;

	    public override CrewRoleType CrewRole => CrewRoleType.Commander;

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    public TutorSkill(Database database)
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
