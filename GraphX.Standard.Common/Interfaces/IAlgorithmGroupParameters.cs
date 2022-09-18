using GraphX.Common.Interfaces;
using GraphX.Measure;

namespace GraphX.Logic.Algorithms.LayoutAlgorithms.Grouped
{
    public interface IAlgorithmGroupParameters<TVertex, TEdge>
        where TVertex : class, IGraphXVertex
        where TEdge : IGraphXEdge<TVertex>
    {
        int GroupId { get; set; }
        bool IsAlgorithmBounded { get; set; }
        IExternalLayout<TVertex, TEdge> LayoutAlgorithm { get; set; }
        Rect? ZoneRectangle { get; set; }
    }
}