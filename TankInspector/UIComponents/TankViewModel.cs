using Smellyriver.TankInspector.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace Smellyriver.TankInspector.UIComponents
{
	internal partial class TankViewModel : TankViewModelBase, ICloneable, IDatabaseObject
    {

        public override string IconKey => this.Tank.HyphenFullKey;

	    public override string NationKey => this.Tank.Nation.Key;

	    public Database Database => this.Tank.Database;

	    public override bool HasPredecessor => this.Tank.Predecessors.Any();

	    public override IEnumerable<TankViewModelBase> Predecessors
        {
            get
            {
                return this.Tank.Predecessors.Select(info => new TankViewModel(this.CommandBindings, info.Target, this.Owner, true, false, info.ExperiencePrice));
            }
        }

        public override bool HasSuccessor => this.Tank.Successors.Any();

	    public override IEnumerable<TankViewModelBase> Successors
        {
            get
            {
                return this.Tank.Successors.Select(info => new TankViewModel(this.CommandBindings, info.Target, this.Owner, false, true, info.ExperiencePrice));
            }
        }

        // these properties are utilized in the predecessor/successor tank tip
        public bool IsPredecessor { get; }
        public bool IsSuccessor { get; }
        public int UnlockExperiencePrice { get; }

        private DoubleCollection _regularArmorValues;
        public DoubleCollection RegularArmorValues
        {
            get => _regularArmorValues;
	        private set
            {
                _regularArmorValues = value;
                this.RaisePropertyChanged(() => this.RegularArmorValues);
            }
        }

        private DoubleCollection _spacingArmorValues;
        public DoubleCollection SpacingArmorValues
        {
            get => _spacingArmorValues;
	        private set
            {
                _spacingArmorValues = value;
                this.RaisePropertyChanged(() => this.SpacingArmorValues);
            }
        }

        public new Tank Tank => (Tank)base.Tank;

	    public TankViewModel(CommandBindingCollection commandBindings, Tank tank, HangarViewModel owner)
            : base(commandBindings, tank, owner)
        {

        }

        private TankViewModel(CommandBindingCollection commandBindings, Tank tank, HangarViewModel owner, bool isPredecessor, bool isSuccessor, int unlockExperiencePrice)
            : this(commandBindings, tank, owner)
        {
            this.IsPredecessor = isPredecessor;
            this.IsSuccessor = isSuccessor;
            this.UnlockExperiencePrice = unlockExperiencePrice;
        }

        public double GetArmorValue(string key)
        {
			foreach (var armorProvider in this.LoadedModules.Select(m => m.Module).OfType<IHasArmor>())
				if (armorProvider.Armor.TryGetArmorValue(key, out ArmorGroup armor))
					return armor.Value;

			return 0.0;
        }

        public bool GetIsSpacingArmor(string key)
        {
			foreach (var armorProvider in this.LoadedModules.Select(m => m.Module).OfType<IHasArmor>())
				if (armorProvider.Armor.TryGetArmorValue(key, out ArmorGroup armor))
					return armor.VehicleDamageFactor == 0;

			return false;
        }

        private void UpdateArmorValues()
        {
            var regularArmorValues = new DoubleCollection();
            var spacingArmorValues = new DoubleCollection();

            spacingArmorValues.Add(0.0);    // surveying device

            foreach (var armorProvider in this.LoadedModules.Where(m => m != null).Select(m => m.Module).OfType<IHasArmor>())
                foreach (var armor in armorProvider.Armor.ArmorGroups.Values)
                {
                    if (armor.VehicleDamageFactor > 0)
                        regularArmorValues.Add(armor.Value);
                    else
                        spacingArmorValues.Add(armor.Value);
                }

            regularArmorValues.Freeze();
            spacingArmorValues.Freeze();

            this.RegularArmorValues = regularArmorValues;
            this.SpacingArmorValues = spacingArmorValues;
        }



        protected override void OnModulesChanged()
        {
            base.OnModulesChanged(); this.UpdateArmorValues();
        }



    }
}
