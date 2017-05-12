using Smellyriver.TankInspector.Design;
using Smellyriver.TankInspector.Modeling;
using System.Text;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class BattleTiersViewModel
    {

        

        public int From { get; }
        public int To { get; }

        private readonly string _toStringResult;

        public BattleTiersViewModel(BattleTierSpan battleTiers)
        {
            this.From = battleTiers.From;
            this.To = battleTiers.To;

            if (battleTiers.To - battleTiers.From > 2)
                _toStringResult =
		                $"{RomanNumberService.GetRomanNumber(battleTiers.From)}-{RomanNumberService.GetRomanNumber(battleTiers.To)}";
            else
            {
                var builder = new StringBuilder();
                for (int i = battleTiers.From; i <= battleTiers.To; ++i)
                {
                    if (i != battleTiers.From)
                        builder.Append(' ');

                    builder.Append(RomanNumberService.GetRomanNumber(i));
                }
                _toStringResult = builder.ToString();
            }
        }

        public override string ToString()
        {
            return _toStringResult;
        }

    }
}
