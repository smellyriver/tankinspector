namespace Smellyriver.TankInspector.Modeling
{
	internal class SmoothTurretSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:smoothTurret";
        public const string ShotDispersionDecrementFactorSkillKey = "shotDispersionDecrementFactor";

        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;

	    protected override CrewSkillType Type => CrewSkillType.Skill;

	    public override CrewRoleType CrewRole => CrewRoleType.Gunner;
	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    public SmoothTurretSkill(Database database)
            : base(database)
        {

        }
        protected override void Execute(ModificationContext context, double level)
        {
            context.SetValue(this.EffectiveDomains[0], ShotDispersionDecrementFactorSkillKey, this.Parameters["shotDispersionFactorPerLevel"] * level);
        }

        protected override void Clear(ModificationContext context)
        {
            context.SetValue(this.EffectiveDomains[0], ShotDispersionDecrementFactorSkillKey, 0);
        }
    }
}
