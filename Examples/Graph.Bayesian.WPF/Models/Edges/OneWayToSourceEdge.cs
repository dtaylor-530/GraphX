using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Bayesian.WPF.Models.Edges {

   public class OneWayToSourceEdge : Edge {

      /// <summary>
      /// permits message from target to source
      /// </summary>
      /// <param name="source">Source vertex data</param>
      /// <param name="target">Target vertex data</param>
      /// <param name="weight">Optional edge weight</param>
      public OneWayToSourceEdge(Vertex source, Vertex target, double weight = 1)
         : base(source, target, weight, new NoFilter(), filterTarget: new AbsoluteFilter()) {

      }

      /// <summary>
      /// Default parameter-less constructor (for serialization compatibility)
      /// </summary>
      public OneWayToSourceEdge()
         : base(null, null, 1) {
      }
   }

    public class OneWayToFilterSourceEdge : Edge
    {

        /// <summary>
        /// permits message from target to source
        /// </summary>
        /// <param name="source">Source vertex data</param>
        /// <param name="target">Target vertex data</param>
        /// <param name="weight">Optional edge weight</param>
        public OneWayToFilterSourceEdge(Vertex source, Vertex target, double weight = 1)
           : base(source, target, weight, filterTarget: new AbsoluteFilter())
        {

        }

        /// <summary>
        /// Default parameter-less constructor (for serialization compatibility)
        /// </summary>
        public OneWayToFilterSourceEdge()
           : base(null, null, 1)
        {
        }
    }
}
