using System.Windows;
using System.Windows.Controls;
using Graph.Bayesian.WPF.Infrastructure;
using GraphX.Controls;

namespace Graph.Bayesian.WPF.View
{
    /// <summary>
    /// Interaction logic for ConnectionView.xaml
    /// </summary>
    public partial class HistoryView : UserControl {
      public HistoryView() {
         InitializeComponent();

         Loaded += (s, e) => GraphAreaExample_Setup();
      }


      private void GraphAreaExample_Setup() {
         var dataGraph = GraphFactory.Create4();
         //  Area.LogicCore = GraphBuilder.CreateLogicCore(dataGraph);
         ZoomControl.SetViewFinderVisibility(zoomctrl, Visibility.Collapsed);
         Area.LogicCore = GraphBuilder.CreateLogicCore(dataGraph);
         GraphBuilder.ConfigureArea(Area);
         Area.RelayoutGraph();
         zoomctrl.ZoomToFill();

      }
   }
}
