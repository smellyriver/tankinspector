using Smellyriver.TankInspector.Modeling;
using System.Data;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class CrewBasicSkillViewModel : CrewSkillViewModelBase
    {

        public new CrewBasicSkill CrewSkill => (CrewBasicSkill)base.CrewSkill;

	    public override bool IsLearnt
        {
            get => true;
	        set => throw new ReadOnlyException();
        }

        public override bool CanForget => false;

	    public override string BigIcon => "gui/maps/icons/tankmen/roles/big/" + this.CrewSkill.Icon;

	    public override string SmallIcon => "gui/maps/icons/tankmen/roles/small/" + this.CrewSkill.Icon;

	    public CrewBasicSkillViewModel(CrewBasicSkill skill, CrewViewModel owner)
            : base(skill, owner)
        {

        }
    }
}
