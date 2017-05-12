using Smellyriver.Utilities;
using Smellyriver.Wpf.Animation;
using Smellyriver.Wpf.Utilities;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Globalization;
using Smellyriver.TankInspector.Graphics.Scene;
using Ookii.Dialogs.Wpf;

namespace Smellyriver.TankInspector.UIComponents
{
    partial class HangarView : UserControl
    {

        static HangarView()
        {
            DataContextProperty.OverrideMetadata(typeof(HangarView), new FrameworkPropertyMetadata(HangarView.OnDataContextChanged));
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(Int32.MaxValue));
        }

        private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HangarView)d).OnDataContextChanged(e.OldValue, e.NewValue);
        }

        internal HangarViewModel ViewModel
        {
            get => this.DataContext as HangarViewModel;
	        set => this.DataContext = value;
        }

        private IPreviewable _previewingComponent;

        private ToggleButton _currentIsCrewDeadToggleButton;
        private MenuItem _currentCrewMenuItem;
        private FrameworkElement _currentTrainingLevelPanel;

        private Button _bulkLearnCamouflageButton;
        private Button _bulkLearnBiAButton;

        private Point _previousMousePosOnModelViewContainer;

        //private DispatcherOperation _previewReplacedTankTask;
        public HangarView()
        {
            InitializeComponent();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void OnDataContextChanged(object oldValue, object newValue)
        {
            var oldVm = oldValue as HangarViewModel;
            var newVm = newValue as HangarViewModel;

            if (oldVm != null)
                oldVm.PropertyChanged -= ViewModel_PropertyChanged;

            if (newVm != null)
            {
                this.UpdateMajorViewSlideAnimation(newVm.MajorViewSlideDirection);
                newVm.PropertyChanged += ViewModel_PropertyChanged;
            }
        }

	    private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MajorViewSlideDirection")
            {
                this.UpdateMajorViewSlideAnimation(this.ViewModel.MajorViewSlideDirection);
            }
        }


        private void UpdateMajorViewSlideAnimation(HangarViewModel.MajorViewSlideDirectionEnum direction)
        {
            Thickness enterMargin, exitMargin;
            if (direction == HangarViewModel.MajorViewSlideDirectionEnum.Left)
            {
                enterMargin = new Thickness(1280, 0, -1280, 0);
                exitMargin = new Thickness(-1280, 0, 1280, 0);
            }
            else
            {
                enterMargin = new Thickness(-1280, 0, 1280, 0);
                exitMargin = new Thickness(1280, 0, -1280, 0);
            }

            var duration = new Duration(TimeSpan.FromSeconds(0.5));
            var enterEase = new QuadraticEase { EasingMode = EasingMode.EaseOut };

            var enterStoryboard = new Storyboard();
            var enterMarginAnimation = new ThicknessAnimation(enterMargin, new Thickness(0), duration, FillBehavior.HoldEnd) { EasingFunction = enterEase };
            Storyboard.SetTargetProperty(enterMarginAnimation, new PropertyPath(MarginProperty));
            enterStoryboard.Children.Add(enterMarginAnimation);
            var enterOpacityAnimation = new DoubleAnimation(1.0, duration) { EasingFunction = enterEase };
            Storyboard.SetTargetProperty(enterOpacityAnimation, new PropertyPath(OpacityProperty));
            enterStoryboard.Children.Add(enterOpacityAnimation);


            var exitEase = new QuadraticEase { EasingMode = EasingMode.EaseOut };
            var exitStoryboard = new Storyboard();
            var exitMarginAnimation = new ThicknessAnimation(new Thickness(0), exitMargin, duration, FillBehavior.HoldEnd) { EasingFunction = exitEase };
            Storyboard.SetTargetProperty(exitMarginAnimation, new PropertyPath(MarginProperty));
            exitStoryboard.Children.Add(exitMarginAnimation);
            var exitOpacityAnimation = new DoubleAnimation(0.0, duration) { EasingFunction = exitEase };
            Storyboard.SetTargetProperty(exitOpacityAnimation, new PropertyPath(OpacityProperty));
            exitStoryboard.Children.Add(exitOpacityAnimation);

            ShowHideAnimation.SetShowStoryboard(this.ModelViewSlideTransitionContainer, enterStoryboard);
            ShowHideAnimation.SetShowStoryboard(this.DetailedViewTransitionContainer, enterStoryboard);
            ShowHideAnimation.SetHideStoryboard(this.ModelViewSlideTransitionContainer, exitStoryboard);
            ShowHideAnimation.SetHideStoryboard(this.DetailedViewTransitionContainer, exitStoryboard);
        }

        private void TankButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var tankVm = (TankViewModel)button.DataContext;
            this.ViewModel.LoadTank(tankVm.Tank);
        }


        //private void TankButton_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    if (this.ViewModel.IsDetailedDataViewShown)
        //    {
        //        if (_previewReplacedTankTask != null)
        //            _previewReplacedTankTask.Abort();

        //        _previewReplacedTankTask = Dispatcher.BeginInvoke(new Action(() =>
        //            {
        //                var button = (Button)sender;
        //                var tankVm = (TankViewModel)button.DataContext;
        //                var eliteTankVm = tankVm.Clone();
        //                this.ViewModel.ConfigureTank(eliteTankVm);
        //                this.ViewModel.InheritCurrentTank(eliteTankVm);
        //                this.ViewModel.DetailedDataView.ScheduleComparationWithReplacedTank(eliteTankVm);
        //                _previewReplacedTankTask = null;
        //            }), DispatcherPriority.Background);
        //    }

        //}

        //private void TankButton_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    if (this.ViewModel.IsDetailedDataViewShown)
        //    {
        //        if (_previewReplacedTankTask != null)
        //            _previewReplacedTankTask.Abort();

        //        this.ViewModel.DetailedDataView.ClearComparedComponent();
        //    }
        //}

        private void CurrentTankButton_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.ShowTankGallery();
        }

        private void ServiceMenuDetailedDataRelatedComponentHeader_MouseEnter(object sender, MouseEventArgs e)
        {
            var element = (FrameworkElement)sender;

            if (this.ViewModel.IsDetailedDataViewShown)
            {
                ToolTipService.SetIsEnabled(element, false);
                var component = element.DataContext as IDetailedDataRelatedComponent;
                this.ViewModel.DetailedDataView.HighlightedComponentType = component.ComponentType;
                this.ViewModel.DetailedDataView.HighlightedModificationDomains = component.ModificationDomains;
            }
            else
            {
                ToolTipService.SetIsEnabled(element, true);
            }

        }

        private void ServiceMenuDetailedDataRelatedComponentHeader_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.ViewModel.IsDetailedDataViewShown)
            {
                this.ViewModel.DetailedDataView.HighlightedComponentType = DetailedDataRelatedComponentType.None;
                this.ViewModel.DetailedDataView.ClearComparedComponent();
            }
        }


        private void ServiceMenuTopLevelModuleOrShellItem_MouseEnter(object sender, MouseEventArgs e)
        {
            var element = (FrameworkElement)sender;

            if (this.ViewModel.IsDetailedDataViewShown)
            {

                var component = element.DataContext as IDetailedDataRelatedComponent;
                this.ViewModel.DetailedDataView.HighlightedComponentType = component.ComponentType;
                this.ViewModel.DetailedDataView.HighlightedModificationDomains = component.ModificationDomains;

                _previewingComponent = (IPreviewable)element.DataContext;
                _previewingComponent.IsPreviewing = true;
                this.ViewModel.DetailedDataView.ScheduleComparationWithReplacedComponent(element.DataContext);
            }
        }

        private void ServiceMenuPreviewableItem_MouseLeave(object sender, MouseEventArgs e)
        {
            var element = (FrameworkElement)sender;

            if (this.ViewModel.IsDetailedDataViewShown)
            {
                if (_previewingComponent != null)
                {
                    _previewingComponent.IsPreviewing = false;
                    _previewingComponent = null;
                }
                this.ViewModel.DetailedDataView.HighlightedComponentType = DetailedDataRelatedComponentType.None;
                this.ViewModel.DetailedDataView.ClearComparedComponent();
            }
        }

        private void ServiceMenuTopLevelEquipmentItem_MouseEnter(object sender, MouseEventArgs e)
        {
            var element = (FrameworkElement)sender;

            if (this.ViewModel.IsDetailedDataViewShown)
            {
                _previewingComponent = (IPreviewable)element.DataContext;
                _previewingComponent.IsPreviewing = true;

                var component = element.DataContext as IDetailedDataRelatedComponent;
                this.ViewModel.DetailedDataView.HighlightedComponentType = component.ComponentType;
                this.ViewModel.DetailedDataView.HighlightedModificationDomains = component.ModificationDomains;

                var index = this.ViewModel.Tank.CurrentEquipmentSlotIndex;
                if (index.HasValue)
                    this.ViewModel.DetailedDataView.ScheduleComparationWithReplacedEquipment(index.Value, (IEquipmentViewModel)element.DataContext);
            }
        }

        private void ServiceMenuTopLevelConsumableItem_MouseEnter(object sender, MouseEventArgs e)
        {
            var element = (FrameworkElement)sender;

            if (this.ViewModel.IsDetailedDataViewShown)
            {
                _previewingComponent = (IPreviewable)element.DataContext;
                _previewingComponent.IsPreviewing = true;

                var component = element.DataContext as IDetailedDataRelatedComponent;
                this.ViewModel.DetailedDataView.HighlightedComponentType = component.ComponentType;
                this.ViewModel.DetailedDataView.HighlightedModificationDomains = component.ModificationDomains;

                var index = this.ViewModel.Tank.CurrentConsumableSlotIndex;
                if (index.HasValue)
                    this.ViewModel.DetailedDataView.ScheduleComparationWithReplacedConsumable(index.Value, (IConsumableViewModel)element.DataContext);
            }
        }


        private void ServiceMenuSecondLevelCrewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            var element = (FrameworkElement)sender;

            if (this.ViewModel.IsDetailedDataViewShown)
            {
                _previewingComponent = (IPreviewable)element.DataContext;
                _previewingComponent.IsPreviewing = true;

                this.ViewModel.DetailedDataView.ScheduleComparationWithToggledCrewSkill((CrewSkillViewModel)element.DataContext);
            }
        }

        private void ServiceMenuSecondLevelCrewItem_Click(object sender, EventArgs e)
        {
            var element = (FrameworkElement)sender;

            if (this.ViewModel.IsDetailedDataViewShown)
            {
                this.ViewModel.DetailedDataView.ScheduleComparationWithToggledCrewSkill((CrewSkillViewModel)element.DataContext);
            }
        }

        private void ServiceMenuTopLevelCrewItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            _currentCrewMenuItem = (MenuItem)sender;
            var popup = VisualTreeHelperEx.FindChild<Popup>(_currentCrewMenuItem, "Popup");
            _currentIsCrewDeadToggleButton = VisualTreeHelperEx.FindChild<ToggleButton>(popup.Child, "IsCrewDeadToggleButton");
            _currentTrainingLevelPanel = _currentIsCrewDeadToggleButton.Parent as Grid;
            _currentIsCrewDeadToggleButton.MouseEnter += IsCrewDeadToggleButton_MouseEnter;
            _currentIsCrewDeadToggleButton.MouseLeave += IsCrewDeadToggleButton_MouseLeave;
            _currentIsCrewDeadToggleButton.Click += IsCrewDeadToggleButton_Click;
            _currentTrainingLevelPanel.PreviewMouseRightButtonDown += ServiceMenuPreviewableItem_PreviewMouseRightButtonDown;
            _currentTrainingLevelPanel.PreviewMouseRightButtonUp += ServiceMenuPreviewableItem_PreviewMouseRightButtonUp;
        }

	    private void IsCrewDeadToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.IsDetailedDataViewShown)
            {
                this.ViewModel.DetailedDataView.ScheduleComparationWithToggledCrewDeadOrAlive((CrewViewModel)_currentTrainingLevelPanel.DataContext);
            }
        }

	    private void IsCrewDeadToggleButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.ViewModel.IsDetailedDataViewShown)
            {
                this.ViewModel.DetailedDataView.HighlightedComponentType = DetailedDataRelatedComponentType.None;
                this.ViewModel.DetailedDataView.ClearComparedComponent();
            }
        }

	    private void IsCrewDeadToggleButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.ViewModel.IsDetailedDataViewShown)
            {
                var component = _currentCrewMenuItem.DataContext as IDetailedDataRelatedComponent;
                this.ViewModel.DetailedDataView.HighlightedComponentType = component.ComponentType;
                this.ViewModel.DetailedDataView.HighlightedModificationDomains = component.ModificationDomains;

                this.ViewModel.DetailedDataView.ScheduleComparationWithToggledCrewDeadOrAlive((CrewViewModel)_currentTrainingLevelPanel.DataContext);
            }
        }

        private void ServiceMenuTopLevelCrewItem_SubmenuClosed(object sender, RoutedEventArgs e)
        {

            if (_currentIsCrewDeadToggleButton != null)
            {
                _currentIsCrewDeadToggleButton.MouseEnter -= IsCrewDeadToggleButton_MouseEnter;
                _currentIsCrewDeadToggleButton.MouseLeave -= IsCrewDeadToggleButton_MouseLeave;
                _currentIsCrewDeadToggleButton = null;
            }

            if (_currentTrainingLevelPanel != null)
            {
                _currentTrainingLevelPanel.PreviewMouseRightButtonDown -= ServiceMenuPreviewableItem_PreviewMouseRightButtonDown;
                _currentTrainingLevelPanel.PreviewMouseRightButtonUp -= ServiceMenuPreviewableItem_PreviewMouseRightButtonUp;
                _currentTrainingLevelPanel = null;
            }

            _currentCrewMenuItem = null;
        }


        private void ServiceMenuTopLevelCrewHeader_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            var crewMenu = (MenuItem)sender;
            var popup = VisualTreeHelperEx.FindChild<Popup>(crewMenu, "Popup");
            _bulkLearnBiAButton = VisualTreeHelperEx.FindChild<Button>(popup.Child, "BulkLearnBiAButton");
            _bulkLearnCamouflageButton = VisualTreeHelperEx.FindChild<Button>(popup.Child, "BulkLearnCamouflageButton");

            _bulkLearnBiAButton.MouseEnter += BulkLearnBiAButton_MouseEnter;
            _bulkLearnBiAButton.MouseLeave += BulkLearnBiAButton_MouseLeave;

            _bulkLearnCamouflageButton.MouseEnter += BulkLearnCamouflageButton_MouseEnter;
            _bulkLearnCamouflageButton.MouseLeave += BulkLearnCamouflageButton_MouseLeave;
        }

	    private void BulkLearnCamouflageButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.ViewModel.IsDetailedDataViewShown)
            {
                this.ViewModel.DetailedDataView.ClearComparedComponent();
            }
        }

	    private void BulkLearnCamouflageButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.ViewModel.IsDetailedDataViewShown)
            {
                this.ViewModel.DetailedDataView.ScheduleComparationWithAllCrewsCamouflageLearnt();
            }
        }

	    private void BulkLearnBiAButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.ViewModel.IsDetailedDataViewShown)
            {
                this.ViewModel.DetailedDataView.ClearComparedComponent();
            }
        }

	    private void BulkLearnBiAButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.ViewModel.IsDetailedDataViewShown)
            {
                this.ViewModel.DetailedDataView.ScheduleComparationWithAllCrewsBiALearnt();
            }
        }
        private void ServiceMenuTopLevelCrewHeader_SubmenuClosed(object sender, RoutedEventArgs e)
        {

            if (_bulkLearnBiAButton != null)
            {
                _bulkLearnBiAButton.MouseEnter -= BulkLearnBiAButton_MouseEnter;
                _bulkLearnBiAButton.MouseLeave -= BulkLearnBiAButton_MouseLeave;
                _bulkLearnBiAButton = null;
            }

            if (_bulkLearnCamouflageButton != null)
            {
                _bulkLearnCamouflageButton.MouseEnter -= BulkLearnCamouflageButton_MouseEnter;
                _bulkLearnCamouflageButton.MouseLeave -= BulkLearnCamouflageButton_MouseLeave;
                _bulkLearnCamouflageButton = null;
            }

        }


        private void LoadEliteConfigButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.ViewModel.IsDetailedDataViewShown)
            {
                this.ViewModel.DetailedDataView.ScheduleComparationWithEliteComponent();
            }
        }

        private void LoadEliteConfigButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.ViewModel.IsDetailedDataViewShown)
            {
                this.ViewModel.DetailedDataView.ClearComparedComponent();
            }
        }

        private void SidebarTrigger_MouseEnter(object sender, MouseEventArgs e)
        {
            this.ViewModel.ShowSidebar();
            this.SearchTextBox.Focus();
        }

        private void ServiceMenuPreviewableItem_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.ViewModel.IsDetailedDataViewShown)
            {
                var element = (FrameworkElement)sender;
                var ancestor = VisualTreeHelperEx.FindRoot(element) as FrameworkElement;
                var storyboard = this.FindResource("OpaquizeMenuItemsStoryboard") as Storyboard;
                element.ReleaseMouseCapture();
                storyboard.Begin(ancestor);
            }
        }

        private void ServiceMenuTopLevelEquipment_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            this.ViewModel.Tank.CurrentEquipmentSlotIndex = HangarView.GetTagIndex(menuItem);
        }

        private void ServiceMenuTopLevelConsumable_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            this.ViewModel.Tank.CurrentConsumableSlotIndex = HangarView.GetTagIndex(menuItem);
        }

        private static int GetTagIndex(FrameworkElement element)
        {
            return int.Parse((string)element.Tag, CultureInfo.InvariantCulture);
        }

        private void ServiceMenuPreviewableItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.ViewModel.IsDetailedDataViewShown)
            {
                var element = (FrameworkElement)sender;
                var ancestor = VisualTreeHelperEx.FindRoot(element) as FrameworkElement;
                var storyboard = this.FindResource("TrasparentizeMenuItemsStoryboard") as Storyboard;
                element.CaptureMouse();
                storyboard.Begin(ancestor);
            }
        }

        private void SearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.Up:
                    this.ViewModel.HighlightPreviousSearchResult();
                    e.Handled = true;
                    break;
                case Key.Down:
                    this.ViewModel.HighlightNextSearchResult();
                    e.Handled = true;
                    break;
                case Key.Enter:
                    if (Keyboard.GetKeyStates(Key.LeftCtrl) == KeyStates.Down
                        || Keyboard.GetKeyStates(Key.RightCtrl) == KeyStates.Down)
                        this.ViewModel.SelectHighlightedSearchResultTankAsReferenceTank();
                    else
                        this.ViewModel.SelectHighlightedSearchResultTank();
                    e.Handled = true;
                    break;
                case Key.Escape:
                    var textBox = (TextBox)sender;
                    if (textBox.Text.Length > 0)
                        textBox.Text = string.Empty;
                    else
                        this.ViewModel.HideSidebar();
                    e.Handled = true;
                    break;
            }

        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
                e.Handled = true;
        }

        private void HangarView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // MouseEnter and MouseLeave events are often not stable to produce desired show/hide behavior
            // we check for mouse position to make this smooth
            if (this.ViewModel != null && this.ViewModel.IsSidebarShown && !this.TankHierachyPanel.IsMouseOver && !this.ReferencePanel.IsMouseOver)
            {
                if (this.SearchTextBox.IsFocused && this.SearchTextBox.Text.Length > 0)
                    return;

                var point = Mouse.GetPosition(this.Sidebar);
                if (point.X < 0 || point.Y < 0)
                    this.ViewModel.HideSidebar();
            }
        }

        private void HangarView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat)
                return;
            switch (e.Key)
            {
                case Key.LeftShift:
                    if (this.ViewModel != null && this.ViewModel.IsModelViewShown)
                    {
                        this.ViewModel.IsInSniperMode = !this.ViewModel.IsInSniperMode;
                        if (this.ViewModel.IsInSniperMode)
                            this.SniperModeDecorator.Focus();
                        e.Handled = true;
                    }
                    break;


            }
        }

        private void HangarView_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.PrintScreen:
                    if (this.ViewModel != null && this.ViewModel.IsModelViewShown)
                    {
                        e.Handled = true;

                        this.ViewModel.IsTakingPhoto = true;

                        var saveFileDialog = new VistaSaveFileDialog();
                        saveFileDialog.Filter = App.GetLocalizedString("PNGFileFilter");
                        saveFileDialog.AddExtension = true;
                        saveFileDialog.DefaultExt = "png";
                        var filename = this.ViewModel.Tank.Tank.HyphenFullKey + ".png";
                        foreach (var chr in System.IO.Path.GetInvalidFileNameChars())
                            filename = filename.Replace(chr, '_');
                        saveFileDialog.FileName = filename;
                        saveFileDialog.CheckPathExists = true;
                        saveFileDialog.OverwritePrompt = true;
                        saveFileDialog.Title = App.GetLocalizedString("SaveAs");
                        if (saveFileDialog.ShowDialog() == true)
                        {
                            var width = ApplicationSettings.Default.PhotoSize.Clamp(512, 4096);
                            var height = (int)(this.ModelViewContainer.ActualHeight * width / this.ModelViewContainer.ActualWidth).Clamp(1, 4096);
                            HangarScene.Current.HdShot(width, height, saveFileDialog.FileName);
                        }

                        this.ViewModel.IsTakingPhoto = false;
                    }
                    break;
            }
        }

        //private int _previousHitTestBulletinHorizontalPosition = 1; // 0: left, 1: right
        //private int _previousHitTestBulletinVerticalPosition = 0; // 0: top, 1: bottom


	    private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (!ApplicationSettings.Default.ShowSoftwareCrosshair)
                return;

            if (this.ViewModel == null || !this.ViewModel.IsModelViewShown)
                return;

            if (this.ViewModel.ModelView == null || !this.ViewModel.ModelView.IsMouseOverModel)
                return;

            var mousePos = Mouse.GetPosition(this.ModelViewContainer);
            if (mousePos == _previousMousePosOnModelViewContainer)
                return;

            _previousMousePosOnModelViewContainer = mousePos;

            var availableWidth = this.ModelViewContainer.ActualWidth;
            var availableHeight = this.ModelViewContainer.ActualHeight;

            this.CrosshairHorizontalLine.X1 = 0;
            this.CrosshairHorizontalLine.X2 = availableWidth;
            this.CrosshairHorizontalLine.Y1 = this.CrosshairHorizontalLine.Y2 = mousePos.Y;
            this.CrosshairVerticalLine.Y1 = 0;
            this.CrosshairVerticalLine.Y2 = availableHeight;
            this.CrosshairVerticalLine.X1 = this.CrosshairVerticalLine.X2 = mousePos.X;

            //const double bulletinMargin = 10;

            //var bulletinX = mousePos.X + bulletinMargin;
            //var bulletinY = mousePos.Y - this.HitTestBulletin.ActualHeight - bulletinMargin;
            //var horizontalPosition = 1;
            //var verticalPosition = 0;
            //if (availableWidth - bulletinX < this.HitTestBulletin.ActualWidth)
            //{
            //    bulletinX = mousePos.X - this.HitTestBulletin.ActualWidth - bulletinMargin;
            //    horizontalPosition = 0;
            //}

            //if (bulletinY < 0)
            //{
            //    bulletinY = mousePos.Y + bulletinMargin;
            //    verticalPosition = 1;
            //}

            //if (horizontalPosition != _previousHitTestBulletinHorizontalPosition || verticalPosition != _previousHitTestBulletinVerticalPosition)
            //{
            //    var animationX = new DoubleAnimation(bulletinX, new Duration(TimeSpan.FromSeconds(0.2)), FillBehavior.Stop);
            //    var animationY = new DoubleAnimation(bulletinY, new Duration(TimeSpan.FromSeconds(0.2)), FillBehavior.Stop);

            //    this.HitTestBulletin.BeginAnimation(Canvas.LeftProperty, animationX);
            //    this.HitTestBulletin.BeginAnimation(Canvas.TopProperty, animationY);

            //    _previousHitTestBulletinHorizontalPosition = horizontalPosition;
            //    _previousHitTestBulletinVerticalPosition = verticalPosition;
            //}
            //else
            //{
            //    Canvas.SetLeft(this.HitTestBulletin, bulletinX);
            //    Canvas.SetTop(this.HitTestBulletin, bulletinY);
            //}
        }




    }
}
