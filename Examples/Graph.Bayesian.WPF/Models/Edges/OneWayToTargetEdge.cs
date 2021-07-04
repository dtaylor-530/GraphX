using Graph.Bayesian.WPF.Models.Vertices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Bayesian.WPF.Models.Edges
{

    public class OneWayToTargetEdge : Edge
    {

        /// <summary>
        /// Permits messages from source-to-target
        /// </summary>
        /// <param name="source">Source vertex data</param>
        /// <param name="target">Target vertex data</param>
        /// <param name="weight">Optional edge weight</param>
        public OneWayToTargetEdge(Vertex source, Vertex target, double weight = 1)
           : base(source, target, weight, new AbsoluteFilter(), new NoFilter())
        {

        }


        /// <summary>
        /// Default parameter-less constructor (for serialization compatibility)
        /// </summary>
        public OneWayToTargetEdge() : base(null, null, 1)
        {
        }
    }
    public class OneWayToTargetFilterEdge : Edge
    {

        /// <summary>
        /// Permits messages from source-to-target
        /// </summary>
        /// <param name="source">Source vertex data</param>
        /// <param name="target">Target vertex data</param>
        /// <param name="weight">Optional edge weight</param>
        public OneWayToTargetFilterEdge(Vertex source, Vertex target, double weight = 1)
           : base(source, target, weight, new AbsoluteFilter())
        {

        }

        /// <summary>
        /// Default parameter-less constructor (for serialization compatibility)
        /// </summary>
        public OneWayToTargetFilterEdge() : base(null, null, 1)
        {
        }
    }
}
