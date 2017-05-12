namespace Smellyriver.TankInspector.Modeling
{
	internal class GunnerSkill : CrewBasicSkill
    {
        public const string SkillDomain = "crewSkill:gunner";
        public const string AccuracyFactorSkillKey = "accuracyFactor";
        public const string AimingTimeFactorSkillKey = "aimingTimeFactor";
        public const string ShotDispersionFactorSkillKey = "shotDispersionFactor";
        public const string TurretRotationSpeedSkillKey = "turretRotationSpeed";

        public override string Name => App.GetLocalizedString("Gunner");

	    public override string ShortDescription => App.GetLocalizedString("GunnerSkillShortDescription");

	    public override CrewRoleType CrewRole => CrewRoleType.Gunner;

	    public override string Description => App.GetLocalizedString("GunnerSkillDescription");

	    public override string Icon => "gunner.png";

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    protected override void Execute(ModificationContext context, double level)
        {
            var decrementalFactor = CrewBasicSkill.GetDecrementalSkillFactor(level);
            context.SetValue(this.EffectiveDomains[0], AccuracyFactorSkillKey, decrementalFactor);
            context.SetValue(this.EffectiveDomains[0], AimingTimeFactorSkillKey, decrementalFactor);
            context.SetValue(this.EffectiveDomains[0], ShotDispersionFactorSkillKey, decrementalFactor);

            var incrementalFactor = CrewBasicSkill.GetIncrementalSkillFactor(level);
            context.SetValue(this.EffectiveDomains[0], TurretRotationSpeedSkillKey, incrementalFactor);
        }
    }
}
