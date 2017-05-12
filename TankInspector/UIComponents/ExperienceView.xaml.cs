using Smellyriver.TankInspector.Design;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for ExperienceView.xaml
    /// </summary>
    public partial class ExperienceView : UserControl
    {
        
        internal double Experience
        {
            get => (double)GetValue(ExperienceProperty);
	        set => SetValue(ExperienceProperty, value);
        }

        public static readonly DependencyProperty ExperienceProperty =
            DependencyProperty.Register("Experience", typeof(double), typeof(ExperienceView), new PropertyMetadata(0.0, ExperienceView.OnExperienceChanged));

        private static void OnExperienceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (ExperienceView)d;
            view.UpdateExperienceText();
        }

        internal ExperienceType ExperienceType
        {
            get => (ExperienceType)GetValue(ExperienceTypeProperty);
	        set => SetValue(ExperienceTypeProperty, value);
        }

        public static readonly DependencyProperty ExperienceTypeProperty =
            DependencyProperty.Register("CurrencyType", typeof(ExperienceType), typeof(ExperienceView), new PropertyMetadata(ExperienceType.Vehicle, ExperienceView.OnExperienceTypeChanged));

        private static void OnExperienceTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (ExperienceView)d;
            view.UpdateExperienceIcon();
        }

        public ExperienceView()
        {
            InitializeComponent();
            this.UpdateExperienceIcon();
            this.UpdateExperienceText();
        }

        private void UpdateExperienceText()
        {
            this.ExperienceText.Text = this.Experience.ToString("#,0");
        }

        private void UpdateExperienceIcon()
        {
            this.ExperienceIcon.Source = (ImageSource)new ExperienceIconConverter().Convert(this.ExperienceType, null, null, null);
        }
    }
}
