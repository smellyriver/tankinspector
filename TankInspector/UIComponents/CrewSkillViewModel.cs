using Smellyriver.TankInspector.Modeling;
using Smellyriver.Wpf.Input;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class CrewSkillViewModel : CrewSkillViewModelBase, IDatabaseObject
    {
        public new CrewSkill CrewSkill => (CrewSkill)base.CrewSkill;

	    public Database Database => this.CrewSkill.Database;

	    public override bool CanForget => true;

	    public override string BigIcon => "gui/maps/icons/tankmen/skills/big/" + this.CrewSkill.Icon;

	    public override string SmallIcon => "gui/maps/icons/tankmen/skills/small/" + this.CrewSkill.Icon;

	    public ICommand ToggleCommand { get; }

        //private CommandBindingCollection _commandBindings;
        //private CrewViewModel _owner;

        public CrewSkillViewModel(CommandBindingCollection commandBindings, CrewSkill skill, CrewViewModel owner)
            : base(skill, owner)
        {
            this.ToggleCommand = Command.FromAction(commandBindings, this.ToggleLearnt);
        }

        private void ToggleLearnt()
        {
            this.IsLearnt = !this.IsLearnt;
        }


    }
}
