using log4net;
using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Modeling;
using Smellyriver.TankInspector.UIComponents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Smellyriver.Utilities;
using Smellyriver.Wpf.Input;

namespace Smellyriver.TankInspector
{
	internal class MainWindowViewModel : NotificationObject
    {
        private static readonly ILog Log = SafeLog.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

        private class AddDatabaseToken
        {
            public static readonly AddDatabaseToken Instance = new AddDatabaseToken();

            public override string ToString()
            {
                return App.GetLocalizedString("AddDatabase");
            }
        }

        private HangarViewModel _hangar;
        public HangarViewModel Hangar
        {
            get => _hangar;
	        private set
            {
                _hangar = value;
                this.RaisePropertyChanged(() => this.Hangar);
            }
        }

        private TankGalleryViewModel _tankGallery;
        public TankGalleryViewModel TankGallery
        {
            get => _tankGallery;
	        private set
            {
                _tankGallery = value;
                this.RaisePropertyChanged(() => this.TankGallery);
            }
        }

        public MessagesWindowViewModel Messages { get; }

        private bool _isWindowActive;
        public bool IsWindowActive
        {
            get => _isWindowActive;
	        set
            {
                _isWindowActive = value;
                this.UpdateRenderActivityLevel();
            }
        }
        
        public LoadingViewModel LoadingViewModel { get; private set; }

        public CreditsViewModel CreditsViewModel { get; }

        public SettingsViewModel SettingsViewModel { get; }

        private readonly List<MainWindowOverlayViewModel> _popupViewModels;

        public IEnumerable<object> DatabaseVersions => Database.AvailableDatabaseKeys.Cast<object>();

	    public object ActiveDatabaseVersion
        {
            get => Database.CurrentDatabaseKey;
		    set
            {
                if (value == AddDatabaseToken.Instance)
                {
                    var newVersion = Database.PromptAddDatabase();
                    if (newVersion != null)
                    {
                        this.RaisePropertyChanged(() => this.DatabaseVersions);
                        this.ActiveDatabaseVersion = newVersion;
                    }
                    else
                    {
                        this.ActiveDatabaseVersion = this.ActiveDatabaseVersion;
                    }
                }
                else
                {
                    Database.CurrentDatabaseKey = (DatabaseKey)value;
                }

                this.RaisePropertyChanged(() => this.ActiveDatabaseVersion);
            }
        }

        public ImageSource BackgroundImage => LoadingImage.Random(this.IsLoading ? ApplicationSettings.Default.GamePathes[0] : null);

	    private int _unreadMessageCount;
        public int UnreadMessageCount
        {
            get => _unreadMessageCount;
	        set
            {
                _unreadMessageCount = value;
                this.RaisePropertyChanged(() => this.UnreadMessageCount);
                this.RaisePropertyChanged(() => this.MessageLinkButtonCaption);
            }
        }

        public string MessageLinkButtonCaption
        {
            get
            {
	            if (this.UnreadMessageCount == 0)
                    return App.GetLocalizedString("Messages");
	            return string.Format(App.GetLocalizedString("MessagesWithCount"), this.UnreadMessageCount);
            }
        }

        public string DisconnectLinkToolTip => string.Format(App.GetLocalizedString("DisconnectLinkToolTip"), ApplicationSettings.Default.EMailAddress);


	    private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
	        private set
            {
                _isLoading = value;
                this.RaisePropertyChanged(() => this.IsLoading);
            }
        }

        private bool _isLoaded;
        public bool IsLoaded
        {
            get => _isLoaded;
	        private set
            {
                _isLoaded = value;
                this.RaisePropertyChanged(() => this.IsLoaded);
            }
        }

        private bool _isOverlayShown;
        public bool IsOverlayShown
        {
            get => _isOverlayShown;
	        private set
            {
                _isOverlayShown = value;
                this.RaisePropertyChanged(() => this.IsOverlayShown);
                this.RaisePropertyChanged(() => this.ShouldBlurBackground);

                this.UpdateRenderActivityLevel();
            }
        }

        private bool _isFullWindowOverlayShown;
        public bool IsFullWindowOverlayShown
        {
            get => _isFullWindowOverlayShown;
	        private set
            {
                _isFullWindowOverlayShown = value;
                this.RaisePropertyChanged(() => this.IsFullWindowOverlayShown);
                this.RaisePropertyChanged(() => this.ShouldBlurBackground);

                this.UpdateRenderActivityLevel();
            }
        }
        
        public bool ShouldBlurBackground => this.IsOverlayShown && !this.IsFullWindowOverlayShown;


	    private Task _loadingTask;

        public event EventHandler LoadComplete;
		
        internal CommandBindingCollection CommandBindings { get; }

        public ICommand ShowCreditsCommand { get; }
        public ICommand DonateCommand { get; }
		
        public MainWindowViewModel(CommandBindingCollection commandBindings)
        {
            this.CommandBindings = commandBindings;
            _popupViewModels = new List<MainWindowOverlayViewModel>();
			
            this.LoadingViewModel = new LoadingViewModel(commandBindings, this);

            this.CreditsViewModel = new CreditsViewModel(this);
            this.RegisterPopupViewModel(this.CreditsViewModel);

            this.SettingsViewModel = new SettingsViewModel(this);
            this.RegisterPopupViewModel(this.SettingsViewModel);

            this.ShowCreditsCommand = new RelayCommand(this.ShowCredits);
            this.DonateCommand = new RelayCommand(this.NavigateToDonatePage);
            this.Messages = new MessagesWindowViewModel();
        }
        
        private void RegisterPopupViewModel(MainWindowOverlayViewModel vm)
        {
            _popupViewModels.Add(vm);
            vm.VisibilityChanged += this.VM_VisibilityChanged;
        }

