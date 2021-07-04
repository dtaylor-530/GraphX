using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.Models;
using Graph.Bayesian.WPF.Models.Vertices;
using GraphX.Controls;
using QuikGraph;

namespace Graph.Bayesian.WPF.Controls
{
    /// <summary>
    /// This is custom GraphArea representation using custom data types.
    /// GraphArea is the visual panel component responsible for drawing visuals (vertices and edges).
    /// It is also provides many global preferences and methods that makes GraphX so customizable and user-friendly.
    /// </summary>
    public class GraphArea : GraphArea<Vertex, Edge, BidirectionalGraph<Vertex, Edge>>
    {
        public GraphArea()
        {
            EdgeClicked += GraphAreaExample_EdgeClicked;
            VertexClicked += GraphAreaExample_VertexClicked;
        }

        private void GraphAreaExample_VertexClicked(object sender, GraphX.Controls.Models.VertexClickedEventArgs args)
        {
            (args.Control.Vertex as Vertex).OnNext(new ClickMessage(string.Empty, string.Empty));
        }

        private void GraphAreaExample_EdgeClicked(object sender, GraphX.Controls.Models.EdgeClickedEventArgs args)
        {

            (args.Control.Edge as Edge).OnNext(new ClickMessage(string.Empty, string.Empty));
        }
    }
}
