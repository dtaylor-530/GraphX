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

namespace Graph.Bayesian.WPF.View {
   /// <summary>
   /// Interaction logic for MainView.xaml
   /// </summary>
   public partial class TimerView : UserControl {
      public TimerView() {

         InitializeComponent();

         //Lets setup GraphArea settings

         //gg_but_randomgraph.Click += gg_but_randomgraph_Click;
         //gg_but_relayout.Click += gg_but_relayout_Click;

         Loaded += (s,e)=> GraphAreaExample_Setup();
      }


      private void GraphAreaExample_Setup() {
         var dataGraph = GraphFactory.ConnectionsFactory.Create2(out var connectionVertex, out var generalVertex);
         //  Area.LogicCore = GraphBuilder.CreateLogicCore(dataGraph);
         ServiceView.Graph = dataGraph;

         var dataGraph2 = GraphFactory.ConnectionsFactory.Create(connectionVertex);
         //  Area2.LogicCore = GraphBuilder.CreateLogicCore(dataGraph2);
         DetailsView.Graph = dataGraph2;

         var dataGraph3 = GraphFactory.ConnectionsFactory.Create2(generalVertex);
         //  Area3.LogicCore = GraphBuilder.CreateLogicCore(dataGraph3);
         ModelView.Graph = dataGraph3;
      }


      public void Dispose() {
         //If you plan dynamicaly create and destroy GraphArea it is wise to use Dispose() method
         //that ensures that all potential memory-holding objects will be released.
         //Area.Dispose();
      }
   }
}
