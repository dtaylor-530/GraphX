using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.ViewModel;
using System;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using DynamicData;
using ReactiveUI;
using System.Reactive;
using System.Linq;
using System.Reactive.Subjects;
using System.Collections;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public abstract class ListViewModel: ReactiveObject
    {
        public abstract ICollection Collection { get; }

        public abstract object? Selection { get; set; }
    }

    public class ListViewModel<T> : ListViewModel, ISubject<IChangeSet<T>, T> where T : Record, ISelected
    {
        private readonly ReadOnlyObservableCollection<T> collection;
        private readonly ReactiveSubject<IChangeSet<T>, T> subject = new();
        private T? selection;

        public ListViewModel()
        {
            subject.In.Select(a=> { 
                return a; }).Bind(out collection).Subscribe();
        }

        public override object? Selection
        {
            get => selection;
            set
            {
                if (value == null && selection != null)
                    return;
                if (value == null)
                    value = collection.FirstOrDefault();
                if (value == null || value == selection)
                    return;

                T val = (value as T) with { IsSelected = true };
                subject.Out.OnNext(val);
                this.RaiseAndSetIfChanged(ref selection, val);
            }
        }

        public override ICollection Collection => collection;

        public Guid Guid { get; }

        public void OnCompleted() => subject.OnCompleted();
        public void OnError(Exception error) => subject.OnError(error);
        public void OnNext(IChangeSet<T> value) => subject.OnNext(value);
        public IDisposable Subscribe(IObserver<T> observer) => subject.Subscribe(observer);
    }
}
