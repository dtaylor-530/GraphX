using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.Models.Vertices.History;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public class HistoryChangeSetVertex : Vertex
    {
        public HistoryChangeSetVertex()
        {
            In
                .OfType<HistoryCurrentMessage<PropertyChange, Guid>>()
                .Subscribe(a =>
                {
                    OnPropertyChanged(nameof(LastChanges));
                    CurrentSelected = a.Data.Current;
                    OnPropertyChanged(nameof(CurrentSelected));      
                    CurrentChanges = a.Data.Array;
                    OnPropertyChanged(nameof(CurrentChanges));
                    LastChanges = CurrentChanges;
                });          
        }

        [MonitorPropertyChange(false)]
        public IEnumerable? CurrentChanges { get; private set; }      
        
        [MonitorPropertyChange(false)]
        public IEnumerable? LastChanges { get; private set; }

        [MonitorPropertyChange(false)]
        public PropertyChange? CurrentSelected { get; private set; }
    }
       
}
