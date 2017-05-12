using System;
using System.Windows.Markup;

namespace Smellyriver.TankInspector.Design
{
	internal class LocalizationExtension : MarkupExtension
    {
        public string Key { get; set; }

        public LocalizationExtension(string key)
        {
            this.Key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return App.GetLocalizedString(this.Key);
        }
    }
}
