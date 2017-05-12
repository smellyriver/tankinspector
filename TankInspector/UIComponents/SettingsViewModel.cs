using Smellyriver.TankInspector.Modeling;
using System;
using System.IO;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class SettingsViewModel : MainWindowOverlayViewModel
    {

        public override bool IsFullWindow => false;

	    public bool IsGameFolderChanged { get; set; }

        private readonly string _originalGameFolder;

        public string GameFolder
        {
            get => ApplicationSettings.Default.GamePathes.Count == 0 ? null : ApplicationSettings.Default.GamePathes[0];
	        set
            {
                this.IsGameFolderChanged = _originalGameFolder == null || (Path.GetFullPath(_originalGameFolder) != Path.GetFullPath(value));

                var path = Path.GetFullPath(value);
                if (Database.IsPathValid(path))
                {
                    ApplicationSettings.Default.GamePathes[0] = path;
                    ApplicationSettings.Default.EnsureGamePathesDistinct();
                    ApplicationSettings.Default.Save();
                }

                this.RaisePropertyChanged(() => this.GameFolder);
            }
        }

        public bool SsaoEnabled
        {
            get => ApplicationSettings.Default.SSAOEnabled;
	        set
            {
                ApplicationSettings.Default.SSAOEnabled = value;
                ApplicationSettings.Default.Save();
            }
        }

        public bool SmaaEnabled
        {
            get => ApplicationSettings.Default.SMAAEnabled;
	        set
            {
                ApplicationSettings.Default.SMAAEnabled = value;
                ApplicationSettings.Default.Save();
            }
        }

        public bool SpecularTextureEnabled
        {
            get => ApplicationSettings.Default.SpecularTextureEnabled;
	        set
            {
                ApplicationSettings.Default.SpecularTextureEnabled = value;
                ApplicationSettings.Default.Save();
            }
        }

        public bool NormalTextureEnabled
        {
            get => ApplicationSettings.Default.NormalTextureEnabled;
	        set
            {
                ApplicationSettings.Default.NormalTextureEnabled = value;
                ApplicationSettings.Default.Save();
            }
        }

        public bool CollisionModelStrokeEnabled
        {
            get => ApplicationSettings.Default.CollisionModelStrokeEnabled;
	        set
            {
                ApplicationSettings.Default.CollisionModelStrokeEnabled = value;
                ApplicationSettings.Default.Save();
            }
        }

        public bool WireframeMode
        {
            get => ApplicationSettings.Default.WireframeMode;
	        set
            {
                ApplicationSettings.Default.WireframeMode = value;
                ApplicationSettings.Default.Save();
            }
        }

        public bool ShowFps
        {
            get => ApplicationSettings.Default.ShowFPS;
	        set
            {
                ApplicationSettings.Default.ShowFPS = value;
                ApplicationSettings.Default.Save();
            }
        }
        public bool ShowModelTriangleCount
        {
            get => ApplicationSettings.Default.ShowModelTriangleCount;
	        set
            {
                ApplicationSettings.Default.ShowModelTriangleCount = value;
                ApplicationSettings.Default.Save();
            }
        }


        public bool AutoLoadEliteConfigEnabled
        {
            get => ApplicationSettings.Default.AutoLoadEliteConfigEnabled;
	        set
            {
                ApplicationSettings.Default.AutoLoadEliteConfigEnabled = value;
                ApplicationSettings.Default.Save();
            }
        }

        public bool AutoResetReferenceTankEnabled
        {
            get => ApplicationSettings.Default.AutoResetReferenceTankEnabled;
	        set
            {
                ApplicationSettings.Default.AutoResetReferenceTankEnabled = value;
                ApplicationSettings.Default.Save();
            }
        }

        public bool ResetShellTypeOnChangingVehicle
        {
            get => ApplicationSettings.Default.ResetShellTypeOnChangingVehicle;
	        set
            {
                ApplicationSettings.Default.ResetShellTypeOnChangingVehicle = value;
                ApplicationSettings.Default.Save();
            }
        }

        public bool RecruitFullyTrainedCrews
        {
            get => ApplicationSettings.Default.RecruitFullyTrainedCrews;
	        set
            {
                ApplicationSettings.Default.RecruitFullyTrainedCrews = value;
                ApplicationSettings.Default.Save();
            }
        }

        public bool RecruitNewCrewsOnChangingVehicle
        {
            get => ApplicationSettings.Default.RecruitNewCrewsOnChangingVehicle;
	        set
            {
                ApplicationSettings.Default.RecruitNewCrewsOnChangingVehicle = value;
                ApplicationSettings.Default.Save();
            }
        }

        public bool ClearEquipmentsOnChangingVehicle
        {
            get => ApplicationSettings.Default.ClearEquipmentsOnChangingVehicle;
	        set
            {
                ApplicationSettings.Default.ClearEquipmentsOnChangingVehicle = value;
                ApplicationSettings.Default.Save();
            }
        }

        public bool ClearConsumablesOnChangingVehicle
        {
            get => ApplicationSettings.Default.ClearConsumablesOnChangingVehicle;
	        set
            {
                ApplicationSettings.Default.ClearConsumablesOnChangingVehicle = value;
                ApplicationSettings.Default.Save();
            }
        }

        public bool ShowBackgroundImage
        {
            get => ApplicationSettings.Default.ShowBackgroundImage;
	        set
            {
                ApplicationSettings.Default.ShowBackgroundImage = value;
                ApplicationSettings.Default.Save();
            }
        }

        public bool ShowSoftwareCrosshair
        {
            get => ApplicationSettings.Default.ShowSoftwareCrosshair;
	        set
            {
                ApplicationSettings.Default.ShowSoftwareCrosshair = value;
                ApplicationSettings.Default.Save();
            }
        }

        public bool ShowDescriptionInDataTip
        {
            get => ApplicationSettings.Default.ShowDescriptionInDataTip;
	        set
            {
                ApplicationSettings.Default.ShowDescriptionInDataTip = value;
                ApplicationSettings.Default.Save();
            }
        }
        public bool UseRealTraverseSpeedOnTurretController
        {
            get => ApplicationSettings.Default.UseRealTraverseSpeedOnTurretController;
	        set
            {
                ApplicationSettings.Default.UseRealTraverseSpeedOnTurretController = value;
                ApplicationSettings.Default.Save();
            }
        }

        public bool ShowAsMuchStatsAsPossible
        {
            get => ApplicationSettings.Default.ShowAsMuchStatsAsPossible;
	        set
            {
                ApplicationSettings.Default.ShowAsMuchStatsAsPossible = value;
                ApplicationSettings.Default.Save();
            }
        }

        

        private int _selectedAnisotropicFilterLevel;
        public int SelectedAnisotropicFilterLevel
        {
            get => _selectedAnisotropicFilterLevel;
	        set
            {
                _selectedAnisotropicFilterLevel = value;
                ApplicationSettings.Default.AnisotropicFilterLevel = value;
                ApplicationSettings.Default.Save();
            }
        }

        public bool AnisotropicFilterEnabled
        {
            get => ApplicationSettings.Default.AnisotropicFilterLevel > 0;
	        set
            {
                ApplicationSettings.Default.AnisotropicFilterLevel = value ? _selectedAnisotropicFilterLevel : 0;
                ApplicationSettings.Default.Save();
                this.RaisePropertyChanged(() => this.AnisotropicFilterEnabled);
            }
        }

        public int SelectedPhotoSizeLevel
        {
            get => ApplicationSettings.Default.PhotoSize;
	        set
            {
                ApplicationSettings.Default.PhotoSize = value;
                ApplicationSettings.Default.Save();
            }
        }

        public int[] AnisotropicLevels { get; }

        public int[] PhotoSizeLevels { get; }

        public SettingsViewModel(MainWindowViewModel owner)
            : base(owner)
        {
            AnisotropicLevels = new[] { 2, 4, 8, 16 };
            this.PhotoSizeLevels = new[] { 1024, 2048, 4096 };
            _selectedAnisotropicFilterLevel = Math.Max(2, ApplicationSettings.Default.AnisotropicFilterLevel);
            _originalGameFolder = ApplicationSettings.Default.GamePathes.Count == 0 ? null : ApplicationSettings.Default.GamePathes[0];

            ApplicationSettings.Default.SettingChanging += OnApplicationSettingsChanging;
        }

	    private void OnApplicationSettingsChanging(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            if (e.SettingName == "GamePathes")
                this.RaisePropertyChanged(() => this.GameFolder);
        }


    }
}
