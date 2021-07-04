using System;
using System.Windows;
using GraphX.Controls;
using ReactiveUI;

namespace Graph.Bayesian.WPF.View
{
    using Models;
    /// <summary>
    /// Interaction logic for GraphView.xaml
    /// </summary>
    public partial class GraphView
    {
        public GraphView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.WhenAnyValue(a => a.ViewModel).Subscribe(a =>
                {
                    GraphAreaExample_Setup(a);
                });
            });
        }


        private void GraphAreaExample_Setup(Graph dataGraph)
        {
            if (dataGraph == null)
            {
                ErrorTextBlock.Text = "DataGraph is null";
                return;
            }
            ZoomControl.SetViewFinderVisibility(MainZoomControl, Visibility.Collapsed);
            MainGraphArea.LogicCore = GraphBuilder.CreateLogicCore(dataGraph);
            GraphBuilder.ConfigureArea(MainGraphArea);
            MainGraphArea.RelayoutGraph();
            MainZoomControl.ZoomToFill();
        }
    }
}