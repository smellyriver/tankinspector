using Smellyriver.TankInspector.Design;
using Smellyriver.Wpf.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for DetailedDataView.xaml
    /// </summary>
    public partial class DetailedDataView : UserControl
    {
        private static readonly IValueConverter SHalfConverter = new HalfConverter();
        private static IMultiValueConverter _sMinusConverter = new MultiValueConverter(values => (double)values[0] - (double)values[1]);


        private static bool IsMouseOverElement(FrameworkElement dataView)
        {
            var position = Mouse.GetPosition(dataView);
            if (position.X < 0 || position.Y < 0 || position.X > dataView.ActualWidth || position.Y > dataView.ActualHeight)
                return false;

            return true;
        }

        private static DetailedDataGroupView GetCounterpart(DetailedDataGroupView target)
        {
            var panel = VisualTreeHelper.GetParent(target) as Panel;
            foreach (UIElement child in panel.Children)
                if (child != target)
                    return (DetailedDataGroupView)child;

            return null;
        }


        private DetailedDataGroupView _previousMouseOverDataView;
        private readonly DetailedDataGroupView[] _dataViews;

        private DispatcherTimer _dataViewUpdateTimer;

        // after configuring the tank with the menu in the bottom, people may want to stick to the 
        // changed stats for a while. so do not highlight any data view until the mouse is moved
        private bool _mouseEnteredJustNow;

        private int _defaultDataPriority;

        public DetailedDataView()
        {
            InitializeComponent();

            _dataViews = new[]
                {
                    this.FirePowerDataGroupView,
                    this.ScoutabilityDataGroupView,
                    this.ManeuverabilityDataGroupView,
                    this.BattleDataGroupView,
                    this.MobilityDataGroupView,
                    this.EconomyDataGroupView,
                    this.SurvivabilityDataGroupView,
                    this.MiscellaneousDataGroupView
                };

            this.Loaded += DetailedDataView_Loaded;

            ApplicationSettings.Default.PropertyChanged += ApplicationSettings_PropertyChanged;
            _defaultDataPriority = this.GetDefaultDataPriority(ApplicationSettings.Default.ShowAsMuchStatsAsPossible);
        }

        private int GetDefaultDataPriority(bool showAsMuchStatsAsPossible)
        {
	        if (showAsMuchStatsAsPossible)
                return (int)DetailedDataViewModel.DisplayPriority.All;
	        return (int)DetailedDataViewModel.DisplayPriority.NotThatImportant;
        }

	    private void ApplicationSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
            if(e.PropertyName == "ShowAsMuchStatsAsPossible")
            {
                var newDefaultDataPriority = this.GetDefaultDataPriority(ApplicationSettings.Default.ShowAsMuchStatsAsPossible);

                foreach(var dataView in _dataViews)
                {
                    if (dataView.MinimumPriority == _defaultDataPriority)
                        dataView.MinimumPriority = newDefaultDataPriority;
                }

                _defaultDataPriority = newDefaultDataPriority;
            }
        }

	    private void DetailedDataView_Loaded(object sender, RoutedEventArgs e)
        {
            _dataViewUpdateTimer = new DispatcherTimer(DispatcherPriority.Background);
            _dataViewUpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            _dataViewUpdateTimer.Tick += _dataViewUpdateTimer_Tick;
            _dataViewUpdateTimer.Start();
        }

	    private void _dataViewUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (!DetailedDataView.IsMouseOverElement(this) && this.DataContext != null)
                this.UpdateDataViewsMouseOverState();
        }

        private void NotifyDataViewMouseEnter(DetailedDataGroupView dataView)
        {
            var counterPart = DetailedDataView.GetCounterpart(dataView);

            Panel.SetZIndex(dataView, 1);
            if (counterPart != null)
                Panel.SetZIndex(counterPart, 0);

            dataView.MinimumPriority = (int)DetailedDataViewModel.DisplayPriority.DefaultHidden;
            dataView.Height = double.NaN;
        }

        private void NotifyDataViewMouseLeave(DetailedDataGroupView dataView)
        {
            var counterPart = DetailedDataView.GetCounterpart(dataView);
            dataView.MinimumPriority = _defaultDataPriority;

            var binding = new Binding("ActualHeight");
            binding.Converter = SHalfConverter;
            binding.Source = VisualTreeHelper.GetParent(dataView);

            dataView.SetBinding(MinHeightProperty, binding);
            dataView.SetBinding(HeightProperty, binding);
        }


        private void UpdateDataViewsMouseOverState()
        {
            // mouse might be over 2 data views at the same time (e.g. an expanded upper view and a covered lower view)
            var mouseOverDataViews = new List<DetailedDataGroupView>(2);

            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && this.IsMouseOver)
            {
                foreach (var dataView in _dataViews)
                {
                    if (DetailedDataView.IsMouseOverElement(dataView))
                    {
                        mouseOverDataViews.Add(dataView);

                        if (mouseOverDataViews.Count == 2)
                            break;
                    }
                }
            }

            if (mouseOverDataViews.Count == 0)
            {
                if (_previousMouseOverDataView != null)
                    this.NotifyDataViewMouseLeave(_previousMouseOverDataView);

                _previousMouseOverDataView = null;

                foreach (var dataView in _dataViews)
                    this.BrightenDataView(dataView);
            }
            else
            {
                var mouseOverDataView = mouseOverDataViews.OrderByDescending(v => Panel.GetZIndex(v)).First();

                if (_previousMouseOverDataView == mouseOverDataView)
                    return;

                if (_previousMouseOverDataView != null)
                    this.NotifyDataViewMouseLeave(_previousMouseOverDataView);

                this.NotifyDataViewMouseEnter(mouseOverDataView);
                _previousMouseOverDataView = mouseOverDataView;


                foreach (var dataView in _dataViews)
                {
                    if (dataView == mouseOverDataView)
                        this.BrightenDataView(dataView);
                    else
                        this.DarkenDataView(dataView);
                }
            }
        }

        private void DarkenDataView(DetailedDataGroupView dataView)
        {
            dataView.ViewModel.IsDarken = true;
        }

        private void BrightenDataView(DetailedDataGroupView dataView)
        {
            dataView.ViewModel.IsDarken = false;
        }

        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseEnteredJustNow)
            {
                _mouseEnteredJustNow = false;
                return;
            }

            this.UpdateDataViewsMouseOverState();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            _mouseEnteredJustNow = true;
        }

    }
}
