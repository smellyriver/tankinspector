using Smellyriver.Collections;
using Smellyriver.TankInspector.Modeling;
using Smellyriver.Wpf.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class TankGalleryViewModel : MainWindowOverlayViewModel
    {

        public override bool IsFullWindow => true;

	    private KeyValuePair<string, TankGalleryPageViewModel> _selectedCategory;
        public KeyValuePair<string, TankGalleryPageViewModel> SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                if (!Equals(_selectedCategory, value))
                {
                    _selectedCategory = value;
                    this.RaisePropertyChanged(() => this.SelectedCategory);

                    ApplicationSettings.Default.PreviousTankGalleryPageKey = _selectedCategory.Key;
                    ApplicationSettings.Default.Save();
                }
            }
        }

        public bool CanCloseTankGallery => this.Owner.Hangar.Tank != null;

	    public ObservableDictionary<string, TankGalleryPageViewModel> Categories { get; }

        private readonly CommandBindingCollection _commandBindings;

        public ICommand CloseTankGalleryCommand { get; }

        public TankGalleryViewModel(CommandBindingCollection commandBindings, MainWindowViewModel owner)
            : base(owner)
        {
            _commandBindings = commandBindings;

            this.CloseTankGalleryCommand = new RelayCommand(this.Hide);

            this.Categories = new ObservableDictionary<string, TankGalleryPageViewModel>();

            this.UpdateCategories();
        }

        private void UpdateCategories()
        {
            this.Categories.Clear();

            foreach (var nationItem in Database.Current.Nations)
            {
                var nationVM = new NationalTechTreePageViewModel(_commandBindings, this, nationItem.Value.TechTree.Layout, nationItem.Value);
                this.Categories.Add(nationItem.Key, nationVM);
            }

            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    var categoryKey = ApplicationSettings.Default.PreviousTankGalleryPageKey;
                    if (!this.Categories.ContainsKey(categoryKey))
                        categoryKey = this.Categories.Keys.First();
                    this.SelectedCategory = new KeyValuePair<string, TankGalleryPageViewModel>(categoryKey, this.Categories[categoryKey]);
                }), DispatcherPriority.Background);
        }

        internal void LoadTank(Tank tank)
        {
            this.Owner.Hangar.LoadTank(tank);
            this.Hide();
        }

        public override void Show()
        {
            base.Show();
            this.RaisePropertyChanged(() => this.CanCloseTankGallery);
        }

    }
}
