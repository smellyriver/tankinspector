namespace Smellyriver.TankInspector.Modeling
{
	internal class CommanderSkill : CrewBasicSkill
    {
        public const string ViewRangeFactorSkillKey = "viewRangeFactor";
        public const string CrewTrainingLevelBuffSkillKey = "crewTrainingLevelBuff";

        public const string SkillDomain = "crewSkill:commander";

        public override string Name => App.GetLocalizedString("CommanderSkill");

	    public override string ShortDescription => App.GetLocalizedString("CommanderSkillShortDescription");

	    public override CrewRoleType CrewRole => CrewRoleType.Commander;

	    public override string Description => App.GetLocalizedString("CommanderSkillDescription");

	    public override string Icon => "commander.png";

	    public override int Priority => -10;

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    protected override void Execute(ModificationContext context, double level)
        {
            var viewRangeFactor = CrewBasicSkill.GetIncrementalSkillFactor(level);
            context.SetValue(this.EffectiveDomains[0], ViewRangeFactorSkillKey, viewRangeFactor);
            var crewTrainingLevelBuff = level / 10;
            context.SetValue(this.EffectiveDomains[0], CrewTrainingLevelBuffSkillKey, crewTrainingLevelBuff);
        }
    }
}
