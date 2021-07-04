using System;
using System.Linq;
using System.Reactive.Linq;
using Graph.Bayesian.WPF.Infrastructure;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public class SelectServiceVertex : Vertex {

      public SelectServiceVertex() {

         In
            .OfType<VerticeselectedMessage>()
            .Subscribe(a =>
            {
               //LastId = a.From;
               //OnPropertyChanged(nameof(LastId));
            });

         In
            .OfType<VerticeselectedMessage>()
            .JoinRight(Types.WhereTypeIs<NavigationVertex>())
            .Subscribe(a =>
            {
               var ((@from, to, vertex), item2) = a;
               Out.OnNext(new NavigateVertexMessage(this.ID.ToString(), item2, vertex));
            });

      }

   }
}
