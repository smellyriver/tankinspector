namespace Smellyriver.TankInspector.Modeling
{
	internal class DriverSkill : CrewBasicSkill
    {
        public const string SkillDomain = "crewSkill:driver";
        public const string TerrainResistanceFactorSkillKey = "terrainResistanceFactor";
        public override string Name => App.GetLocalizedString("DriverSkill");

	    public override string ShortDescription => App.GetLocalizedString("DriverSkillShortDescription");

	    public override CrewRoleType CrewRole => CrewRoleType.Driver;

	    public override string Description => App.GetLocalizedString("DriverSkillDescription");

	    public override string Icon => "driver.png";

	    public override string[] EffectiveDomains => new[] { SkillDomain};

	    protected override void Execute(ModificationContext context, double level)
        {
            var terrainResistanceFactor = CrewBasicSkill.GetDecrementalSkillFactor(level);
            context.SetValue(this.EffectiveDomains[0], TerrainResistanceFactorSkillKey, terrainResistanceFactor);
        }
    }
}
