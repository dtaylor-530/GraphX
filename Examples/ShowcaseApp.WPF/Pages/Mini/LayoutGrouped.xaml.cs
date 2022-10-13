using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GraphX;
using GraphX.Common;
using GraphX.Common.Enums;
using GraphX.Common.Interfaces;
using GraphX.Logic.Algorithms.LayoutAlgorithms;
using GraphX.Logic.Algorithms.LayoutAlgorithms.Grouped;
using QuikGraph;
using ShowcaseApp.WPF.Models;
using Rect = GraphX.Measure.Rect;

namespace ShowcaseApp.WPF.Pages.Mini
{
    /// <summary>
    /// Interaction logic for LayoutVCP.xaml
    /// </summary>
    public partial class LayoutGrouped : UserControl
    {
        public LayoutGrouped()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += ControlLoaded;
            graphArea.SideExpansionSize = new Size(80, 80);
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            var graph = ShowcaseHelper.GenerateDataGraph(10);
            Grouper.AssignGroupIds(graph.Vertices);
            GenerateGraph(graph);
            zoomControl.ZoomToFill();
        }

        private void GenerateGraph(GraphExample graph)
        {
            graphArea.ClearLayout();
            //generate group params
            var prms = Grouper.CreateGroupParameters().ToList();

            graphArea.LogicCore = GetLogicCore(prms, graph, cbArrangeGroups.IsChecked ?? false); ;
            //generate graphs
            graphArea.GenerateGraph();

            Random rand = new Random();
            foreach (var (rect, size) in
                prms
                .Where(a => a.ZoneRectangle.HasValue)
                .Select(a => GenerateGroupBorder(a.ZoneRectangle.Value, string.Format("Group {0}", a.GroupId), PickRandomBrush(rand))))

            {
                graphArea.InsertCustomChildControl(0, rect);
                GraphAreaBase.SetX(rect, size.X);
                GraphAreaBase.SetY(rect, size.Y);
            }
        }

        private static Brush PickRandomBrush(Random rnd)
        {
            PropertyInfo[] properties = typeof(Brushes).GetProperties();
            return (Brush)properties[rnd.Next(properties.Length)].GetValue(null, null);
        }

        private static LogicCoreExample GetLogicCore(List<IAlgorithmGroupParameters<DataVertex, DataEdge>> prms, GraphExample graph, bool arrangeGroups)
        {
            var logicCore = new LogicCoreExample()
            {
                Graph = graph,
                DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.None,
            };

            //generate grouping algo
            logicCore.ExternalLayoutAlgorithm =
                new GroupingLayoutAlgorithm<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>>(
                    graph,
                null,
                CreateLayoutAlgorithmParameters(prms, logicCore.AlgorithmFactory, arrangeGroups));

            return logicCore;
        }

        private static GroupingLayoutAlgorithmParameters<DataVertex, DataEdge> CreateLayoutAlgorithmParameters(
            List<IAlgorithmGroupParameters<DataVertex, DataEdge>> prms,
            IAlgorithmFactory<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>> factory,
            bool arrangeGroups)
        {
            return new GroupingLayoutAlgorithmParameters<DataVertex, DataEdge>(prms, true)
            {
                OverlapRemovalAlgorithm = factory.CreateFSAA<object>(100, 100),
                ArrangeGroups = arrangeGroups,
            };
        }

        private static class Grouper
        {
            private static int g = 0;

            public static void AssignGroupIds(IEnumerable<DataVertex> vertices)
            {
                int i = 0;
                foreach (var v in vertices)
                {
                    if (i % 5 == 0)
                        g++;
                    v.GroupId = (g);
                    i++;
                }
            }

            public static IEnumerable<IAlgorithmGroupParameters<DataVertex, DataEdge>> CreateGroupParameters()
            {
                for (int i = 0; i < g; i++)
                {
                    yield return new AlgorithmGroupParameters<DataVertex, DataEdge>
                    {
                        GroupId = (i + 1),
                        LayoutAlgorithm =
                        new RandomLayoutAlgorithm<DataVertex, DataEdge, GraphExample>(
                            new RandomLayoutAlgorithmParams { Bounds = new Rect(10, 10, 490, 490) }),
                    };
                };
            }
        }

        private static (Border, Point) GenerateGroupBorder(Rect rect, string text, Brush brush)
        {
            const double _headerHeight = 30;
            const double _groupInnerPadding = 20;
            var size = new Point((rect.X - _groupInnerPadding * .5), (rect.Y - _groupInnerPadding * .5 - _headerHeight));

            var border = new Border
            {
                Width = rect.Width + _groupInnerPadding,
                Height = rect.Height + _groupInnerPadding + _headerHeight,
                Background = brush,
                Opacity = 1,
                CornerRadius = new CornerRadius(8),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(2),
                Child = new Border
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    CornerRadius = new CornerRadius(8, 8, 0, 0),
                    Background = Brushes.Black,
                    Height = _headerHeight,
                    Child = new TextBlock
                    {
                        Text = text,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = Brushes.White,
                        FontWeight = FontWeights.Bold,
                        FontSize = 12
                    }
                }
            };
            return (border, size);
        }

        private void GraphRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            var graph = ShowcaseHelper.GenerateDataGraph(10);
            Grouper.AssignGroupIds(graph.Vertices);
            GenerateGraph(graph);
            zoomControl.ZoomToFill();
        }
    }
}