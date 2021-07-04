using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Graph.Bayesian.WPF.Infrastructure;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public class ClickServiceVertex : Vertex
    {

        public ClickServiceVertex()
        {
            In
               .OfType<ClickMessage>()
               .JoinRight(Types.AllTypes())
               .Subscribe(a =>
               {
                   var ((@from, _), item2) = a;
                   Out.OnNext(new IsSelectedMessage(ID.ToString(), item2, @from == item2));
               });
        }
    }
}
