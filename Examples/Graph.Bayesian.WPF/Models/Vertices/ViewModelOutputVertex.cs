using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using Graph.Bayesian.WPF.Infrastructure;
using ReactiveUI;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public class ViewModelOutputVertex : Vertex
    {
        private readonly ReadOnlyObservableCollection<ViewModelResponse> collection;

        public ViewModelOutputVertex()
        {
            In
                .OfType<ViewModelResponseMessage>()
                .Subscribe(a =>
                {
                    var (@from, to, dateTime, value) = a;
                    LastResponse = a.Response;
                    LastResponseChange = DateTime.Now;
                    OnPropertyChanged(nameof(LastResponse));
                    OnPropertyChanged(nameof(LastResponseChange));
                });

            In
                .OfType<ViewModelResponseMessage>()
                .Select(a => a.Response)
                .ToObservableChangeSet()
                .Bind(out collection)
                .Subscribe();
        }

        public ViewModelResponse LastResponse { get; private set; }

        public ReadOnlyObservableCollection<ViewModelResponse> Responses => collection;

        public DateTime LastResponseChange { get; private set; }
    }
}
