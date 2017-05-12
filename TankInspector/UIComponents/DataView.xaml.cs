using System.Windows;
using System.Windows.Controls;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class DataView : UserControl
    {

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
            DependencyProperty.Register("ConstantComparedTank", typeof(object), typeof(DataView), new PropertyMetadata(null, DataView.OnConstantComparedTankChanged));

        private static void OnConstantComparedTankChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Equals(e.NewValue, e.OldValue))
                return;

            var dataview = (DataView)d;

            var tank = (TankViewModelBase)e.NewValue;

            dataview.ViewModel.CompareTo(tank);
        }

        public DataView()
        {
            InitializeComponent();
        }
    }
}
