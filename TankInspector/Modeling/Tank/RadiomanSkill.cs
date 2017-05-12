namespace Smellyriver.TankInspector.Modeling
{
	internal class RadiomanSkill : CrewBasicSkill
    {
        public const string SkillDomain = "crewSkill:radioman";
        public const string SignalRangeFactorSkillKey = "signalRangeFactor";

        public override string Name => App.GetLocalizedString("Radioman");

	    public override string ShortDescription => App.GetLocalizedString("RadiomanSkillShortDescription");
	    public override CrewRoleType CrewRole => CrewRoleType.Radioman;

	    public override string Description => App.GetLocalizedString("RadiomanSkillDescription");

	    public override string Icon => "radioman.png";

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    protected override void Execute(ModificationContext context, double level)
        {
            var signalRangeFactor = CrewBasicSkill.GetIncrementalSkillFactor(level);
            context.SetValue(this.EffectiveDomains[0], SignalRangeFactorSkillKey, signalRangeFactor);
        }
    }
}
