using System;
using System.Windows.Markup;

namespace Smellyriver.TankInspector.Design
{
    public class RomanNumberExtension : MarkupExtension
    {
        public int Number { get; set; }

        public RomanNumberExtension(int number)
        {
            this.Number = number;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return RomanNumberService.GetRomanNumber(this.Number);
        }
    }
}
