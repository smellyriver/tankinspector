using Smellyriver.TankInspector.Design;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for TierView.xaml
    /// </summary>
    public partial class TierView : UserControl
    {


        internal int Tier
        {
            get => (int)GetValue(TierProperty);
	        set => SetValue(TierProperty, value);
        }

        public static readonly DependencyProperty TierProperty =
            DependencyProperty.Register("Tier", typeof(int), typeof(TierView), new PropertyMetadata(1, TierView.OnTierChanged));

        private static void OnTierChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (TierView)d;
            view.UpdateTierIcon();
        }

        public TierView()
        {
            InitializeComponent();
            this.UpdateTierIcon();
        }

        private void UpdateTierIcon()
        {
            this.TierIcon.Source = (ImageSource)new TankTierIconConverter { Type = TankTierIconType.Heavy }.Convert(this.Tier, null, null, null);
        }
    }
}
