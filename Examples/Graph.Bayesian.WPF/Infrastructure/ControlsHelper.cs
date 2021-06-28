using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Graph.Bayesian.WPF.Infrastructure
{
    public static class ControlsHelper
    {
        public static PathGeometry ToPathGeometry(this Point[] points)
        {
            return new PathGeometry
            {
                Figures = new PathFigureCollection
            {
               CreatePathFigure(points)
            }
            };

            static PathFigure CreatePathFigure(Point[] points)
            {
                PathFigure pathFigure = new PathFigure { StartPoint = points[0] };

                for (int i = 1; i < points.Length; i++)
                {
                    pathFigure.Segments.Add(new LineSegment { Point = points[i] });
                }

                return pathFigure;
            }
        }

        public static void ProcessAnimationsQueue<T>(Queue<T> list, Animatable geometry, DependencyProperty property) where T : AnimationTimeline
        {
            if (list.Any() == false)
                return;

            var pointAnimation = list.Dequeue();
            pointAnimation.Completed += (s, e) => ProcessAnimationsQueue(list, geometry, property);
            geometry.BeginAnimation(property, pointAnimation);
        }

        public static PathGeometry ConvertToPathGeometry(params Point[] points)
        {
            return ToPathGeometry(points);
        }

        public static IEnumerable<Point> ToPoints(this PathGeometry geometry)
        {

            var points = geometry
               .Figures
               .SelectMany(figure => 
               new[] { figure.StartPoint }
               .Concat(figure.Segments.OfType<LineSegment>().Select(segment => segment.Point))
               .Concat(figure.Segments.OfType<PolyLineSegment>().SelectMany(segment => segment.Points))
               )
               .Distinct()
               .ToArray();

            return points;
        }

        public static Point GetEndPoint(this Geometry geo)
        {
            PathGeometry path = geo.GetFlattenedPathGeometry();

            var seg = path.Figures.Last().Segments.Last();
            switch (seg)
            {
                case PolyLineSegment pls:
                    return pls.Points.Last();
                case LineSegment ls:
                    return ls.Point;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}