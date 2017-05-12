using System;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class GunVerticalTraverse : ICloneable
    {
        public PitchLimits DefaultPitchLimits { get; set; }

        public ExtraPitchLimits ExtraPitchLimits { get; set; }

        public PitchLimits Front
        {
            get
            {
	            if (this.HasExtraFrontPitchLimits)
                    return this.ExtraPitchLimits.Front;
	            return this.DefaultPitchLimits;
            }
        }

        [Stat("Depression", DataAnalysis.ComparisonMode.HigherBetter)]
        public double Depression => this.Front.Depression;

	    [Stat("Elevation", DataAnalysis.ComparisonMode.HigherBetter)]
        public double Elevation => this.Front.Elevation;

	    public bool HasExtraFrontPitchLimits => this.ExtraPitchLimits != null && this.ExtraPitchLimits.Front != null;

	    public bool HasExtraBackPitchLimits => this.ExtraPitchLimits != null && this.ExtraPitchLimits.Back != null;

	    public PitchLimits GetPitchLimits(double degree)
	    {
		    if (this.DefaultPitchLimits.IsPost909Format)
                return this.GetPitchLimitsPost909(degree);
		    return this.GetPitchLimitsPre909(degree);
	    }

        private PitchLimits GetPitchLimitsPost909(double degree)
        {
            degree %= 360;

            if (degree < 0)
                degree += 360;

            degree /= 360;

            return new PitchLimits(this.InterpolatePitchLimit(degree, this.DefaultPitchLimits.ElevationData),
                                   this.InterpolatePitchLimit(degree, this.DefaultPitchLimits.DepressionData));
        }

        private double InterpolatePitchLimit(double progress, PitchLimits.Component[] components)
        {
            PitchLimits.Component? from = null;
            PitchLimits.Component? to = null;
            for (var i = 0; i < components.Length - 1; ++i)
            {
                if (components[i].Angle <= progress && components[i + 1].Angle > progress)
                {
                    from = components[i];
                    to = components[i + 1];
                    break;
                }
            }

            if (from == null)
                throw new InvalidOperationException("invalid pitch data");

            var v1 = from.Value.Limit;
            var v2 = to.Value.Limit;
            var a1 = from.Value.Angle;
            var a2 = to.Value.Angle;
            var ax = progress;

            return v1 + (ax - a1) * (v2 - v1) / (a2 - a1);
        } 

        private PitchLimits GetPitchLimitsPre909(double degree)
        {
            degree %= 360;

            if (degree < 0)
                degree += 360;

            // assume symmetry
            if (degree > 180)
                degree = 360 - degree;

            if (this.HasExtraFrontPitchLimits)
            {
	            if (degree <= this.ExtraPitchLimits.FrontLimitsRightBound)
                    return this.ExtraPitchLimits.Front;
	            if (degree <= this.ExtraPitchLimits.FrontTransitionRightBound)
	            {
		            var progress = (degree - this.ExtraPitchLimits.FrontLimitsRightBound) / (this.ExtraPitchLimits.FrontTransitionRightBound - this.ExtraPitchLimits.FrontLimitsRightBound);
		            return new PitchLimits(this.ExtraPitchLimits.Front.Elevation + (this.DefaultPitchLimits.Elevation - this.ExtraPitchLimits.Front.Elevation) * progress,
			            this.ExtraPitchLimits.Front.Depression + (this.DefaultPitchLimits.Depression - this.ExtraPitchLimits.Front.Depression) * progress);
	            }
            }

            if (this.HasExtraBackPitchLimits)
            {
	            if (degree >= this.ExtraPitchLimits.BackLimitsRightBound)
                    return this.ExtraPitchLimits.Back;
	            if (degree >= this.ExtraPitchLimits.BackTransitionRightBound)
	            {
		            var progress = (degree - this.ExtraPitchLimits.BackTransitionRightBound) / (this.ExtraPitchLimits.BackLimitsRightBound - this.ExtraPitchLimits.BackTransitionRightBound);
		            return new PitchLimits(this.DefaultPitchLimits.Elevation + (this.ExtraPitchLimits.Back.Elevation - this.DefaultPitchLimits.Elevation) * progress,
			            this.DefaultPitchLimits.Depression + (this.ExtraPitchLimits.Back.Depression - this.DefaultPitchLimits.Depression) * progress);
	            }
            }

            return this.DefaultPitchLimits;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public GunVerticalTraverse Clone()
        {
            return (GunVerticalTraverse)this.MemberwiseClone();
        }

        public override string ToString()
        {
            return $"Gun Vertical Traverse: Depression = {this.Depression}, Elevation = {this.Elevation}";
        }
    }
}
