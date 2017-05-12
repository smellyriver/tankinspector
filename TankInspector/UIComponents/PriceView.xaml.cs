using Smellyriver.TankInspector.Design;
using Smellyriver.TankInspector.Modeling;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for PriceView.xaml
    /// </summary>
    public partial class PriceView : UserControl
    {

        internal double Price
        {
            get => (double)GetValue(PriceProperty);
	        set => SetValue(PriceProperty, value);
        }

        public static readonly DependencyProperty PriceProperty =
            DependencyProperty.Register("Price", typeof(double), typeof(PriceView), new PropertyMetadata(0.0, PriceView.OnPriceChanged));

        private static void OnPriceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (PriceView)d;
            view.UpdatePriceText();
        }

        internal CurrencyType CurrencyType
        {
            get => (CurrencyType)GetValue(CurrencyTypeProperty);
	        set => SetValue(CurrencyTypeProperty, value);
        }

        public static readonly DependencyProperty CurrencyTypeProperty =
            DependencyProperty.Register("CurrencyType", typeof(CurrencyType), typeof(PriceView), new PropertyMetadata(CurrencyType.Credit, PriceView.OnCurrencyTypeChanged));

        private static void OnCurrencyTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (PriceView)d;
            view.UpdateCurrencyIcon();
        }


        public PriceView()
        {
            InitializeComponent();

            this.UpdateCurrencyIcon();
            this.UpdatePriceText();
        }

        private void UpdatePriceText()
        {        
            this.PriceText.Text = this.Price.ToString("#,0");
        }

        private void UpdateCurrencyIcon()
        {
            this.CurrencyIcon.Source = (ImageSource)new CurrencyIconConverter().Convert(this.CurrencyType, null, null, null);
        }
    }
}
