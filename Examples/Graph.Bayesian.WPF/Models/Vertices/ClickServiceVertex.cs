using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Graph.Bayesian.WPF.Infrastructure;

namespace Graph.Bayesian.WPF.Models.Vertexs {

   public class ClickServiceVertex : Vertex {

      public ClickServiceVertex()
      {
         InMessages
            .OfType<ClickMessage>()
            .JoinRight(TypesChangeSet.SelectAll())
            .Subscribe(a =>
            {
               var ((@from, _), item2) = a;
               OutMessages.OnNext(new IsSelectedMessage(this.ID.ToString(), item2, @from == item2));
            });
      }
   }
}
