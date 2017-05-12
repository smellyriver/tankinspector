using System.Windows;
using System.Windows.Controls;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for DetailedDataGroupView.xaml
    /// </summary>
    public partial class DetailedDataGroupView : UserControl
    {



        public int MinimumPriority
        {
            get => (int)GetValue(MinimumPriorityProperty);
	        set => SetValue(MinimumPriorityProperty, value);
        }

        public static readonly DependencyProperty MinimumPriorityProperty =
            DependencyProperty.Register("MinimumPriority", typeof(int), typeof(DetailedDataGroupView), new PropertyMetadata(0));



        internal DataViewModel ViewModel
        {
            get => this.DataContext as DataViewModel;
	        set => this.DataContext = value;
        }

        internal TankViewModelBase ConstantComparedTank
        {
            get => (TankViewModelBase)GetValue(ConstantComparedTankProperty);
	        set => SetValue(ConstantComparedTankProperty, value);
        }

        public static readonly DependencyProperty ConstantComparedTankProperty =
            DependencyProperty.Register("ConstantComparedTank", typeof(object), typeof(DetailedDataGroupView), new PropertyMetadata(null, DetailedDataGroupView.OnConstantComparedTankChanged));

        private static void OnConstantComparedTankChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Equals(e.NewValue, e.OldValue))
                return;

            var priorityDataView = (DetailedDataGroupView)d;

            var tank = (TankViewModelBase)e.NewValue;

            priorityDataView.ViewModel.CompareTo(tank);
        }

        public DetailedDataGroupView()
        {
            InitializeComponent();
        }

        private void DataToolTip_Opened(object sender, RoutedEventArgs e)
        {
            var tip = (ToolTip)sender;
            tip.UpdateLayout();
        }
    }
}
