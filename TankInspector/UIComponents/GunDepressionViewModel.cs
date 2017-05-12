using Smellyriver.TankInspector.Modeling;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class GunDepressionViewModel: GunVerticalTraverseComponentViewModel
    {
        public GunDepressionViewModel(GunVerticalTraverse vertical, HorizontalTraverse horizontal)
            : base(vertical, horizontal)
        {

        }

        protected override double GetValue(PitchLimits pitchLimits)
        {
            return pitchLimits.Depression;
        }

        protected override PitchLimits.Component[] GetPitchLimitComponents(PitchLimits pitchLimits)
        {
            return pitchLimits.DepressionData;
        }

        public override bool InverseFigure => true;
    }
}
