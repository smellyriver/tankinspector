using System;
using System.Windows;
using System.Windows.Media;
using Smellyriver.Utilities;
using Smellyriver.TankInspector.Modeling;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Smellyriver.TankInspector.UIComponents
{
    internal static class GunTraverseHelper
    {
        public static Point PolarToCartesian(Point center, double radius, double degrees)
        {
            return new Point(radius * Math.Cos(degrees * Math.PI / 180) + center.X, -radius * Math.Sin(degrees * Math.PI / 180) + center.Y);
        }

        public static void CartesianToPolar(Point center, Point point, out double radius, out double degree)
        {
            var d = point - center;
            radius = d.Length;
            degree = GunTraverseHelper.NormalizeAngle(-Math.Atan2(d.Y, d.X) * 180 / Math.PI);
        }

        public static double NormalizeAngle(double degree)
        {
            while (degree < 0)
                degree += 360;

            while (degree > 360)
                degree -= 360;

            return degree;
        }

        public static double RotateAngle(double from, double to, double delta, double limitRight, double limitLeft)
        {
            from = GunTraverseHelper.NormalizeAngle(from);
            to = GunTraverseHelper.NormalizeAngle(to);

            if (Math.Abs(from - to) < 0.0001)
                return to;

            delta = GunTraverseHelper.NormalizeAngle(delta);
            double direction;

            if (from < to)
            {
                if (limitRight + limitLeft < 360
                    && from <= limitRight
                    && to >= 360 - limitLeft)
                    direction = -1.0;
                else if (to - from > 180)
                    direction = -1.0;
                else
                    direction = 1.0;
            }
            else
            {
                if (limitRight + limitLeft < 360
                    && to <= limitRight
                    && from >= 360 - limitLeft)
                    direction = 1.0;
                else if (from - to > 180)
                    direction = 1.0;
                else
                    direction = -1.0;
            }

            var result = from + delta * direction;
            if ((from < to && result > to) || (from > to && result < to))
                result = to;

            return result;
        }

        private static double DefaultVerticalTraverseTransform(double value)
        {
            return value;
        }

        public static Geometry CreateGeometry(GunVerticalTraverseComponentViewModel traverse,
                                              Point center,
                                              Func<double, double> radiusConverter,
                                              double geometryRotation = 0)
        {
	        if (traverse.IsPost909Format)
                return GunTraverseHelper.CreateGeometryPost909(traverse, center, radiusConverter, geometryRotation);
	        return GunTraverseHelper.CreateGeometryPre909(traverse, center, radiusConverter, geometryRotation);
        }

        private static Geometry CreateGeometryPost909(GunVerticalTraverseComponentViewModel traverse,
                                                      Point center,
                                                      Func<double, double> radiusConverter,
                                                      double geometryRotation = 0)
        {
            Geometry geometry;
            double maxRadius;
            var data = traverse.TraverseData;

            if (data.Length == 1 || (data.Length == 2 && data[0].Limit == data[1].Limit))
            {
                var radius = radiusConverter(data[0].Limit);
                geometry = new EllipseGeometry(center, radius, radius);
                maxRadius = radius;
            }
            else
            {
                maxRadius = radiusConverter(traverse.MaximumTraverse);

                var figureFigure = new PathFigure();

                var previousTheta = 0.0;
                var previousPoint = new Point();
                var previousRadius = 0.0;
                PitchLimits.Component previousNode = new PitchLimits.Component();

                for (var i = 0; i < data.Length; ++i)
                {
                    var node = data[i];
                    var theta = geometryRotation + node.Angle * 360;
                    var radius = radiusConverter(node.Limit);
                    var point = GunTraverseHelper.PolarToCartesian(center, radius, theta);
                    var thetaRange = theta - previousTheta;

                    if (i == 0)
                        figureFigure.StartPoint = point;
                    else
                    {
                        if (previousNode.Angle == node.Angle)
                            figureFigure.Segments.Add(new LineSegment(point, true));
                        else if (previousNode.Limit == node.Limit)
                        {

                            figureFigure.Segments.Add(new ArcSegment(point,
                                                                     new Size(radius, radius),
                                                                     thetaRange,
                                                                     thetaRange >= 180,
                                                                     SweepDirection.Counterclockwise,
                                                                     true));
                        }
                        else
                        {
                            figureFigure.Segments.Add(GunTraverseHelper.CreateTraverseFigureTransition(ref figureFigure,
                                                                                                       ref center,
                                                                                                       previousTheta,
                                                                                                       thetaRange,
                                                                                                       previousRadius,
                                                                                                       radius));
                        }
                    }

                    previousTheta = theta;
                    previousPoint = point;
                    previousNode = node;
                    previousRadius = radius;
                }


                var pathGeometry = new PathGeometry();
                pathGeometry.Figures.Add(figureFigure);
                geometry = pathGeometry;
            }

            return GunTraverseHelper.ProcessGeometryHorizontalTraverse(traverse, center, geometryRotation, geometry, maxRadius);
        }

        private static Geometry ProcessGeometryHorizontalTraverse(GunVerticalTraverseComponentViewModel traverse,
                                                                  Point center,
                                                                  double geometryRotation,
                                                                  Geometry geometry,
                                                                  double maxRadius)
        {
            var horizontal = traverse.HorizontalTraverse;

            if (horizontal == null || horizontal.Range == 360)
                return geometry;
	        // create a horizontal fan
	        var fanFigure = new PathFigure();
	        fanFigure.StartPoint = center;
	        var point = GunTraverseHelper.PolarToCartesian(center, maxRadius, 360 - horizontal.Right + geometryRotation);
	        fanFigure.Segments.Add(new LineSegment(point, true));
	        point = GunTraverseHelper.PolarToCartesian(center, maxRadius, horizontal.Left + geometryRotation);
	        fanFigure.Segments.Add(new ArcSegment(point,
		        new Size(maxRadius, maxRadius),
		        horizontal.Range,
		        horizontal.Range > 180,
		        SweepDirection.Counterclockwise,
		        true));
	        fanFigure.IsClosed = true;
	        var fanGeometry = new PathGeometry();
	        fanGeometry.Figures.Add(fanFigure);

	        return new CombinedGeometry(GeometryCombineMode.Intersect, geometry, fanGeometry);
        }

        private static Geometry CreateGeometryPre909(GunVerticalTraverseComponentViewModel traverse,
                                                     Point center,
                                                     Func<double, double> radiusConverter,
                                                     double geometryRotation = 0)
        {
            Geometry geometry;
            double maxRadius;

            if (traverse.HasSingularValue && traverse.HorizontalTraverse.Range == 360)
            {
                var radius = radiusConverter(traverse.Default);
                geometry = new EllipseGeometry(center, radius, radius);
                maxRadius = radius;
            }
            else
            {
                var leftRadius = radiusConverter(traverse.Left);
                var rightRadius = radiusConverter(traverse.Right);
                var frontRadius = radiusConverter(traverse.Front);
                var backRadius = radiusConverter(traverse.Back);

                maxRadius = MathEx.Max(leftRadius, rightRadius, frontRadius, backRadius);

                var figureFigure = new PathFigure();

                double theta;
                Point point;
                double arcRange;

                if (traverse.HasExtraFrontPitchLimits)
                {
                    // in standard polar coordinate system
                    theta = geometryRotation - traverse.ExtraFrontPitchLimitsRange / 2 - traverse.ExtraPitchLimitsTransition;
                    point = GunTraverseHelper.PolarToCartesian(center, rightRadius, theta);
                    figureFigure.StartPoint = point;

                    // right to front transition
                    point = GunTraverseHelper.PolarToCartesian(center, frontRadius, theta);
                    if (traverse.ExtraPitchLimitsTransition == 0)
                        figureFigure.Segments.Add(new LineSegment(point, true));
                    else
                        figureFigure.Segments.Add(GunTraverseHelper.CreateTraverseFigureTransition(ref figureFigure, ref center, theta, traverse.ExtraPitchLimitsTransition, rightRadius, frontRadius));

                    // front
                    theta = geometryRotation + traverse.ExtraFrontPitchLimitsRange / 2;
                    point = GunTraverseHelper.PolarToCartesian(center, frontRadius, theta);
                    arcRange = traverse.ExtraFrontPitchLimitsRange;
                    var frontArc = new ArcSegment(point, new Size(frontRadius, frontRadius), arcRange, arcRange >= 180, SweepDirection.Counterclockwise, true);
                    figureFigure.Segments.Add(frontArc);

                    // front to left transition
                    point = GunTraverseHelper.PolarToCartesian(center, leftRadius, theta);
                    if (traverse.ExtraPitchLimitsTransition == 0)
                        figureFigure.Segments.Add(new LineSegment(point, true));
                    else
                        figureFigure.Segments.Add(GunTraverseHelper.CreateTraverseFigureTransition(ref figureFigure, ref center, theta, traverse.ExtraPitchLimitsTransition, frontRadius, leftRadius));
                }
                else
                {
                    theta = geometryRotation;
                    point = GunTraverseHelper.PolarToCartesian(center, leftRadius, theta);
                    figureFigure.StartPoint = point;
                }

                // left
                var leftArcEndAngle = traverse.HasExtraBackPitchLimits
                    ? 180 + geometryRotation - traverse.ExtraBackPitchLimitsRange / 2 - traverse.ExtraPitchLimitsTransition
                    : 180 + geometryRotation;

                point = GunTraverseHelper.PolarToCartesian(center, leftRadius, leftArcEndAngle);
                arcRange = leftArcEndAngle - theta;
                var leftArc = new ArcSegment(point, new Size(leftRadius, leftRadius), arcRange, arcRange >= 180, SweepDirection.Counterclockwise, true);
                figureFigure.Segments.Add(leftArc);

                if (traverse.HasExtraBackPitchLimits)
                {
                    // left to back transition
                    theta = leftArcEndAngle;
                    point = GunTraverseHelper.PolarToCartesian(center, backRadius, theta);
                    if (traverse.ExtraPitchLimitsTransition == 0)
                        figureFigure.Segments.Add(new LineSegment(point, true));
                    else
                        figureFigure.Segments.Add(GunTraverseHelper.CreateTraverseFigureTransition(ref figureFigure, ref center, theta, traverse.ExtraPitchLimitsTransition, leftRadius, backRadius));

                    // back
                    theta = 180 + geometryRotation + traverse.ExtraBackPitchLimitsRange / 2;
                    point = GunTraverseHelper.PolarToCartesian(center, backRadius, theta);
                    arcRange = traverse.ExtraBackPitchLimitsRange;
                    var backArc = new ArcSegment(point, new Size(backRadius, backRadius), arcRange, arcRange >= 180, SweepDirection.Counterclockwise, true);
                    figureFigure.Segments.Add(backArc);

                    // back to right transition
                    point = GunTraverseHelper.PolarToCartesian(center, rightRadius, theta);
                    if (traverse.ExtraPitchLimitsTransition == 0)
                        figureFigure.Segments.Add(new LineSegment(point, true));
                    else
                        figureFigure.Segments.Add(GunTraverseHelper.CreateTraverseFigureTransition(ref figureFigure, ref center, theta, traverse.ExtraPitchLimitsTransition, backRadius, rightRadius));
                }

                // right
                var rightArcEndAngle = traverse.HasExtraFrontPitchLimits ? 360 + geometryRotation - traverse.ExtraFrontPitchLimitsRange / 2 - traverse.ExtraPitchLimitsTransition : 360 + geometryRotation;
                point = GunTraverseHelper.PolarToCartesian(center, rightRadius, rightArcEndAngle);
                arcRange = theta - rightArcEndAngle;
                var rightArc = new ArcSegment(point, new Size(rightRadius, rightRadius), arcRange, arcRange >= 180, SweepDirection.Counterclockwise, true);
                figureFigure.Segments.Add(rightArc);


                var pathGeometry = new PathGeometry();
                pathGeometry.Figures.Add(figureFigure);
                geometry = pathGeometry;
            }

            return GunTraverseHelper.ProcessGeometryHorizontalTraverse(traverse, center, geometryRotation, geometry, maxRadius);
        }

        public static Geometry CreateGeometry(GunVerticalTraverseComponentViewModel traverse,
                                              double size,
                                              Point center,
                                              double geometryRotation = 0,
                                              double margin = 0,
                                              double padding = 0,
                                              Func<double, double> verticalTraverseTransform = null)
        {

            if (verticalTraverseTransform == null)
                verticalTraverseTransform = GunTraverseHelper.DefaultVerticalTraverseTransform;

            if (traverse.HasSingularValue && traverse.HorizontalTraverse.Range == 360)
            {
                var radius = size / 2 - margin;
                return new EllipseGeometry(center, radius, radius);
            }
	        var maxRadiusInDegrees = traverse.MaximumTraverse;
	        var scale = (size / 2 - margin - padding) / maxRadiusInDegrees;

	        Func<double, double> radiusConverter = r => padding + Math.Max(verticalTraverseTransform(r) * scale, 0);

	        return traverse.IsPost909Format
		        ? GunTraverseHelper.CreateGeometryPost909(traverse, center, radiusConverter, geometryRotation)
		        : GunTraverseHelper.CreateGeometryPre909(traverse, center, radiusConverter, geometryRotation);
        }



        private static PathSegment CreateTraverseFigureTransition(ref PathFigure figure, ref Point center, double startAngle, double transitionAngle, double from, double to)
        {
            const double step = 0.1;
            var endAngle = startAngle + transitionAngle;
            var delta = to - from;
            var segment = new PolyLineSegment();
            segment.IsStroked = true;
            for (var angle = startAngle + step; angle <= endAngle; angle += step)
            {
                var traverse = from + delta * (angle - startAngle) / transitionAngle;
                segment.Points.Add(GunTraverseHelper.PolarToCartesian(center, traverse, angle));
            }

            return segment;
        }

    }
}
