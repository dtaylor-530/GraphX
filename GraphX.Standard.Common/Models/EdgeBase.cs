using GraphX.Common.Enums;
using GraphX.Common.Interfaces;
using GraphX.Measure;
using System;

namespace GraphX.Common.Models
{
    /// <summary>
    /// Base class for graph edge
    /// </summary>
    /// <typeparam name="TVertex">Vertex class</typeparam>
    public abstract class EdgeBase<TVertex> : IGraphXEdge<TVertex>, IEquatable<IIdentifiableGraphDataObject>
    {
        protected readonly string key;

        /// <summary>
        /// Skip edge in algo calc and visualization
        /// </summary>
        public ProcessingOptionEnum SkipProcessing { get; set; }

        protected EdgeBase(TVertex source, TVertex target, double weight = 1, long id = -1) : this(weight, id)
        {
            Source = source;
            Target = target;

        }

        protected EdgeBase(double weight = 1, long id = -1)
        {
            Weight = weight;
            ID = id;
        }

        /// <summary>
        /// Unique edge ID
        /// </summary>
        public virtual long ID { get; set; }

        public virtual object Content { get; }

        /// <summary>
        /// Returns true if Source vertex equals Target vertex
        /// </summary>
        public bool IsSelfLoop => Source.Equals(Target);

        /// <summary>
        /// Optional parameter to bind edge to static vertex connection point
        /// </summary>
        public int? SourceConnectionPointId { get; set; }

        /// <summary>
        /// Optional parameter to bind edge to static vertex connection point
        /// </summary>
        public int? TargetConnectionPointId { get; set; }

        /// <summary>
        /// Routing points collection used to make Path visual object
        /// </summary>
        public virtual Point[] RoutingPoints { get; set; }

        /// <summary>
        /// Source vertex
        /// </summary>
        public TVertex Source { get; set; }

        /// <summary>
        /// Target vertex
        /// </summary>
        public TVertex Target { get; set; }

        /// <summary>
        /// Edge weight that can be used by some weight-related layout algorithms
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Reverse the calculated routing path points.
        /// </summary>
        public bool ReversePath { get; set; }

        public bool Equals(IIdentifiableGraphDataObject other)
        {
            return this.ID == other.ID;
        }
    }
}
