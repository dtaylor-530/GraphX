using Graph.Bayesian.WPF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public class IsDirtyVertex : Vertex
    {

        public IsDirtyVertex()
        {
            In
             .OfType<PropertyChangeMessage>()
             .Subscribe(a =>
             {
                 //OutMessages.OnNext(new IsDirtyMessage(this.ID.ToString(), string.Empty, DateTime.Now, new IsDirty[] { new IsDirty(a.Change.ParentId, true) }));
             });
        }
    }
}
