using Graph.Bayesian.WPF.Controls;
using Graph.Bayesian.WPF.Models;
using Graph.Bayesian.WPF.Models.Vertices;
using GraphX.Common.Enums;
using GraphX.Controls;
using GraphX.Logic.Algorithms.LayoutAlgorithms;
using QuikGraph;

namespace Graph.Bayesian.WPF
{
    static class GraphBuilder {
      public static void ConfigureArea(GraphArea area) {
         //Lets generate configured graph using pre-created data graph assigned to LogicCore object.
         //Optionaly we set first method param to True (True by default) so this method will automatically generate edges
         //  If you want to increase performance in cases where edges don't need to be drawn at first you can set it to False.
         //  You can also handle edge generation by calling manually Area.GenerateAllEdges() method.
         //Optionaly we set second param to True (True by default) so this method will automaticaly checks and assigns missing unique data ids
         //for edges and vertices in _dataGraph.
         //Note! Area.Graph property will be replaced by supplied _dataGraph object (if any).
         area.GenerateGraph(true, true);

         /* 
       * After graph generation is finished you can apply some additional settings for newly created visual vertex and edge controls
       * (VertexControl and EdgeControl classes).
       * 
       */

         //This method sets the dash style for edges. It is applied to all edges in Area.EdgesList. You can also set dash property for
         //each edge individually using EdgeControl.DashStyle property.
         //For ex.: Area.EdgesList[0].DashStyle = GraphX.EdgeDashStyle.Dash;
         area.SetEdgesDashStyle(EdgeDashStyle.Dash);

         //This method sets edges arrows visibility. It is also applied to all edges in Area.EdgesList. You can also set property for
         //each edge individually using property, for ex: Area.EdgesList[0].ShowArrows = true;
         area.ShowAllEdgesArrows(true);

         //This method sets edges labels visibility. It is also applied to all edges in Area.EdgesList. You can also set property for
         //each edge individually using property, for ex: Area.EdgesList[0].ShowLabel = true;
         area.ShowAllEdgesLabels(true);
      }

      public static GXLogicCore CreateLogicCore(BidirectionalGraph<Vertex, Edge> graph) {
         //Lets create logic core and filled data graph with edges and vertices
         var logicCore = new GXLogicCore {

            Graph = graph,
            //This property sets layout algorithm that will be used to calculate vertices positions
            //Different algorithms uses different values and some of them uses edge Weight property.
            DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.KK,
            DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA,
            //This property sets edge routing algorithm that is used to build route paths according to algorithm logic.
            //For ex., SimpleER algorithm will try to set edge paths around vertices so no edge will intersect any vertex.
            //Bundling algorithm will try to tie different edges that follows same direction to a single channel making complex graphs more appealing.
            DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.SimpleER,
            //This property sets async algorithms computation so methods like: Area.RelayoutGraph() and Area.GenerateGraph()
            //will run async with the UI thread. Completion of the specified methods can be catched by corresponding events:
            //Area.RelayoutFinished and Area.GenerateGraphFinished.
            AsyncAlgorithmCompute = false
         };

         //Now we can set parameters for selected algorithm using AlgorithmFactory property. This property provides methods for
         //creating all available algorithms and algo parameters.
         logicCore.DefaultLayoutAlgorithmParams = logicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.KK);
         //This property sets vertex overlap removal algorithm.
         //Such algorithms help to arrange vertices in the layout so no one overlaps each other.

         //Default parameters are created automaticaly when new default algorithm is set and previous params were NULL
         logicCore.DefaultOverlapRemovalAlgorithmParams.HorizontalGap = 50;
         logicCore.DefaultOverlapRemovalAlgorithmParams.VerticalGap = 50;
         //Unfortunately to change algo parameters you need to specify params type which is different for every algorithm.
         ((KKLayoutParameters)logicCore.DefaultLayoutAlgorithmParams).MaxIterations = 100;

         //Finally assign logic core to GraphArea object
         return logicCore;
      }
   }
}