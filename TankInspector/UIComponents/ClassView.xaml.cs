using Smellyriver.TankInspector.Design;
using Smellyriver.TankInspector.Modeling;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for ClassView.xaml
    /// </summary>
    public partial class ClassView : UserControl
    {

        internal TankClass Class
        {
            get => (TankClass)GetValue(ClassProperty);
	        set => SetValue(ClassProperty, value);
        }

        public static readonly DependencyProperty ClassProperty =
            DependencyProperty.Register("Class", typeof(TankClass), typeof(ClassView), new PropertyMetadata(TankClass.HeavyTank, ClassView.OnClassChanged));

        private static void OnClassChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (ClassView)d;
            view.UpdateClass();
        }

        public ClassView()
        {
            InitializeComponent();
            this.UpdateClass();
        }

        private void UpdateClass()
        {
            this.ClassIcon.Source = (ImageSource)new TankClassIconConverter { Type = TankClassConversionType.Small }.Convert(new object[] { this.Class, false }, null, null, null);
            this.ClassText.Text = (string)new TankClassNameConverter().Convert(this.Class, null, null, null);
        }
    }
}
