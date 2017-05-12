using Smellyriver.Wpf;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class HelpService : DependencyNotificationObject
    {
        public static HelpService Instance { get; }

        private static readonly Dictionary<object, ItemsControl> ItemsControls = new Dictionary<object, ItemsControl>();
        static HelpService()
        {
            HelpService.Instance = new HelpService();
            HelpService.Instance.IsHelpEnabled = true;
        }



        public static object GetHelpContent(DependencyObject obj)
        {
            return (object)obj.GetValue(HelpContentProperty);
        }

        public static void SetHelpContent(DependencyObject obj, object value)
        {
            obj.SetValue(HelpContentProperty, value);
        }

        public static readonly DependencyProperty HelpContentProperty =
            DependencyProperty.RegisterAttached("HelpContent", typeof(object), typeof(HelpService), new PropertyMetadata(null, HelpService.OnHelpContentChanged));


        private static void OnHelpContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (Control)d;
            control.Loaded += HelpService.Control_Loaded;
        }

        private static void Control_Loaded(object sender, RoutedEventArgs e)
        {
            var control = (UIElement)sender;
            var layer = AdornerLayer.GetAdornerLayer(control);

            if (layer != null)
            {
                layer.Add(new HelpAdorner(control, HelpService.GetHelpContent(control)));
            }
        }

        private bool _isHelpEnabled;
        public bool IsHelpEnabled
        {
            get => _isHelpEnabled;
	        set
            {
                _isHelpEnabled = value;
                this.RaisePropertyChanged(() => this.IsHelpEnabled);
            }
        }

        private HelpService()
        {

        }
    }
}
