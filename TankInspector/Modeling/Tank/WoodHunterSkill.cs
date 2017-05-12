namespace Smellyriver.TankInspector.Modeling
{
	internal class WoodHunterSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:woodHunter";
        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;

	    protected override CrewSkillType Type => CrewSkillType.Perk;

	    public override CrewRoleType CrewRole => CrewRoleType.Gunner;
	    public override string[] EffectiveDomains => new[] { SkillDomain };
	    public override bool IsEnabled => false;

	    public WoodHunterSkill(Database database)
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
