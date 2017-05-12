using Smellyriver.TankInspector.Modeling;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class GunElevationViewModel : GunVerticalTraverseComponentViewModel
    {
        public GunElevationViewModel(GunVerticalTraverse vertical, HorizontalTraverse horizontal)
            : base(vertical, horizontal)
        {

        }

        protected override double GetValue(PitchLimits pitchLimits)
        {
            return pitchLimits.Elevation;
        }

        protected override PitchLimits.Component[] GetPitchLimitComponents(PitchLimits pitchLimits)
        {
            return pitchLimits.ElevationData;
        }

        public override bool InverseFigure => false;
    }
}
