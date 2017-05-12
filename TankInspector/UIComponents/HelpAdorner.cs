using Smellyriver.Wpf.Converters;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class HelpAdorner : Adorner
    {
        private readonly ContentPresenter _contentPresenter;

        public HelpAdorner(UIElement adornedElement, object content)
            : base(adornedElement)
        {
            this.IsHitTestVisible = false;

            _contentPresenter = new ContentPresenter();
            _contentPresenter.Content = content;


            var multiBinding = new MultiBinding();
            multiBinding.Converter = new MultiValueConverterChain(
                new MultiValueConverter(bools => bools.Cast<bool>().All(b => b)), 
                new BooleanToVisibilityConverter());

            var binding = new Binding("IsHelpEnabled");
            binding.Source = HelpService.Instance;
            multiBinding.Bindings.Add(binding);

            binding = new Binding("IsVisible");
            binding.Source = adornedElement;
            multiBinding.Bindings.Add(binding);

            this.SetBinding(VisibilityProperty, multiBinding);

            binding = new Binding("Opacity");
            binding.Source = adornedElement;
            this.SetBinding(OpacityProperty, binding);

        }

        protected override int VisualChildrenCount => 1;

	    protected override Visual GetVisualChild(int index)
        {
            return _contentPresenter;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _contentPresenter.Measure(this.AdornedElement.RenderSize);
            return this.AdornedElement.RenderSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _contentPresenter.Arrange(new Rect(finalSize));
            return finalSize;
        }
    }
}
