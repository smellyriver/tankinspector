namespace Smellyriver.TankInspector.Modeling
{
	internal class VirtuosoSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:virtuoso";
        public const string ChassisRotationSpeedIncrementFactor = "chassisRotationSpeedIncrementFactor";

        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;
	    protected override CrewSkillType Type => CrewSkillType.Skill;

	    public override CrewRoleType CrewRole => CrewRoleType.Driver;

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    public VirtuosoSkill(Database database)
            : base(database)
        {

        }

        protected override void Execute(ModificationContext context, double level)
        {
            context.SetValue(this.EffectiveDomains[0], ChassisRotationSpeedIncrementFactor, this.Parameters["rotationSpeedFactorPerLevel"] * level);
        }

        protected override void Clear(ModificationContext context)
        {
            context.SetValue(this.EffectiveDomains[0], ChassisRotationSpeedIncrementFactor, 0);
        }
    }
}
