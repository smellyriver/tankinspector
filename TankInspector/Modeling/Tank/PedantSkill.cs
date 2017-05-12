namespace Smellyriver.TankInspector.Modeling
{
	internal class PedantSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:pedant";
        public const string AmmobayHealthFactorSkillKey = "ammobayHealthFactor";

        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;

	    protected override CrewSkillType Type => CrewSkillType.Perk;
	    public override CrewRoleType CrewRole => CrewRoleType.Loader;

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    public PedantSkill(Database database)
            : base(database)
        {

        }
        protected override void Execute(ModificationContext context, double level)
        {
            context.SetValue(this.EffectiveDomains[0], AmmobayHealthFactorSkillKey, this.Parameters["ammoBayHealthFactor"]);
        }

        protected override void Clear(ModificationContext context)
        {
            context.SetValue(this.EffectiveDomains[0], AmmobayHealthFactorSkillKey, 1);
        }
    }
}
