using System;

namespace Smellyriver.TankInspector.Modeling
{
	internal abstract class CrewBasicSkill : CrewSkillBase
    {
        
        public static CrewBasicSkill Create(CrewRoleType role)
        {
            switch(role)
            {
                case CrewRoleType.Commander:
                    return new CommanderSkill();
                case CrewRoleType.Driver:
                    return new DriverSkill();
                case CrewRoleType.Gunner:
                    return new GunnerSkill();
                case CrewRoleType.Loader:
                    return new LoaderSkill();
                case CrewRoleType.Radioman:
                    return new RadiomanSkill();
                default:
                    throw new NotSupportedException();
            }
        }

        protected override CrewSkillType Type => CrewSkillType.Skill;

	    internal static double GetIncrementalSkillFactor(double level)
        {
            return (0.00375 * level + 0.5) / 0.875;
        }

        internal static double GetDecrementalSkillFactor(double level)
        {
            return 0.875 / (0.00375 * level + 0.5);
        }
        
        public override DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy => DuplicatedCrewSkillPolicy.Average;

	    protected override void Clear(ModificationContext context)
        {

        }
    }
}
