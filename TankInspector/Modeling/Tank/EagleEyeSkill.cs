namespace Smellyriver.TankInspector.Modeling
{
	internal class EagleEyeSkill : CrewSkill
    {
        
        public const string SkillDomain = "crewSkill:eagleEye";
        public const string DistanceFactorPerLevelWhenDeviceWorkingSkillKey = "distanceFactorPerLevelWhenDeviceWorking";
        public const string DistanceFactorPerLevelWhenDeviceDestroyedSkillKey = "distanceFactorPerLevelWhenDeviceDestroyed";

        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;

	    protected override CrewSkillType Type => CrewSkillType.Skill;

	    public override CrewRoleType CrewRole => CrewRoleType.Commander;

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    public EagleEyeSkill(Database database)
            : base(database)
        {

        }

        protected override void Execute(ModificationContext context, double level)
        {
            context.SetValue(this.EffectiveDomains[0], DistanceFactorPerLevelWhenDeviceWorkingSkillKey, level * this.Parameters["distanceFactorPerLevelWhenDeviceWorking"]);
            context.SetValue(this.EffectiveDomains[0], DistanceFactorPerLevelWhenDeviceDestroyedSkillKey, level * this.Parameters["distanceFactorPerLevelWhenDeviceDestroyed"]);
        }

        protected override void Clear(ModificationContext context)
        {
            context.SetValue(this.EffectiveDomains[0], DistanceFactorPerLevelWhenDeviceWorkingSkillKey, 0);
            context.SetValue(this.EffectiveDomains[0], DistanceFactorPerLevelWhenDeviceDestroyedSkillKey, 0);
        }
    }
}
