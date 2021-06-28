using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using Graph.Bayesian.WPF.Infrastructure;
using GraphX.Common.Models;

namespace Graph.Bayesian.WPF.Models {

   public class UnFilteredEdge : Edge {
      
      /// <summary>
      /// Default constructor. We need to set at least Source and Target properties of the edge.
      /// </summary>
      /// <param name="source">Source vertex data</param>
      /// <param name="target">Target vertex data</param>
      /// <param name="weight">Optional edge weight</param>
      public UnFilteredEdge(Vertex source, Vertex target, double weight = 1)
       : base(source, target, weight, new NoFilter(), new NoFilter()) {

      }

      /// <summary>
      /// Default parameter-less constructor (for serialization compatibility)
      /// </summary>
      public UnFilteredEdge()
          : base(null, null, 1) {
      }
   }
}