	    private void VM_VisibilityChanged(object sender, EventArgs e)
        {
            _isOverlayShown = _popupViewModels.Any(v => v.IsShown);
            if (this.IsOverlayShown)
                this.IsFullWindowOverlayShown = _popupViewModels.Any(v => v.IsFullWindow && v.IsShown);
            else
                _isFullWindowOverlayShown = false;

            this.RaisePropertyChanged(() => this.ShouldBlurBackground);

            this.UpdateRenderActivityLevel();
        }

        private void ShowCredits()
        {
            this.CreditsViewModel.Show();
        }
        
        
        private void NavigateToDonatePage()
        {
            MessageBox.Show(App.GetLocalizedString("DonationTip"), App.GetLocalizedString("DonationTitle"), MessageBoxButton.OK);
            Process.Start("https://www.paypal.com/webapps/shoppingcart?flowlogging_id=4b7de2e4d8256&mfid=1494580630152_4b7de2e4d8256#/checkout/openButton");
        }

        public void BeginLoad()
        {
            this.IsLoading = true;
            var dispatcher = Dispatcher.CurrentDispatcher;

            if (!Database.ValidateGamePathes())
                Application.Current.Shutdown();

            _loadingTask = Task.Factory.StartNew(() => Database.Load(AppState.Default.FirstRun));
            _loadingTask.ContinueWith(task => dispatcher.Invoke(new Action(this.OnDatabaseLoaded)));
        }

        private void OnDatabaseLoaded()
        {
            if (_loadingTask.IsFaulted)
            {
                Log.Fatal("database load failed", _loadingTask.Exception);

                throw new InvalidOperationException("database load failed", _loadingTask.Exception);
            }


            this.IsLoading = false;
            this.LoadingViewModel.LoadingCompleted = true;

            this.ScheduleUnloadLoadingView();

            AppState.Default.FirstRun = false;
            AppState.Default.Save();

            ITank tank = null;
            if (!string.IsNullOrEmpty(ApplicationSettings.Default.PreviousTankKey))
                tank = Database.Current.GetTank(ApplicationSettings.Default.PreviousTankKey);

            if (tank == null)
                tank = MainWindowViewModel.AssignDefaultTank();

            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => this.RaisePropertyChanged(() => this.BackgroundImage)), DispatcherPriority.Background);

            this.RaisePropertyChanged(() => this.DatabaseVersions);
            this.RaisePropertyChanged(() => this.ActiveDatabaseVersion);

            this.Hangar = new HangarViewModel(this.CommandBindings, this);
            this.TankGallery = new TankGalleryViewModel(this.CommandBindings, this);
            this.RegisterPopupViewModel(this.TankGallery);

            if (tank != null)
                this.Hangar.LoadTank((Tank)tank);
            else
                this.TankGallery.Show();

        }

        private static ITank AssignDefaultTank()
        {
            ITank tank = null;
            var currentCulture = CultureInfo.CurrentCulture;
            if (currentCulture.CanFallbackTo("zh-TW"))
                tank = Database.Current.GetTank("china:Ch24_Type64");
            else if (currentCulture.CanFallbackTo("zh"))
                tank = Database.Current.GetTank("china:Ch18_WZ-120");
            else if (currentCulture.CanFallbackTo("de"))
                tank = Database.Current.GetTank("germany:PzVI");
            else if (currentCulture.CanFallbackTo("fr"))
                tank = Database.Current.GetTank("france:AMX_13_90");
            else if (currentCulture.CanFallbackTo("en-GB") || currentCulture.CanFallbackTo("en-AU"))
                tank = Database.Current.GetTank("uk:GB24_Centurion_Mk3");
            else if (currentCulture.CanFallbackTo("en"))
                tank = Database.Current.GetTank("usa:M4_Sherman");
            else if (currentCulture.CanFallbackTo("ja"))
                tank = Database.Current.GetTank("japan:Type_61") ?? Database.Current.GetTank("japan:Chi_Nu_Kai");
            else if (currentCulture.CanFallbackTo("cz"))
                tank  = Database.Current.GetTank("czech:Cz10_LT_vz38");
            else if (currentCulture.CanFallbackTo("po"))
                tank = Database.Current.GetTank("ussr:R117_T34_85_Rudy");
            else if (currentCulture.CanFallbackTo("kr"))
                tank = Database.Current.GetTank("usa:A63_M46_Patton_KR");
            else if (currentCulture.CanFallbackTo("he"))     //israel
                tank = Database.Current.GetTank("france:F73_M4A1_Revalorise");

            if (tank == null)
                tank = Database.Current.GetTank("ussr:R04_T-34");

            if (tank == null)
                tank = Database.Current.Tanks.First();

            return tank;
        }


        private void UpdateRenderActivityLevel()
        {
            if (this.Hangar == null)
                return;

            var activityLevel = RenderActivityLevel.Normal;
            if (this.IsWindowActive)
            {
                if (this.IsOverlayShown)
                    activityLevel = RenderActivityLevel.Muted;
                else
                    activityLevel = RenderActivityLevel.Normal;
            }
            else
            {
                activityLevel = RenderActivityLevel.Background;
            }

            this.Hangar.ModelView.RenderActivityLevel = activityLevel;
        }


        private void ScheduleUnloadLoadingView()
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1.0);
            timer.Tick += (o, e) =>
                {
                    timer.Stop();
                    this.UnloadLoadingView();
                };
            timer.Start();
        }

        internal void UnloadLoadingView()
        {
            if (this.LoadComplete != null)
                this.LoadComplete(this, new EventArgs());

            this.IsLoaded = true;

            this.LoadingViewModel = null;
        }

    }
}
