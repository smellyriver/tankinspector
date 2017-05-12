namespace Smellyriver.TankInspector.Modeling
{
	internal class SniperSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:sniper";
        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;

	    protected override CrewSkillType Type => CrewSkillType.Perk;

	    public override CrewRoleType CrewRole => CrewRoleType.Gunner;

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    public SniperSkill(Database database)
            : base(database)
        {

        }
        protected override void Execute(ModificationContext context, double level)
        {
            //todo
        }

        protected override void Clear(ModificationContext context)
        {
         
        }
    }
}
