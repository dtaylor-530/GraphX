using GraphX.Common.Enums;
using GraphX.Common.Interfaces;
using System;
using System.Drawing;

namespace GraphX.Common.Models
{
    public abstract class VertexBase : IGraphXVertex
    {
        private readonly Lazy<string> type;

        protected VertexBase(long id = -1)
        {
            type = new Lazy<string>(() => GetType().Name);
            ID = id;
        }

        /// <summary>
        /// Unique vertex ID
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// Gets or sets custom angle associated with the vertex
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// Gets or sets optional group identificator
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Skip vertex in algo calc and visualization
        /// </summary>
        public ProcessingOptionEnum SkipProcessing { get; set; }

        public bool Equals(IGraphXVertex other)
        {
            return Equals(this.ID, other.ID);
        }

        public virtual Color Color { get; } = Color.Gold;

        public string TypeName => type.Value;

        public override string ToString() => type.Value + " " + ID;
    }
}