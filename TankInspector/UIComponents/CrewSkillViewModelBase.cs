using Smellyriver.TankInspector.Modeling;
using System;
using System.Collections.Generic;

namespace Smellyriver.TankInspector.UIComponents
{
	internal abstract class CrewSkillViewModelBase : NotificationObject, IDetailedDataRelatedComponent, IPreviewable
    {
        public CrewSkillBase CrewSkill { get; }
        private bool _isLearnt;
        public virtual bool IsLearnt
        {
            get => _isLearnt;
	        set
            {
                if (!value && !this.CanForget)
                    throw new InvalidOperationException("this skill cannot be forgotten");

                _isLearnt = value;
                this.RaisePropertyChanged(() => this.IsLearnt);
            }
        }

        public bool IsPreviewing { get; set; }
        public abstract bool CanForget { get; }
        
        public abstract string BigIcon { get; }
        public abstract string SmallIcon { get; }


        public DetailedDataRelatedComponentType ComponentType => DetailedDataRelatedComponentType.CrewSkill;

	    public IEnumerable<string> ModificationDomains => this.CrewSkill.EffectiveDomains;

	    public CrewViewModel Owner { get; }

        public CrewSkillViewModelBase(CrewSkillBase skill, CrewViewModel owner)
        {
            this.CrewSkill = skill;
            this.Owner = owner;
        }

    }
}
