using System;

namespace Smellyriver.TankInspector.Graphics.Scene
{
    public class ShootTestResult
    {

        public static readonly ShootTestResult NotApplicable = new ShootTestResult(PenetrationState.NotApplicable, 0, 0, 0, false, false);
        public int EquivalentThickness { get; }
        public PenetrationState PenetrationState { get; }
        public int ImpactAngle { get; }
        public int NormalizationAngle { get; }
        public bool Is2XRuleActive { get; }
        public bool Is3XRuleActive { get; }

        public ShootTestResult(PenetrationState state, double equivalentThickness, double impactAngle, double normalizationAngle, bool is2XRuleActive, bool is3XRuleActive)
        {
            this.PenetrationState = state;
            this.EquivalentThickness = (int)Math.Round(equivalentThickness);
            this.ImpactAngle = (int)Math.Round(impactAngle);
            this.NormalizationAngle = (int)Math.Round(normalizationAngle);
            this.Is2XRuleActive = is2XRuleActive;
            this.Is3XRuleActive = is3XRuleActive;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ShootTestResult;
            if (other == null)
                return false;

            return this.EquivalentThickness == other.EquivalentThickness
                && this.PenetrationState == other.PenetrationState
                && this.ImpactAngle == other.ImpactAngle
                && this.NormalizationAngle == other.NormalizationAngle
                && this.Is2XRuleActive == other.Is2XRuleActive
                && this.Is3XRuleActive == other.Is3XRuleActive;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
