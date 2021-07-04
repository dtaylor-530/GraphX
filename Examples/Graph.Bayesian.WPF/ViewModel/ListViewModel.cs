using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.Vertices;
using System;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using DynamicData;
using ReactiveUI;
using System.Reactive;
using System.Linq;
using System.Reactive.Subjects;
using System.Collections;
using System.ComponentModel;
using DynamicData.Binding;

namespace Graph.Bayesian.WPF.ViewModel
{
    public abstract class ListViewModel : ReactiveObject, IViewModel
    {
        public abstract ICollection Collection { get; }

        public abstract object? Selection { get; set; }
    }


    public class ListViewModel<T> : ListViewModel, ISubject<IChangeSet<T>, T> where T : Record, ISelected
    {
        private readonly ReadOnlyObservableCollection<T> collection;
        private readonly ReactiveService<IChangeSet<T>, T> subject = new();
        private T? selection;

        public ListViewModel(string key = "")
        {
            Key = key;
            subject.In.Select(a =>
            {
                return a;
            })
            .Bind(out collection).Subscribe();
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

                this.RaiseAndSetIfChanged(ref selection, (value as T) with { IsSelected = true });
                subject.Out.OnNext(selection);
            }
        }

        public override ICollection Collection => collection;

        public Guid Guid { get; }

        public string Key { get; }

        public void OnNext(IChangeSet<T> value) => subject.OnNext(value);
        public void OnCompleted() => subject.OnCompleted();
        public void OnError(Exception error) => subject.OnError(error);
        public IDisposable Subscribe(IObserver<T> observer) => subject.Subscribe(observer);
    }


    public class ListViewModel<T, TKey> : ListViewModel<T>, ISubject<IChangeSet<T, TKey>, T> where T : Record, ISelected where TKey : notnull
    {
        private readonly ReadOnlyObservableCollection<T> collection;
        private readonly ReactiveService<IChangeSet<T, TKey>, T> subject = new();

        public ListViewModel(string key, SortExpressionComparer<T> comparer):base(key)
        {
            Random random = new();

            subject.In
                .Select(a =>
                {
                    return a;
                })
                .Sort(comparer)
                .Bind(out collection).Subscribe(a =>
                {
                
                });
        }

        public override ICollection Collection => collection;

        public void OnNext(IChangeSet<T, TKey> value) => subject.OnNext(value);
    }
}
