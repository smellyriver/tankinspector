using Smellyriver.TankInspector.Modeling;

namespace Smellyriver.TankInspector.UIComponents
{
	internal partial class HangarViewModel
    {
        public enum MajorViewSlideDirectionEnum
        {
            Left,
            Right
        }



        private bool _isSidebarShown;
        public bool IsSidebarShown
        {
            get => _isSidebarShown;
	        private set
            {
                _isSidebarShown = value;
                this.RaisePropertyChanged(() => this.IsSidebarShown);
                this.RaisePropertyChanged(() => this.IsPredecessorPanelShown);
                this.RaisePropertyChanged(() => this.IsSuccessorPanelShown);
            }
        }


        private bool _isInSniperMode;
        public bool IsInSniperMode
        {
            get => _isInSniperMode;
	        set
            {
                _isInSniperMode = value;
                this.RaisePropertyChanged(() => this.IsInSniperMode);

                if (_isInSniperMode)
                    this.ModelView.CameraMode = Design.CameraMode.Sniper;
                else
                    this.ModelView.CameraMode = Design.CameraMode.TrackBall;
            }
        }


        public bool IsPredecessorPanelShown => this.Tank != null && this.Tank.HasPredecessor && this.IsSidebarShown;

	    public bool IsSuccessorPanelShown => this.Tank != null && this.Tank.HasSuccessor && this.IsSidebarShown;


	    private bool _isModelViewShown;

        public bool IsModelViewShown
        {
            get => _isModelViewShown;
	        set
            {
                _isModelViewShown = value;
                this.RaisePropertyChanged(() => this.IsModelViewShown);
            }
        }

        private bool _isDetailedDataViewShown;
        public bool IsDetailedDataViewShown
        {
            get => _isDetailedDataViewShown;
	        set
            {
                _isDetailedDataViewShown = value;
                this.RaisePropertyChanged(() => this.IsDetailedDataViewShown);
            }
        }

        private MajorViewSlideDirectionEnum _majorViewSlideDirection;
        public MajorViewSlideDirectionEnum MajorViewSlideDirection
        {
            get => _majorViewSlideDirection;
	        private set
            {
                _majorViewSlideDirection = value;
                this.RaisePropertyChanged(() => this.MajorViewSlideDirection);
            }
        }


        private bool _isShowingCollisionModel;
        public bool IsShowingCollisionModel
        {
            get { return _isShowingCollisionModel; }
            set
            {
                if (_isShowingCollisionModel != value)
                {
                    _isShowingCollisionModel = value;
                    if (_isShowingCollisionModel)
                        this.ModelView.ModelType = Graphics.Model.ModelType.Collision;
                    else
                        this.ModelView.ModelType = Graphics.Model.ModelType.Undamaged;

                    this.RaisePropertyChanged(() => this.IsShowingCollisionModel);
                }
            }
        }

        public bool CanSwitchReferenceTank => this.ReferenceTank != null && this.ReferenceTank.Tank is Tank;


	    private void TurnToRightPage()
        {
            this.MajorViewSlideDirection = MajorViewSlideDirectionEnum.Right;
            this.IsModelViewShown = !this.IsModelViewShown;
            this.IsDetailedDataViewShown = !this.IsDetailedDataViewShown;
        }

        private void TurnToLeftPage()
        {
            this.MajorViewSlideDirection = MajorViewSlideDirectionEnum.Left;
            this.IsModelViewShown = !this.IsModelViewShown;
            this.IsDetailedDataViewShown = !this.IsDetailedDataViewShown;
        }


        internal void ShowTankGallery()
        {
            _owner.TankGallery.Show();
        }

        internal void HideSidebar()
        {
            this.IsSidebarShown = false;
        }

        internal void ShowSidebar()
        {
            this.IsSidebarShown = true;
        }


        private void ToggleTankDescriptionVisibility()
        {
            ApplicationSettings.Default.HangarShowTankDescription = !ApplicationSettings.Default.HangarShowTankDescription;
            ApplicationSettings.Default.Save();
        }
    }
}
