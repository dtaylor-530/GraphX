using System;
using System.Reactive.Linq;
using Graph.Bayesian.WPF.Infrastructure;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public class NavigationVertex : Vertex
    {

        public NavigationVertex()
        {

            In
               .OfType<NavigateVertexMessage>()
               .Subscribe(a =>
               {

                   var (from, _, vertex) = a;
                   Vertex = vertex;
                   OnPropertyChanged(nameof(Vertex));
               });

        }

        public Vertex Vertex { get; private set; }
    }
}