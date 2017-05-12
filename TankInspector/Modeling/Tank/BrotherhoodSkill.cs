namespace Smellyriver.TankInspector.Modeling
{
	internal class BrotherhoodSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:brotherhood";
        public const string CrewTrainingLevelIncrementSkillKey = "crewTrainingLevelIncrement";

        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Lowest;

	    protected override CrewSkillType Type => CrewSkillType.Perk;

	    public override CrewRoleType CrewRole => CrewRoleType.All;

	    public override string[] EffectiveDomains => new[] { SkillDomain };
	    public override int Priority => -100;

	    public BrotherhoodSkill(Database database)
            : base(database)
        {

        }

        protected override void Execute(ModificationContext context, double level)
        {
            context.SetValue(this.EffectiveDomains[0], CrewTrainingLevelIncrementSkillKey, this.Parameters["crewLevelIncrease"]);
        }

        protected override void Clear(ModificationContext context)
        {
            context.SetValue(this.EffectiveDomains[0], CrewTrainingLevelIncrementSkillKey, 0);
        }
    }
}
