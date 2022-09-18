using GraphX.Common.Enums;
using GraphX.Common.Interfaces;
using GraphX.Measure;
using QuikGraph;
using System.Collections.Generic;
using System.Threading;

namespace GraphX.Standard.Common.Interfaces
{

    public interface IGXLogicCore<TVertex, TEdge, TGraph>
        where TVertex : class, IGraphXVertex
        where TEdge : class, IGraphXEdge<TVertex>
        where TGraph : class, IMutableBidirectionalGraph<TVertex, TEdge>
    {
        IAlgorithmFactory<TVertex, TEdge, TGraph> AlgorithmFactory { get; }
        IAlgorithmStorage<TVertex, TEdge> AlgorithmStorage { get; set; }
        bool AsyncAlgorithmCompute { get; set; }
        EdgeRoutingAlgorithmTypeEnum DefaultEdgeRoutingAlgorithm { get; set; }
        IEdgeRoutingParameters DefaultEdgeRoutingAlgorithmParams { get; set; }
        LayoutAlgorithmTypeEnum DefaultLayoutAlgorithm { get; set; }
        ILayoutParameters DefaultLayoutAlgorithmParams { get; set; }
        OverlapRemovalAlgorithmTypeEnum DefaultOverlapRemovalAlgorithm { get; set; }
        IOverlapRemovalParameters DefaultOverlapRemovalAlgorithmParams { get; set; }
        bool EdgeCurvingEnabled { get; set; }
        double EdgeCurvingTolerance { get; set; }
        bool EnableParallelEdges { get; set; }
        IExternalEdgeRouting<TVertex, TEdge> ExternalEdgeRoutingAlgorithm { get; set; }
        IExternalLayout<TVertex, TEdge> ExternalLayoutAlgorithm { get; set; }
        IExternalOverlapRemoval<TVertex> ExternalOverlapRemovalAlgorithm { get; set; }
        Queue<IGraphFilter<TVertex, TEdge, TGraph>> Filters { get; set; }
        TGraph Graph { get; set; }
        bool IsCustomLayout { get; }
        bool IsEdgeRoutingEnabled { get; }
        bool IsFiltered { get; }
        bool IsFilterRemoved { get; set; }
        TGraph OriginalGraph { get; }
        int ParallelEdgeDistance { get; set; }

        void ApplyFilters();
        bool AreOverlapNeeded();
        bool AreVertexSizesNeeded();
        void Clear(bool clearStorages = true);
        IDictionary<TVertex, Point> Compute(CancellationToken cancellationToken);
        void ComputeEdgeRoutesByVertex(TVertex dataVertex, Point? vertexPosition = null, Size? vertexSize = null);
        IAlgorithmFactory<TVertex, TEdge, TGraph> CreateNewAlgorithmFactory();
        void CreateNewAlgorithmStorage(IExternalLayout<TVertex, TEdge> layout, IExternalOverlapRemoval<TVertex> or, IExternalEdgeRouting<TVertex, TEdge> er);
        void Dispose();
        bool GenerateAlgorithmStorage(Dictionary<TVertex, Size> vertexSizes, IDictionary<TVertex, Point> vertexPositions);
        IExternalEdgeRouting<TVertex, TEdge> GenerateEdgeRoutingAlgorithm(Size desiredSize, IDictionary<TVertex, Point> vertexPositions = null, IDictionary<TVertex, Rect> rectangles = null);
        IExternalLayout<TVertex, TEdge> GenerateLayoutAlgorithm(Dictionary<TVertex, Size> vertexSizes, IDictionary<TVertex, Point> vertexPositions);
        IExternalOverlapRemoval<TVertex> GenerateOverlapRemovalAlgorithm(Dictionary<TVertex, Rect> rectangles = null);
        Dictionary<TVertex, Rect> GetVertexSizeRectangles(IDictionary<TVertex, Point> positions, Dictionary<TVertex, Size> vertexSizes, bool getCenterPoints = false);
        void PopFilters();
        void PushFilters();
    }
}