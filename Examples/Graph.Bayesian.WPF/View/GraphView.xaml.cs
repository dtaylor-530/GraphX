using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.ViewModel;
using GraphX.Controls;
using ReactiveUI;

namespace Graph.Bayesian.WPF.View
{
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


        private void GraphAreaExample_Setup(GraphViewModel graphViewModel)
        {

            ZoomControl.SetViewFinderVisibility(MainZoomControl, Visibility.Collapsed);

            graphViewModel
                .WhenAnyValue(a => a.Graph)                
                .Subscribe(dataGraph =>
                {
                    if(dataGraph==null)
                    {
                        ErrorTextBlock.Text = "DataGraph is null";
                        return;
                    }

                    MainGraphArea.LogicCore = GraphBuilder.CreateLogicCore(dataGraph);
                    GraphBuilder.ConfigureArea(MainGraphArea);
                    MainGraphArea.RelayoutGraph();
                    MainZoomControl.ZoomToFill();
                });

        }
    }
}