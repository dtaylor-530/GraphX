using System;
using ReactiveUI;
using Graph.Bayesian.WPF.Infrastructure;
using System.Reactive;

namespace Graph.Bayesian.WPF.ViewModel
{
    public class GraphViewModel : ReactiveSubject<Models.Graph, Unit>, IDisposable
    {
        private readonly ObservableAsPropertyHelper<Models.Graph> graph;

        public GraphViewModel()
        {
            In.Subscribe(a =>
            {
                Graph = a;
                this.RaisePropertyChanged(nameof(Graph));
            });

            graph = In.ToProperty(this, a => a.Graph);           
        }

        public Models.Graph Graph { get; private set; }

        public void Dispose()
        {
            graph.Dispose();
        }
    }
}
