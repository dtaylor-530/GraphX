using System;
using ReactiveUI;
using Graph.Bayesian.WPF.Infrastructure;
using System.Reactive;
using System.Reactive.Linq;
using GraphX.Common.Interfaces;

namespace Graph.Bayesian.WPF.Models.Vertices
{

    public record GraphMessage(string From, string To, DateTime Sent, Graph Graph) : Message(From, To, Sent, Graph);

    public record GraphChange();

    public record AddRandomVertexChange() : GraphChange;

    public record GraphChangeMessage(string From, string To, DateTime Sent, GraphChange Change) : Message(From, To, Sent, Change);

    public class GraphVertex : Vertex
    {
        private readonly IDisposable graph;

        public GraphVertex()
        {
            graph = In.OfType<GraphMessage>().Select(a => a.Graph).BindTo(this, a => a.Graph);

            _ = In
                .OfType<GraphChangeMessage>()
                .CombineLatest(In.OfType<GraphMessage>())
                .Select(a =>
                {
                    return a switch
                    {
                        (GraphChangeMessage { Change: AddRandomVertexChange change }, GraphMessage { Graph: Graph graph }) => (child: (IIdentifiableGraphDataObject)new Vertex(), graph),
                        _ => throw new NotImplementedException(),
                    };
                })
                .Subscribe(a =>
                {
                    switch (a.child)
                    {
                        case Vertex v:
                            a.graph.AddVertex(v);
                            break;
                        case Edge e:
                            a.graph.AddEdge(e);
                            break;
                    }
                });

        }

        public Graph? Graph { get; private set; }

        public void Dispose()
        {
            graph.Dispose();
        }
    }
}
