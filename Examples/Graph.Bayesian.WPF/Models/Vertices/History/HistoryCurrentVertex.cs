using System;
using System.Linq;
using System.Reactive.Linq;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.Models.Vertices.History;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public class HistoryCurrentVertex : Vertex
    {
        public HistoryCurrentVertex()
        {
            In
                .OfType<HistoryCurrentMessage<PropertyChange>>()
                .Subscribe(a =>
                {               
                    Output = a.Data;
                    OnPropertyChanged(nameof(Output));
                });          
        }

        [MonitorPropertyChange(false)]
        public PropertyChange? Output { get; private set; }
    }
       
}
