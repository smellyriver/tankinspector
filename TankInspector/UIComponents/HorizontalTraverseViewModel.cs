using Smellyriver.TankInspector.DataAnalysis;
using Smellyriver.TankInspector.Modeling;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class HorizontalTraverseViewModel : ICustomData, IValueTipContentProvider
    {
        public double Left { get; }
        public double Right { get; }

        public double Range => this.Left + this.Right;

	    public HorizontalTraverseViewModel(HorizontalTraverse horizontalTraverse)
        {
            if (horizontalTraverse == null)
            {
                this.Left = 0;
                this.Right = 0;
            }
            else
            {
                this.Left = horizontalTraverse.Left;
                this.Right = horizontalTraverse.Right;
            }
        }

        private HorizontalTraverseViewModel(double left, double right)
        {
            this.Left = left;
            this.Right = right;
        }

        public override string ToString()
        {
            return $"{this.Range:0.#}";
        }

        public string ToString(bool explicitSign)
        {
	        if (explicitSign && this.Range > 0)
                return "+" + this.ToString();
	        return this.ToString();
        }


        public int CompareTo(HorizontalTraverseViewModel other)
        {
            return (int)Math.Round(this.Range - other.Range);
        }

        public object Subtract(object other)
        {
            var otherTraverse = other as HorizontalTraverseViewModel;
            if (otherTraverse == null)
                return this;

            return new HorizontalTraverseViewModel(this.Left - otherTraverse.Left, this.Right - otherTraverse.Right);
        }

        public double GetDeltaRatio(object delta)
        {
            var deltaTraverse = delta as HorizontalTraverseViewModel;
            if (deltaTraverse == null)
                return 0.0;

            return deltaTraverse.Range / Math.Max(this.Range, 1.0);
        }

        public IEnumerable<Inline> ValueTipContent
        {
            get
            {
                var degree = App.GetLocalizedString("UnitDegrees");
                return new[]
                {
                    new Run(string.Format(degree, this.Range.ToString())) { FontSize = 26 },
                    new Run("\n"),
                    new Run(string.Format(degree, $"-{this.Left:0.#}")) { FontSize = DataItemDescriptorBase.StandardTipContentFontSize},
                    new Run(" ~ ") { FontSize = DataItemDescriptorBase.StandardTipContentFontSize},
                    new Run(string.Format(degree, $"+{this.Right:0.#}")) { FontSize = DataItemDescriptorBase.StandardTipContentFontSize},
                };
            }
        }

        public double MaxHeight => double.PositiveInfinity;

	    public bool CanCompare => true;
    }
}
