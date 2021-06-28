using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.Models.Vertices;

namespace Graph.Bayesian.WPF.Models.Vertexs {
   public class SelectServiceVertex : Vertex {

      public SelectServiceVertex() {

         InMessages
            .OfType<VertexSelectedMessage>()
            .Subscribe(a =>
            {
               //LastId = a.From;
               //OnPropertyChanged(nameof(LastId));
            });

         InMessages
            .OfType<VertexSelectedMessage>()
            .JoinRight(TypesChangeSet.SelectOfType<NavigationVertex>())
            .Subscribe(a =>
            {
               var ((@from, to, vertex), item2) = a;
               OutMessages.OnNext(new NavigateVertexMessage(this.ID.ToString(), item2, vertex));
            });

      }

   }
}
