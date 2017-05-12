using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using Smellyriver.TankInspector.Graphics.Frameworks;
using Smellyriver.TankInspector.Modeling;

namespace Smellyriver.TankInspector.Graphics.Scene
{
	internal struct CollisionModelHit
    {
        public float Distance;
        public ArmorGroup Armor;
        public float InjectionCosine { get; set; }
        public TankMesh Mesh { get; set; }
    }

	internal class CollisionModelHitTestResult
    {
        public List<CollisionModelHit> Hits = new List<CollisionModelHit>();

	    public CollisionModelHitTestResult(Ray ray)
        {
            this.HitRay = ray;
        }

        public Ray HitRay { get; }

	    public CollisionModelHit? ClosesetSpacingArmorHit
        {
            get
            {
                if (Hits.Any(a => a.Armor.IsSpacingArmor))
                    return Hits.Where(a => a.Armor.IsSpacingArmor).Aggregate((a, b) => a.Distance > b.Distance ? b : a);
                return null;
            }
        }

        public CollisionModelHit? ClosesetRegularArmorHit
        {
            get
            {
                if (Hits.Any(a => !a.Armor.IsSpacingArmor))
                    return Hits.Where(a => !a.Armor.IsSpacingArmor).Aggregate((a, b) => a.Distance > b.Distance ? b : a);
                return null;
            }
        }

        public CollisionModelHit? ClosesetArmorHit
        {
            get
            {
                if (Hits.Count == 0) return null;
                return Hits.Aggregate((a, b) => a.Distance > b.Distance ? b : a);
            }
        }

        public float? EquivalentThickness
        {
            get
            {
                float thickness = 0.0f;

                var orderedHits = Hits.OrderBy((h) => h.Distance);

                foreach (var hit in orderedHits)
                {
                    if (hit.Armor.UseHitAngle)
                    {
                        thickness += (float)hit.Armor.Value / hit.InjectionCosine;
                    }
                    else
                    {
                        thickness += (float)hit.Armor.Value;
                    }

                    if (!hit.Armor.IsSpacingArmor)
                    {
                        return thickness;
                    }
                }
                return null;
            }
        }

        public IOrderedEnumerable<CollisionModelHit> OrderedHits
        {
            get { return Hits.OrderBy((h) => h.Distance); }
        }

        public ShootTestResult GetShootTestResult(TestShellInfo testShell)
        {
            PenetrationState penetrationState = PenetrationState.NotApplicable;
            double equivalentThickness = 0.0;
            double impactAngle = 0.0;
            double nomarlizationAngle = 0.0;
            bool is2X = false;
            bool is3X = false;
            bool mayRicochet = true;

            bool heatExploded = false;
            double heatExplodedDistance = 0.0;

            bool isFirstHit = true;
            var orderedHits = Hits.OrderBy((h) => h.Distance);

            foreach (var hit in orderedHits)
            {
                var thisImpactAngle = DxUtils.ConvertRadiansToDegrees(Math.Acos(hit.InjectionCosine));

                if (isFirstHit)
                {
                    impactAngle = thisImpactAngle;
                }

                if (hit.Armor.UseHitAngle && hit.Armor.Value != 0.0)
                {
                    mayRicochet = mayRicochet && hit.Armor.MayRicochet;

                    double thisNomarlizationAngle = testShell.ShellType.BasicNormalization();

                    if (testShell.Caliber >= hit.Armor.Value * 3.0 && testShell.ShellType.HasNormalizationEffect())
                    {
                        is3X = true;
                        is2X = true;
                        mayRicochet = false;
                        thisNomarlizationAngle *= 1.4 * testShell.Caliber / hit.Armor.Value;
                    }
                    else if (testShell.Caliber >= hit.Armor.Value * 2.0 && testShell.ShellType.HasNormalizationEffect())
                    {

                        is3X = false;
                        is2X = testShell.ShellType.HasNormalizationEffect();
                        thisNomarlizationAngle *= 1.4 * testShell.Caliber / hit.Armor.Value;
                    }

                    if (mayRicochet && thisImpactAngle > testShell.ShellType.RicochetAngle())
                    {
                        penetrationState = PenetrationState.Richochet;
                        break;
                    }

                    if (isFirstHit && testShell.ShellType.HasNormalizationEffect())
                    {
                        if (thisNomarlizationAngle > thisImpactAngle)
                            thisNomarlizationAngle = thisImpactAngle;

                        thisImpactAngle -= thisNomarlizationAngle;

                        nomarlizationAngle = thisNomarlizationAngle;
                    }

                    if (!heatExploded)
                    {
                        equivalentThickness += hit.Armor.Value / Math.Cos(DxUtils.ConvertDegreesToRadians(thisImpactAngle));
                    }
                    else
                    {
                        var distance = (hit.Distance - heatExplodedDistance);

                        var attenuation = 1 - distance * 0.5;
                        if (attenuation < 0)
                        {
                            penetrationState = PenetrationState.Unpenetratable;
                            break;
                        }

                        equivalentThickness += hit.Armor.Value / attenuation / Math.Cos(DxUtils.ConvertDegreesToRadians(thisImpactAngle));
                    }
                }
                else
                {
                    if (!heatExploded)
                    {
                        equivalentThickness += hit.Armor.Value;
                    }
                    else
                    {
                        var distance = (hit.Distance - heatExplodedDistance);

                        var attenuation = 1 - distance * 0.5;
                        if (attenuation < 0)
                        {
                            penetrationState = PenetrationState.Unpenetratable;
                            break;
                        }

                        equivalentThickness += hit.Armor.Value / attenuation;
                    }
                }

                if (!hit.Armor.IsSpacingArmor)
                {
                    if (equivalentThickness < 999.0)
                        penetrationState = PenetrationState.Penetratable;
                    else
                        penetrationState = PenetrationState.Unpenetratable;

                    break;
                }
	            if (testShell.ShellType == ShellType.He || testShell.ShellType == ShellType.PremiumHe)
	            {
		            penetrationState = PenetrationState.Unpenetratable;
		            break;
	            }
	            if (testShell.ShellType == ShellType.Heat && isFirstHit)
	            {
		            heatExploded = true;
		            heatExplodedDistance = hit.Distance;
		            mayRicochet = false;
	            }
	            isFirstHit = false;
            }
            return new ShootTestResult(penetrationState, equivalentThickness, impactAngle, nomarlizationAngle, is2X, is3X);
        }

        
    }
}
