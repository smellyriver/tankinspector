namespace Smellyriver.TankInspector.Modeling
{
	internal class BadRoadsKingSkill : CrewSkill
    {
        public const string SkillDomain = "crewSkill:badRoadsKing";

        public const string SoftGroundResistanceFactorDecrementSkillKey = "softGroundResistanceFactorDecrement";
        public const string MediumGroundResistanceFactorDecrementSkillKey = "mediumGroundResistanceFactorDecrement";


        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Highest;
	    protected override CrewSkillType Type => CrewSkillType.Skill;

	    public override CrewRoleType CrewRole => CrewRoleType.Driver;

	    public override string[] EffectiveDomains => new[] { SkillDomain };

	    public BadRoadsKingSkill(Database database)
            : base(database)
        {

        }
        protected override void Execute(ModificationContext context, double level)
        {
            context.SetValue(this.EffectiveDomains[0], SoftGroundResistanceFactorDecrementSkillKey, this.Parameters["softGroundResistanceFactorPerLevel"] * level);
            context.SetValue(this.EffectiveDomains[0], MediumGroundResistanceFactorDecrementSkillKey, this.Parameters["mediumGroundResistanceFactorPerLevel"] * level);
        }

        protected override void Clear(ModificationContext context)
        {
            context.SetValue(this.EffectiveDomains[0], SoftGroundResistanceFactorDecrementSkillKey, 0);
            context.SetValue(this.EffectiveDomains[0], MediumGroundResistanceFactorDecrementSkillKey, 0);
        }
    }
}

