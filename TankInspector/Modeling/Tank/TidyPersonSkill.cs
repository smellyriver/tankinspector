namespace Smellyriver.TankInspector.Modeling
{
	internal class TidyPersonSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:tidyPerson";
        public const string EngineFireChanceDecrementSkillKey = "engineFireChanceDecrement";

        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;

	    protected override CrewSkillType Type => CrewSkillType.Perk;

	    public override CrewRoleType CrewRole => CrewRoleType.Driver;

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    public TidyPersonSkill(Database database)
            : base(database)
        {

        }

        protected override void Execute(ModificationContext context, double level)
        {
            context.SetValue(this.EffectiveDomains[0], EngineFireChanceDecrementSkillKey, this.Parameters["fireStartingChanceFactor"]);
        }

        protected override void Clear(ModificationContext context)
        {
            context.SetValue(this.EffectiveDomains[0], EngineFireChanceDecrementSkillKey, 1.0);
        }
    }
}
