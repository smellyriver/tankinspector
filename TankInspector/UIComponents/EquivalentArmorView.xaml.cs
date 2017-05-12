using Smellyriver.TankInspector.Graphics.Scene;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for EquivalentArmorPanel.xaml
    /// </summary>
    public partial class EquivalentArmorView : UserControl
    {



        public ShootTestResult ShootTestResult
        {
            get => (ShootTestResult)GetValue(ShootTestResultProperty);
	        set => SetValue(ShootTestResultProperty, value);
        }

        public static readonly DependencyProperty ShootTestResultProperty =
            DependencyProperty.Register("ShootTestResult", typeof(ShootTestResult), typeof(EquivalentArmorView), new PropertyMetadata(null, EquivalentArmorView.OnShootTestResultChanged));

        private static void OnShootTestResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (EquivalentArmorView)d;
            view.OnShootTestResultChanged();
        }



        public TestShellInfo TestShell
        {
            get => (TestShellInfo)GetValue(TestShellProperty);
	        set => SetValue(TestShellProperty, value);
        }

        public static readonly DependencyProperty TestShellProperty =
            DependencyProperty.Register("TestShell", typeof(TestShellInfo), typeof(EquivalentArmorView), new PropertyMetadata(new TestShellInfo(Modeling.ShellType.Ap, 0.0)));



        internal EquivalentArmorViewModel ViewModel
        {
            get => (EquivalentArmorViewModel)GetValue(ViewModelProperty);
	        set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(object), typeof(EquivalentArmorView), new PropertyMetadata(null, EquivalentArmorView.OnViewModelChanged));

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldVm = (EquivalentArmorViewModel)e.OldValue;
            var newVm = (EquivalentArmorViewModel)e.NewValue;

            ((EquivalentArmorView)d).OnViewModelChanged(oldVm, newVm);
        }

        // TEST SHELL RACE CONDITION PROBLEM
        //   TestShell will be changed once the ViewModel is created, which happens
        //   after the database is loaded. While in the same frame, the binding to TestShell
        //   is set up with ModelViewModel. In such a case, ModelViewModel would not know 
        //   the new TestShell changed by ViewModel creation. So we repeat this change event
        //   in one frame later.
        private bool _isFirstViewModelChange;
        private bool _testShellRaceConditionHandlePending;

        private DispatcherOperation _updateShootTestResultTask;

        public EquivalentArmorView()
        {
            InitializeComponent();
            _isFirstViewModelChange = true;
        }

        private void OnViewModelChanged(EquivalentArmorViewModel oldVm, EquivalentArmorViewModel newVm)
        {
            if (oldVm != null)
                oldVm.ShellChanged -= ViewModel_ShellChanged;

            this.RootElement.DataContext = newVm;

            newVm.ShellChanged += ViewModel_ShellChanged;

            this.UpdateTestShell();
        }


        private void OnShootTestResultChanged()
        {
            if(_updateShootTestResultTask!=null)
                return;

            _updateShootTestResultTask = Dispatcher.BeginInvoke(new Action(this.UpdateViewModelShootTestResult), DispatcherPriority.Background);           
        }

        private void UpdateViewModelShootTestResult()
        {
            if (this.ViewModel != null)
            {

                this.ViewModel.BeginUpdateShootResult();

                if (this.ShootTestResult == null)
                {
                    this.ViewModel.PenetrationState = PenetrationState.NotApplicable;
                    this.ViewModel.ImpactAngle = 0;
                    this.ViewModel.NormalizationAngle = 0;
                    this.ViewModel.Is2XRuleActive = false;
                    this.ViewModel.Is3XRuleActive = false;
                }
                else
                {
                    this.ViewModel.PenetrationState = this.ShootTestResult.PenetrationState;
                    this.ViewModel.ImpactAngle = this.ShootTestResult.ImpactAngle;
                    this.ViewModel.NormalizationAngle = this.ShootTestResult.NormalizationAngle;
                    this.ViewModel.Is2XRuleActive = this.ShootTestResult.Is2XRuleActive;
                    this.ViewModel.Is3XRuleActive = this.ShootTestResult.Is3XRuleActive;
                    this.ViewModel.EquivalentThickness = this.ShootTestResult.EquivalentThickness;
                }

                this.ViewModel.EndUpdateShootResult();
            }

            _updateShootTestResultTask = null;
        }

        private void UpdateTestShell()
        {
            this.TestShell = new TestShellInfo(this.ViewModel.SelectedShellType, this.ViewModel.Caliber);

            // handle TEST SHELL RACE CONDITION PROBLEM
            if (_isFirstViewModelChange)
            {
                _isFirstViewModelChange = false;
                _testShellRaceConditionHandlePending = true;
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
        }

	    private void ViewModel_ShellChanged(object sender, EventArgs e)
        {
            this.UpdateTestShell();
        }

        // handle TEST SHELL RACE CONDITION PROBLEM
	    private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (_testShellRaceConditionHandlePending)
            {
                _testShellRaceConditionHandlePending = false;
                this.TestShell = this.TestShell;
            }

            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.ViewModel != null && !IsMouseOver)
                this.ViewModel.IsSelectShellViewShown = false;
        }
    }
}
