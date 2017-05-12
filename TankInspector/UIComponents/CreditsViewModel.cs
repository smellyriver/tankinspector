using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class CreditsViewModel : MainWindowOverlayViewModel
    {
        public class SpecialThankItem
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public string Description { get; set; }
            public bool IsSubItem { get; set; }

        }

        public string ApplicationVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

	    public override bool IsFullWindow => false;

	    public bool HasLocalizerLink
        {
            get
            {
                var url = App.GetLocalizedString("LocalizerLinkURL");
                return !string.IsNullOrEmpty(url) && url != "(empty)";
            }
        }

        public ObservableCollection<SpecialThankItem> SpecialThanks { get; private set; }

        public CreditsViewModel(MainWindowViewModel owner)
            : base(owner)
        {
            this.InitializeSpecialThanks();
        }

        private void InitializeSpecialThanks()
        {
            this.SpecialThanks = new ObservableCollection<SpecialThankItem>();


            var wotnews = new SpecialThankItem
            {
                Name = App.GetLocalizedString("WOTNews"),
                Description = App.GetLocalizedString("WOTNewsDescription"),
                Url = "http://www.wot-news.com",
            };

            var duowan = new SpecialThankItem
            {
                Name = App.GetLocalizedString("DuowanWoT"),
                Description = App.GetLocalizedString("DuowanWoTDescription"),
                Url = "http://wot.duowan.com",
            };


            var xvmGarphy = new SpecialThankItem
            {
                Name = App.GetLocalizedString("XVMGarphy"),
                Description = App.GetLocalizedString("XVMGarphyDescription"),
                Url = "http://xvm.garphy.com",
            };

            var vbaddict = new SpecialThankItem
            {
                Name = App.GetLocalizedString("vBAddict"),
                Description = App.GetLocalizedString("vBAddictDescription"),
                Url = "http://www.vbaddict.net/wot.php?ref=TankInspector",
            };

            var ftr = new SpecialThankItem
            {
                Name = App.GetLocalizedString("FTR"),
                Description = App.GetLocalizedString("FTRDescription"),
                Url = "http://ftr.wot-news.com",
            };

			var baidu = new SpecialThankItem
			{
				Name = App.GetLocalizedString("BaiduWoT"),
				Description = App.GetLocalizedString("BaiduWoTDescription"),
				Url = "http://tieba.baidu.com/f?kw=坦克世界",
			};

			
			this.SpecialThanks.Add(ftr);
			this.SpecialThanks.Add(vbaddict);
			this.SpecialThanks.Add(wotnews);
	        this.SpecialThanks.Add(baidu);
			this.SpecialThanks.Add(xvmGarphy);
			this.SpecialThanks.Add(duowan);
		}

    }
}
