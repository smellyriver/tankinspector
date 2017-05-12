using Smellyriver.TankInspector.Modeling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class CrewViewModel : NotificationObject, IDetailedDataRelatedComponent, IPreviewable
    {

        private static readonly double SFirstSkillExperience;

        static CrewViewModel()
        {
            SFirstSkillExperience = CrewViewModel.GetLevelUpExperience(100);
        }

        private static double GetLevelUpExperience(int level)
        {
            var experience = 0.0;
            for (int i = 0; i < level; ++i)
                experience += 50 * Math.Pow(100, i * 0.01);

            return experience;
        }

        bool IPreviewable.IsPreviewing { get; set; }

        public DetailedDataRelatedComponentType ComponentType => DetailedDataRelatedComponentType.Crew;

	    public IEnumerable<string> ModificationDomains { get; }

        public Crew Crew { get; }

        public int Index { get; }

        public CrewRole PrimaryRole { get; }
        public IEnumerable<CrewRole> SecondaryRole { get; }

        public event EventHandler EffectiveSkillsChanged;

        public string NationKey => this.Owner.Tank.NationKey;

	    private int _lastSkillTrainingLevel;
        public int LastSkillTrainingLevel
        {
            get => _lastSkillTrainingLevel;
	        set
            {
                _lastSkillTrainingLevel = value;
                this.RaisePropertyChanged(() => this.LastSkillTrainingLevel);
                this.UpdateSkills();
            }
        }

        private int DeadCrewCount
        {
            get { return this.Owner.Crews.Count(c => c.IsDead); }
        }

        private double JackOfAllTradesProvidedTrainingLevel
        {
            get
            {
                var deadCount = this.DeadCrewCount;
                if (deadCount == 0)
                    return 0;

                var trainingLevel = this.Owner.ModificationContext.GetValue(UniversalistSkill.SkillDomain, UniversalistSkill.EfficiencySkillKey, 0.0);
                return trainingLevel / deadCount;
            }
        }

        public double ActualBasicTrainingLevel
        {
            get
            {
	            if (this.IsDead)
                    return this.JackOfAllTradesProvidedTrainingLevel;
	            return (this.Skills.Count > 0 ? 100 : this.LastSkillTrainingLevel) + this.BuffTrainingLevel;
            }
        }

        public double ActualLearntSkillTrainingLevel
        {
            get
            {
	            if (this.IsDead)
                    return 0;
	            return 100 + this.BuffTrainingLevel;
            }
        }

        public double ActualLastSkillTrainingLevel
        {
            get
            {
	            if (this.IsDead)
                    return 0;
	            return this.LastSkillTrainingLevel + this.BuffTrainingLevel;
            }
        }


        public int MaxLastSkillTrainingLevel => 100;


	    private double _buffTrainingLevel;
        public double BuffTrainingLevel
        {
            get
            {
                var context = this.Owner.ModificationContext;
                double commanderBuff;
                if (this.PrimaryRole.Type == CrewRoleType.Commander)
                    commanderBuff = 0;
                else
                    commanderBuff = context.GetValue(CommanderSkill.SkillDomain, CommanderSkill.CrewTrainingLevelBuffSkillKey, 0.0);
                var brotherhoodValue = context.GetValue(BrotherhoodSkill.SkillDomain, BrotherhoodSkill.CrewTrainingLevelIncrementSkillKey, 0.0);
                var ventValue = context.GetValue("staticAdditiveDevice:miscAttrs/crewLevelIncrease", "miscAttrs/crewLevelIncrease", 0.0);
                var stimulatorValue = context.GetValue("stimulator", "crewLevelIncrease", 0.0);
                var result = commanderBuff + brotherhoodValue + ventValue + stimulatorValue;
                var changed = _buffTrainingLevel != result;
                _buffTrainingLevel = result;

                if (changed)
                    this.RaisePropertyChanged(() => this.BuffTrainingLevel);

                return result;
            }
        }


        private bool _isDead;
        public bool IsDead
        {
            get => _isDead;
	        set
            {
                _isDead = value;
                this.RaisePropertyChanged(() => this.IsDead);
                this.UpdateSkills();
            }
        }

        public ObservableCollection<CrewBasicSkillViewModel> BasicSkills { get; }

        public Dictionary<CrewSkill, CrewSkillViewModel> AvailableSkills { get; }

        public ObservableCollection<CrewSkillViewModel> Skills { get; }

        public ObservableCollection<CrewSkillViewModelBase> AllLearntSkills { get; }

        private IEnumerable<CrewSkillViewModelBase> _lastSkillTrainingLevelAffectedSkills;
        public IEnumerable<CrewSkillViewModelBase> LastSkillTrainingLevelAffectedSkills
        {
            get => _lastSkillTrainingLevelAffectedSkills;
	        private set
            {
                _lastSkillTrainingLevelAffectedSkills = value;
                this.RaisePropertyChanged(() => this.LastSkillTrainingLevelAffectedSkills);
            }
        }

        private string _lastSkillTrainingLevelAffectedSkillName;
        public string LastSkillTrainingLevelAffectedSkillName
        {
            get => _lastSkillTrainingLevelAffectedSkillName;
	        private set
            {
                _lastSkillTrainingLevelAffectedSkillName = value;
                this.RaisePropertyChanged(() => this.LastSkillTrainingLevelAffectedSkillName);
            }
        }



        public int RankLevel => ((int)(this.LastSkillTrainingLevel / 50) - 1 + this.Skills.Count * 2 + this.MinRankLevel).Clamp(this.MinRankLevel, this.MaxRankLevel);

	    private int MinRankLevel
        {
            get
            {
                switch (this.Crew.PrimaryRole)
                {
                    case CrewRoleType.Radioman:
                    case CrewRoleType.Loader:
                        return 1;
                    case CrewRoleType.Driver:
                    case CrewRoleType.Gunner:
                        return 2;
                    case CrewRoleType.Commander:
                        return 3;
                    default:
                        return 1;
                }
            }
        }

        private int MaxRankLevel
        {
            get
            {
                switch (this.Crew.PrimaryRole)
                {
                    case CrewRoleType.Radioman:
                    case CrewRoleType.Loader:
                        return 9;
                    case CrewRoleType.Driver:
                    case CrewRoleType.Gunner:
                        return 10;
                    case CrewRoleType.Commander:
                        return 11;
                    default:
                        return 9;
                }
            }
        }

        public long Experience
        {
            get
            {
                var experience = SFirstSkillExperience * (Math.Pow(2, Skills.Count) - 1);
                experience += Math.Pow(2, Skills.Count) * CrewViewModel.GetLevelUpExperience(this.LastSkillTrainingLevel);
                experience = Math.Max(experience - 9548, 0); // exclude first 50% proficiency
                return (long)experience;
            }
        }

        public TankViewModelBase Owner { get; }

        private readonly CommandBindingCollection _commandBindings;

        private bool _isUpdating;

        public CrewViewModel(CommandBindingCollection commandBindings, Crew crew, TankViewModelBase owner, int index)
        {
            this.Owner = owner;
            this.Crew = crew;
            this.Index = index;
            this.Skills = new ObservableCollection<CrewSkillViewModel>();
            this.BasicSkills = new ObservableCollection<CrewBasicSkillViewModel>();
            this.AllLearntSkills = new ObservableCollection<CrewSkillViewModelBase>();

            _commandBindings = commandBindings;

            var crewDb = this.Owner.Tank.Database.CrewDatabase;

            this.PrimaryRole = crewDb.CrewRoles[this.Crew.PrimaryRole];
            this.SecondaryRole = this.Crew.SecondaryRoles.Select(r => crewDb.CrewRoles[r]).ToArray();

            var availableSkills = new HashSet<CrewSkill>();

            foreach (var skill in crewDb.RoleSkills[CrewRoleType.All])
                if (skill.IsEnabled)
                    availableSkills.Add(skill);

            foreach (var role in this.Crew.AllRoles)
            {
                this.BasicSkills.Add(new CrewBasicSkillViewModel(CrewBasicSkill.Create(role), this));
                foreach (var skill in crewDb.RoleSkills[role])
                    if (skill.IsEnabled)
                        availableSkills.Add(skill);
            }

            foreach (var skill in this.BasicSkills)
                AllLearntSkills.Add(skill);

            this.AvailableSkills = new Dictionary<CrewSkill, CrewSkillViewModel>();
            foreach (var skill in availableSkills)
            {
                var vm = new CrewSkillViewModel(_commandBindings, skill, this);
                vm.PropertyChanged += CrewSkillViewModel_PropertyChanged;
                this.AvailableSkills.Add(skill, vm);
            }

            this.ModificationDomains = this.BasicSkills.SelectMany(s => s.CrewSkill.EffectiveDomains).ToArray();

            this.LastSkillTrainingLevel = 50;
        }

	    private void CrewSkillViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsLearnt")
            {
                var skillVm = (CrewSkillViewModel)sender;
                if (skillVm.IsLearnt && !this.Skills.Contains(skillVm))
                {
                    this.Skills.Add(skillVm);
                    this.AllLearntSkills.Add(skillVm);
                }
                else if (!skillVm.IsLearnt && this.Skills.Contains(skillVm))
                {
                    this.Skills.Remove(skillVm);
                    this.AllLearntSkills.Remove(skillVm);
                }

                this.UpdateSkills();
            }
        }

        private void UpdateSkills()
        {
            if (_isUpdating)
                return;

            _isUpdating = true;
            if (this.Skills.Count > 0)
            {
                var lastSkill = this.Skills[this.Skills.Count - 1];

                this.LastSkillTrainingLevelAffectedSkills = new[] { lastSkill };
                this.LastSkillTrainingLevelAffectedSkillName = lastSkill.CrewSkill.Name;
            }
            else
            {
                this.LastSkillTrainingLevelAffectedSkills = this.BasicSkills;
                if (this.BasicSkills.Count == 1)
                    this.LastSkillTrainingLevelAffectedSkillName = App.GetLocalizedString("BasicSkill");
                else
                    this.LastSkillTrainingLevelAffectedSkillName = App.GetLocalizedString("BasicSkills");
            }

            this.RaisePropertyChanged(() => this.ActualBasicTrainingLevel);
            this.RaisePropertyChanged(() => this.ActualLearntSkillTrainingLevel);
            this.RaisePropertyChanged(() => this.ActualLastSkillTrainingLevel);
            this.RaisePropertyChanged(() => this.Experience);
            this.RaisePropertyChanged(() => this.RankLevel);

            if (this.EffectiveSkillsChanged != null)
                this.EffectiveSkillsChanged(this, new EventArgs());
            _isUpdating = false;
        }



        internal double GetSkillLevel(CrewSkillViewModelBase skill)
        {
	        if (skill is CrewBasicSkillViewModel)
                return this.ActualBasicTrainingLevel;
	        if (skill == this.Skills.LastOrDefault())
		        return this.ActualLastSkillTrainingLevel;
	        if (this.Skills.Contains(skill))
		        return this.ActualLearntSkillTrainingLevel;
	        return 0.0;
        }

        internal void Learn<TSkillType>()
            where TSkillType : CrewSkill
        {
            foreach (var skill in this.AvailableSkills)
                if (skill.Key is TSkillType)
                    skill.Value.IsLearnt = true;
        }

        internal void Forget<TSkillType>()
            where TSkillType : CrewSkill
        {
            foreach (var skill in this.AvailableSkills)
                if (skill.Key is TSkillType)
                    skill.Value.IsLearnt = false;
        }


        public CrewViewModel Clone(TankViewModelBase owner, int index)
        {
            var clone = new CrewViewModel(_commandBindings, this.Crew, owner, index);
            clone._isUpdating = true;

            clone._isDead = this._isDead;
            clone._lastSkillTrainingLevel = this._lastSkillTrainingLevel;

            foreach (var skill in this.Skills)
            {
                var targetSkill = clone.AvailableSkills[skill.CrewSkill];
                clone.Skills.Add(targetSkill);
                clone.AllLearntSkills.Add(targetSkill);
            }

            clone._isUpdating = false;
            clone.UpdateSkills();

            return clone;
        }

        public void CopyFrom(CrewViewModel targetCrew)
        {
            _isUpdating = true;
            _isDead = targetCrew.IsDead;
            _lastSkillTrainingLevel = targetCrew._lastSkillTrainingLevel;

            foreach (var skillItem in this.AvailableSkills)
            {
				if (targetCrew.AvailableSkills.TryGetValue(skillItem.Key, out CrewSkillViewModel targetSkill))
					skillItem.Value.IsLearnt = targetSkill.IsLearnt;
			}

            _isUpdating = false;
            this.UpdateSkills();
        }
    }
}
