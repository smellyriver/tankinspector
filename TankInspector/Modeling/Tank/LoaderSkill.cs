namespace Smellyriver.TankInspector.Modeling
{
	internal class LoaderSkill : CrewBasicSkill
    {
        public const string SkillDomain = "crewSkill:loader";
        public const string LoadTimeFactorSkillKey = "loadTimeFactor";

        public override string Name => App.GetLocalizedString("Loader");

	    public override string ShortDescription => App.GetLocalizedString("LoaderSkillShortDescription");

	    public override CrewRoleType CrewRole => CrewRoleType.Loader;

	    public override string Description => App.GetLocalizedString("LoaderSkillDescription");

	    public override string Icon => "loader.png";

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    protected override void Execute(ModificationContext context, double level)
        {
            var loadTimeFactor = CrewBasicSkill.GetDecrementalSkillFactor(level);
            context.SetValue(this.EffectiveDomains[0], LoadTimeFactorSkillKey, loadTimeFactor);
        }
    }
}
