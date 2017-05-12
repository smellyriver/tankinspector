using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Smellyriver.Utilities;
using System.Windows.Markup;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for SniperView.xaml
    /// </summary>
    /// 
    [ContentProperty("ModelView")]
    public partial class SniperModeDecorator : UserControl
    {
        private const double MaximumDistance = 707.0;

        private const double MinimumDistance = 7;

        public double MoveSpeed
        {
            get => (double)GetValue(MoveSpeedProperty);
	        set => SetValue(MoveSpeedProperty, value);
        }

        public static readonly DependencyProperty MoveSpeedProperty =
            DependencyProperty.Register("MoveSpeed", typeof(double), typeof(SniperModeDecorator), new PropertyMetadata(30.0, null, SniperModeDecorator.OnCoerceMoveSpped));

        private static object OnCoerceMoveSpped(DependencyObject d, object baseValue)
        {
            return Math.Max((double)baseValue, 0.0);
        }


        public double Distance
        {
            get => (double)GetValue(DistanceProperty);
	        set => SetValue(DistanceProperty, value);
        }

        public static readonly DependencyProperty DistanceProperty =
            DependencyProperty.Register("Distance", typeof(double), typeof(SniperModeDecorator), new PropertyMetadata(100.0, null, SniperModeDecorator.OnCoerceDistance));

        private static object OnCoerceDistance(DependencyObject d, object baseValue)
        {
            return ((double)baseValue).Clamp(MinimumDistance, MaximumDistance);
        }

        public ModelView ModelView
        {
            get => (ModelView)GetValue(ModelViewProperty);
	        set => SetValue(ModelViewProperty, value);
        }

        public static readonly DependencyProperty ModelViewProperty =
            DependencyProperty.Register("ModelView", typeof(UIElement), typeof(SniperModeDecorator), new PropertyMetadata(null, SniperModeDecorator.OnModelViewChanged));

        private static void OnModelViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SniperModeDecorator)d).OnModelViewChanged((ModelView)e.OldValue, (ModelView)e.NewValue);
        }



        public bool IsInSniperMode
        {
            get => (bool)GetValue(IsInSniperModeProperty);
	        set => SetValue(IsInSniperModeProperty, value);
        }

        public static readonly DependencyProperty IsInSniperModeProperty =
            DependencyProperty.Register("IsInSniperMode", typeof(bool), typeof(SniperModeDecorator), new PropertyMetadata(false, SniperModeDecorator.OnIsInSniperModeChanged));

        private static void OnIsInSniperModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SniperModeDecorator)d).OnIsInSniperModeChanged((bool)e.OldValue, (bool)e.NewValue);
        }



        private enum MoveStates { Stopped = 0, Forward = -1, Reverse = 1 }

        private MoveStates _moveState;
        
        private DateTime _previousUpdateTime;


        private Key _previousKey;
        private DateTime _previousKeyTime;
        private double _sprintFactor = 1.0;
        public SniperModeDecorator()
        {
            InitializeComponent();
            _previousUpdateTime = DateTime.Now;
            this.Distance = ApplicationSettings.Default.SniperModeDistance;

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void OnIsInSniperModeChanged(bool oldValue, bool newValue)
        {
            if(oldValue && !newValue)
            {
                ApplicationSettings.Default.SniperModeDistance = this.Distance;
                ApplicationSettings.Default.Save();
            }
        }

        private void OnModelViewChanged(ModelView oldChild, ModelView newChild)
        {
            this.ChildContainer.Children.Clear();

            if (newChild != null)
            {
                this.ChildContainer.Children.Add(newChild);

                Binding binding = new Binding("DataContext.Distance");
                binding.Source = newChild;
                binding.Mode = BindingMode.OneWayToSource;
                this.SetBinding(DistanceProperty, binding);
                // the SetBinding step will override this.Distance with its default value, we need to set it back again
                this.Distance = ApplicationSettings.Default.SniperModeDistance;
            }
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            var deltaTime = DateTime.Now - _previousUpdateTime;
            _previousUpdateTime = DateTime.Now;

            var distanceFactor = Math.Pow(Math.Tanh((this.Distance / 100) * Math.PI), 2.0);

            var deltaDistance = deltaTime.TotalSeconds * this.MoveSpeed * _sprintFactor * (int)_moveState * distanceFactor;

            var distance = this.Distance + deltaDistance;

            this.Distance = distance.Clamp(MinimumDistance, MaximumDistance);
        }

        private void BeginStoryboard(string resourceKey, FrameworkElement target)
        {
            var storyboard = this.FindResource(resourceKey) as Storyboard;
            storyboard.Begin(target);
        }

        private void TryEnterSpringMode(Key currentKey)
        {
            if (_previousKey == currentKey && (DateTime.Now - _previousKeyTime).TotalMilliseconds < 200)
                _sprintFactor += 10.0;
            else
                _sprintFactor = 1.0;
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!this.IsInSniperMode)
                return;

            if (e.IsRepeat)
                return;

            switch (e.Key)
            {
                case Key.W:
                    this.TryEnterSpringMode(Key.W);

                    this.BeginMoveForward();
                    e.Handled = true;
                    break;
                case Key.S:
                    this.TryEnterSpringMode(Key.S);

                    this.BeginMoveBackward();
                    e.Handled = true;
                    break;
            }

            _previousKey = e.Key;
            _previousKeyTime = DateTime.Now;
        }

        private void UserControl_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (!this.IsInSniperMode)
                return;

            switch (e.Key)
            {
                case Key.W:
                    _sprintFactor = 1.0;

                    this.EndMoveForward();
                    e.Handled = true;
                    break;
                case Key.S:
                    _sprintFactor = 1.0;

                    this.EndMoveBackward();
                    e.Handled = true;
                    break;
            }
        }

        private void BeginMoveBackward()
        {
            if (_moveState == MoveStates.Forward)
                this.EndMoveForward();

            _moveState = MoveStates.Reverse;
            this.BeginStoryboard("HighlightMoveKeyIndicator", this.ReverseKeyIndicator);
        }

        private void BeginMoveForward()
        {
            if (_moveState == MoveStates.Reverse)
                this.EndMoveBackward();

            _moveState = MoveStates.Forward;
            this.BeginStoryboard("HighlightMoveKeyIndicator", this.ForwardKeyIndicator);
        }

        private void EndMoveBackward()
        {
            _moveState = MoveStates.Stopped;
            this.BeginStoryboard("ResetMoveKeyIndicator", this.ReverseKeyIndicator);
        }

        private void EndMoveForward()
        {
            _moveState = MoveStates.Stopped;
            this.BeginStoryboard("ResetMoveKeyIndicator", this.ForwardKeyIndicator);
        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsInSniperMode)
                return;

            if (e.ChangedButton == MouseButton.Left)
                this.TelescopeView.PlayShootFlareEffect();
        }
    }
}
