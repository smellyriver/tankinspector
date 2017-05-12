namespace Smellyriver.TankInspector.Modeling
{
	internal class RancorousSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:rancorous";
        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;

	    protected override CrewSkillType Type => CrewSkillType.Perk;
	    public override CrewRoleType CrewRole => CrewRoleType.Gunner;

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    public RancorousSkill(Database database)
            : base(database)
        {

        }
        protected override void Execute(ModificationContext context, double level)
        {
            // dummy
        }

        protected override void Clear(ModificationContext context)
        {
         
        }
    }
}
