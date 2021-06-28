using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Graph.Bayesian.WPF.Infrastructure;

namespace Graph.Bayesian.WPF.Controls
{

    public class TravellerControl : Canvas
    {
        public enum Direction
        {
            None, SourceToTarget, TargetToSource
        }

        private readonly EllipseGeometry particlePath;
        private Path ellipsePath;
        private Path journeyPath;
        public static readonly DependencyProperty PointProperty = DependencyProperty.Register("Point", typeof(object), typeof(TravellerControl), new PropertyMetadata(default(Point), Changed));
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(TravellerControl), new PropertyMetadata(TimeSpan.FromSeconds(2)));
        public static readonly DependencyProperty RadiusYProperty = DependencyProperty.Register("RadiusY", typeof(double), typeof(TravellerControl), new PropertyMetadata(5d));
        public static readonly DependencyProperty RadiusXProperty = DependencyProperty.Register("RadiusX", typeof(double), typeof(TravellerControl), new PropertyMetadata(5d));
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(TravellerControl), new PropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(TravellerControl), new PropertyMetadata(Brushes.White));
        public static readonly DependencyProperty ShowPathProperty = DependencyProperty.Register("ShowPath", typeof(bool), typeof(TravellerControl), new PropertyMetadata(true));
        public static readonly DependencyProperty RunCommandProperty = DependencyProperty.Register("RunCommand", typeof(ICommand), typeof(TravellerControl), new PropertyMetadata(null));
        public static readonly DependencyProperty RunProperty = DependencyProperty.Register("Run", typeof(Direction), typeof(TravellerControl), new PropertyMetadata(Direction.SourceToTarget, RunChanged));
        private DependencyPropertyChangedEventArgs e;

        private static void RunChanged(DependencyObject d, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (d is TravellerControl traveller)
                traveller._Run(traveller.e, (Direction)dependencyPropertyChangedEventArgs.NewValue);
        }

        public TravellerControl()
        {
            RunCommand = ReactiveUI.ReactiveCommand.Create(() =>
            {
                try
                {
                    _Run(e, Run);
                }
                catch (Exception ex)
                {
                }
            });

            if (ShowPath)
            {
                this.Children.Add(journeyPath = CreateJourneyPath());
            }

            particlePath = new EllipseGeometry { RadiusX = RadiusX, RadiusY = RadiusY };
            ellipsePath = new Path { Fill = Foreground, Data = particlePath };
            this.Children.Add(ellipsePath);
        }

        public Point LastPoint => Point switch
        {

            Point[] points => points.LastOrDefault(),
            Point point => point,
            _ => throw new Exception("sdf  dd")
        };

        public Direction Run
        {
            get => (Direction)GetValue(RunProperty);
            set => SetValue(RunProperty, value);
        }

        public object Point
        {
            get => (object)GetValue(PointProperty);
            set => SetValue(PointProperty, value);
        }

        public ICommand RunCommand
        {
            get => (ICommand)GetValue(RunCommandProperty);
            set => SetValue(RunCommandProperty, value);
        }

        public Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }


        public Brush Stroke
        {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        public TimeSpan Duration
        {
            get => (TimeSpan)GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        public double RadiusX
        {
            get => (double)GetValue(RadiusXProperty);
            set => SetValue(RadiusXProperty, value);
        }

        public double RadiusY
        {
            get => (double)GetValue(RadiusYProperty);
            set => SetValue(RadiusYProperty, value);
        }
        public bool ShowPath
        {
            get => (bool)GetValue(ShowPathProperty);
            set => SetValue(ShowPathProperty, value);
        }

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (!(d is TravellerControl control))
                return;
            if (e.NewValue == null)
                return;
            control.e = e;
        }

        private void _Run(DependencyPropertyChangedEventArgs e, Direction direction)
        {
            if (direction == Direction.None)
                return;

            SetChildProperties();
            GetPoints(e.NewValue, e.OldValue, out var oldPoint, out var points);

            switch (direction)
            {
                case Direction.TargetToSource:
                    oldPoint = points.FirstOrDefault();
                    points = points.Reverse().ToArray();
                    break;
                case Direction.SourceToTarget:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (ShowPath && points.Length > 0)
            {
                if (Children.Count == 1)
                    this.Children.Add(journeyPath = CreateJourneyPath());
                journeyPath.Data = points.ToArray().ToPathGeometry();
            }

            else if (Children.Count == 2)
            {
                Children.Remove(journeyPath);
            }

            ControlsHelper.ProcessAnimationsQueue(GetAnimations(oldPoint, points, Duration), particlePath, EllipseGeometry.CenterProperty);
        }

        private Path CreateJourneyPath()
        {
            return new Path
            {
                Stroke = Stroke,
                StrokeThickness = Math.Max(RadiusX, RadiusY) + 1,
                Data = new[] { new Point(0, 0), new Point(0, 0) }
                  .ToArray().ToPathGeometry()
            };
        }

        static void GetPoints(object newValue, object oldValue, out Point oldPoint, out Point[] points)
        {
            points = newValue switch
            {
                Point[] pts => pts,
                Point pnt => new[] { pnt },
                PathGeometry geometry => geometry.ToPoints().ToArray(),
                null => new Point[] { },// throw new Exception($"Points can't be null"),
                _ => throw new Exception($"Points must be a Point or array of points and not {newValue.GetType().Name}")
            };

            oldPoint = oldValue switch
            {
                Point[] opts => opts.LastOrDefault(),
                Point opnt => opnt,
                PathGeometry geometry => geometry.ToPoints().Last(),
                _ => default
            };
        }

        private static Queue<PointAnimation> GetAnimations(Point oldPoint, Point[] points, TimeSpan duration)
        {
            Queue<PointAnimation> list = new Queue<PointAnimation>();

            if (points.Length > 1)
                points.Skip(1).Aggregate(points.First(), (current, point) => OldPoint(current, duration, point, list));
            else
                points.Aggregate(oldPoint, (current, point) => OldPoint(current, duration, point, list));

            return list;

            static Point OldPoint(Point oldPoint, TimeSpan duration, Point point, Queue<PointAnimation> list)
            {
                var pointAnimation = new PointAnimation
                {
                    Duration = duration,
                    From = oldPoint,
                    To = point
                };
                oldPoint = point;
                list.Enqueue(pointAnimation);
                return oldPoint;
            }
        }

        private void SetChildProperties()
        {
            particlePath.RadiusX = RadiusX;
            particlePath.RadiusY = RadiusY;
            ellipsePath.Fill = Foreground;
            journeyPath.Stroke = Stroke;
            journeyPath.StrokeThickness = Math.Max(RadiusX, RadiusY) + 1;
        }
    }
}

