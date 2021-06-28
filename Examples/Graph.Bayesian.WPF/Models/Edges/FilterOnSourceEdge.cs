using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Bayesian.WPF.Models.Edges {

   public class FilterOnSourceEdge : Edge {

      /// <summary>
      /// Default constructor. We need to set at least Source and Target properties of the edge.
      /// </summary>
      /// <param name="source">Source vertex data</param>
      /// <param name="target">Target vertex data</param>
      /// <param name="weight">Optional edge weight</param>
      public FilterOnSourceEdge(Vertex source, Vertex target, double weight = 1)
         : base(source, target, weight, new NoFilter()) {

      }

      /// <summary>
      /// Default parameter-less constructor (for serialization compatibility)
      /// </summary>
      public FilterOnSourceEdge()
         : base(null, null, 1) {
      }
   }
}
