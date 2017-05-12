using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class ChangeAnimationContentControl : ContentControl
    {

        static ChangeAnimationContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChangeAnimationContentControl), new FrameworkPropertyMetadata(typeof(ChangeAnimationContentControl)));
        }

        private const string ContentContainerName = "ContentContainer";
        private const string ContentPresenter1Name = "ContentPresenter1";
        private const string ContentPresenter2Name = "ContentPresenter2";
        private const string ShowStoryboardName = "ShowStoryboard";
        private const string HideStoryboardName = "HideStoryboard";

        private Grid _contentContainer;
        private ContentPresenter _contentPresenter1;
        private ContentPresenter _contentPresenter2;

        private ContentPresenter _currentContentPresenter;
        private ContentPresenter _alternativeContentPresenter;

        private Storyboard _showStoryboard;
        private Storyboard _hideStoryboard;

        private bool _showStoryboardCompleted;
        private bool _hideStoryboardCompleted;
        private bool _isInTransition;



        private DispatcherOperation _changeAnimationTask;

        public ChangeAnimationContentControl()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _contentContainer = this.Template.FindName(ContentContainerName, this) as Grid;
            if (_contentContainer == null)
                throw new InvalidOperationException("invalid template, ContentContainer not found or invalid");

            _contentPresenter1 = this.Template.FindName(ContentPresenter1Name, this) as ContentPresenter;
            if (_contentPresenter1 == null)
                throw new InvalidOperationException("invalid template, ContentPresenter1 not found or invalid");

            _contentPresenter2 = this.Template.FindName(ContentPresenter2Name, this) as ContentPresenter;
            if (_contentPresenter2 == null)
                throw new InvalidOperationException("invalid template, ContentPresenter2 not found or invalid");

            BindingOperations.ClearBinding(_contentPresenter1, ContentPresenter.ContentProperty);
            BindingOperations.ClearBinding(_contentPresenter2, ContentPresenter.ContentProperty);

            _currentContentPresenter = _contentPresenter1;
            _alternativeContentPresenter = _contentPresenter2;

            _showStoryboard = this.TryFindResource(ShowStoryboardName) as Storyboard;
            if (_showStoryboard == null)
                _showStoryboard = _contentContainer.TryFindResource(ShowStoryboardName) as Storyboard;

            if (_showStoryboard != null)
                _showStoryboard = _showStoryboard.Clone();

            _hideStoryboard = this.TryFindResource(HideStoryboardName) as Storyboard;
            if (_hideStoryboard == null)
                _hideStoryboard = _contentContainer.TryFindResource(HideStoryboardName) as Storyboard;

            if (_hideStoryboard != null)
                _hideStoryboard = _hideStoryboard.Clone();

            _hideStoryboard.Begin(_contentPresenter2, true);
            _hideStoryboard.SkipToFill(_contentPresenter2);
        }


        private void SwapContentPresenters()
        {
            ObjectEx.Swap(ref _currentContentPresenter, ref _alternativeContentPresenter);
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {

            if (_currentContentPresenter == null)
                return;

            var valueChanged = !Equals(oldContent, newContent);
            if (valueChanged)
            {
                base.OnContentChanged(oldContent, newContent);

                if (_isInTransition || (_hideStoryboard == null && _showStoryboard == null))
                {
                    _currentContentPresenter.Content = newContent;
                }
                else
                {
                    if (_changeAnimationTask != null)
                        _changeAnimationTask.Abort();

                    _changeAnimationTask = Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (_hideStoryboard != null)
                            {
                                _hideStoryboard.CurrentStateInvalidated += _hideStoryboard_CurrentStateInvalidated;
                                _hideStoryboard.Begin(_currentContentPresenter, true);
                                _hideStoryboardCompleted = false;

                                this.SwapContentPresenters();
                            }

                            _currentContentPresenter.Content = newContent;

                            if (_showStoryboard != null)
                            {
                                _showStoryboard.CurrentStateInvalidated += _showStoryboard_CurrentStateInvalidated;
                                _showStoryboard.Begin(_currentContentPresenter, true);
                                _showStoryboardCompleted = false;
                            }

                            _isInTransition = true;

                            _changeAnimationTask = null;

                        }), DispatcherPriority.Background);

                }
            }

        }

	    private void _showStoryboard_CurrentStateInvalidated(object sender, EventArgs e)
        {
            var currentState = _showStoryboard.GetCurrentState(_currentContentPresenter);
            if (currentState == ClockState.Filling || currentState == ClockState.Stopped)
            {
                _showStoryboardCompleted = true;
                _isInTransition = !_showStoryboardCompleted || !_hideStoryboardCompleted;
                _showStoryboard.CurrentStateInvalidated -= _showStoryboard_CurrentStateInvalidated;
            }
        }

	    private void _hideStoryboard_CurrentStateInvalidated(object sender, EventArgs e)
        {
            var currentState = _hideStoryboard.GetCurrentState(_alternativeContentPresenter);
            if (currentState == ClockState.Filling || currentState == ClockState.Stopped)
            {
                _hideStoryboardCompleted = true;
                _isInTransition = !_showStoryboardCompleted || !_hideStoryboardCompleted;
                _hideStoryboard.CurrentStateInvalidated -= _hideStoryboard_CurrentStateInvalidated;
            }
        }


    }
}
