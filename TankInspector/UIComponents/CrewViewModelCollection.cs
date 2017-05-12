using Smellyriver.TankInspector.Modeling;
using Smellyriver.Wpf.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class CrewViewModelCollection : Collection<CrewViewModel>, IDetailedDataRelatedComponent, INotifyPropertyChanged
    {
        public DetailedDataRelatedComponentType ComponentType => DetailedDataRelatedComponentType.Crew;

	    public IEnumerable<string> ModificationDomains
        {
            get { return this.SelectMany(c => c.BasicSkills).SelectMany(s => s.CrewSkill.EffectiveDomains); }
        }

        public bool IsCamouflageLearntByAllCrews
        {
            get { return this.All(c => c.Skills.Any(s => s.CrewSkill is CamouflageSkill)); }
        }

        public bool IsBiALearntByAllCrews
        {
            get { return this.All(c => c.Skills.Any(s => s.CrewSkill is BrotherhoodSkill)); }
        }

        public ICommand LearnCamouflageForAllCrewsCommand { get; }
        public ICommand LearnBiAForAllCrewsCommand { get; }

        private readonly CommandBindingCollection _commandBindings;

        public event PropertyChangedEventHandler PropertyChanged;

        public CrewViewModelCollection(CommandBindingCollection commandBindings)
        {
            _commandBindings = commandBindings;
            this.LearnCamouflageForAllCrewsCommand = Command.FromAction(commandBindings, LearnCamouflageForAllCrews);
            this.LearnBiAForAllCrewsCommand = Command.FromAction(commandBindings, LearnBiAForAllCrews);
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected override void ClearItems()
        {
            foreach (var crew in this)
                this.UnhandleCrewEvents(crew);
            base.ClearItems();
        }

        protected override void InsertItem(int index, CrewViewModel item)
        {
            base.InsertItem(index, item);
            this.HandleCrewEvents(item);
        }

        protected override void RemoveItem(int index)
        {
            this.UnhandleCrewEvents(this[index]);
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, CrewViewModel item)
        {
            this.UnhandleCrewEvents(this[index]);
            base.SetItem(index, item);
            this.HandleCrewEvents(item);
        }

        private void HandleCrewEvents(CrewViewModel crew)
        {
            crew.Skills.CollectionChanged += CrewSkills_CollectionChanged;
        }

	    private void CrewSkills_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RaisePropertyChanged("IsCamouflageLearntByAllCrews");
            this.RaisePropertyChanged("IsBiALearntByAllCrews");
        }

        private void UnhandleCrewEvents(CrewViewModel crew)
        {
            crew.Skills.CollectionChanged -= CrewSkills_CollectionChanged;
        }

        private void LearnBiAForAllCrews()
        {
            foreach(var crew in this)
                crew.Learn<BrotherhoodSkill>();
        }

        private void LearnCamouflageForAllCrews()
        {
            foreach (var crew in this)
                crew.Learn<CamouflageSkill>();
        }

        public CrewViewModelCollection Clone(TankViewModelBase owner, TankViewModelCloneFlags flags)
        {
            var clone = new CrewViewModelCollection(_commandBindings);
            foreach (var crew in this)
            {
                var deepCopy = false;
                foreach (var role in crew.Crew.AllRoles)
                {
                    switch (role)
                    {
                        case CrewRoleType.Commander:
                            deepCopy = (flags & TankViewModelCloneFlags.Commander) == TankViewModelCloneFlags.Commander;
                            break;
                        case CrewRoleType.Driver:
                            deepCopy = (flags & TankViewModelCloneFlags.Driver) == TankViewModelCloneFlags.Driver;
                            break;
                        case CrewRoleType.Gunner:
                            deepCopy = (flags & TankViewModelCloneFlags.Gunner) == TankViewModelCloneFlags.Gunner;
                            break;
                        case CrewRoleType.Loader:
                            deepCopy = (flags & TankViewModelCloneFlags.Loader) == TankViewModelCloneFlags.Loader;
                            break;
                        case CrewRoleType.Radioman:
                            deepCopy = (flags & TankViewModelCloneFlags.Radioman) == TankViewModelCloneFlags.Radioman;
                            break;
                    }

                    if (deepCopy)
                        break;
                }

                if (deepCopy)
                    clone.Add(crew.Clone(owner, crew.Index));
                else
                    clone.Add(crew);
            }
            return clone;
        }

    }
}
