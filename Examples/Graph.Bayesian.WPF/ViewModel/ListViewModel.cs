using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.Vertices;
using System;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using DynamicData;
using ReactiveUI;
using System.Linq;
using System.Reactive.Subjects;
using System.Collections;
using System.Collections.Generic;
using Graph.Bayesian.WPF.Models.Vertices.Pagination;

namespace Graph.Bayesian.WPF.ViewModel
{

    public record ListInput(object Value);
    public record PageRequestInput(Pagination PageRequest) : ListInput(PageRequest), IPageRequest
    {
        public int Page => PageRequest.Page;
        public int Size => PageRequest.Size;
    }
    public record ComparerInput<T>(IComparer<T> Comparer) : ListInput(Comparer);
    public record FilterInput<T>(IFilter<T> Filter) : ListInput(Filter);
    public record ChangeSetInput<T, TKey>(IChangeSet<T, TKey> ChangeSet) : ListInput(ChangeSet) where TKey : notnull;
    public record ChangeSetInput<T>(IChangeSet<T> ChangeSet) : ListInput(ChangeSet);
    public record ListOutput<T>(T? Selected, int Count) : ListOutput(Count);
    public record ListOutput(int Count);


    public abstract class ListViewModel : ReactiveObject, IViewModel
    {
        public abstract ICollection Collection { get; }

        public abstract object? Selection { get; set; }
    }


    public class ListViewModel<T> : ListViewModel, ISubject<ListInput, ListOutput<T>> where T : Record, ISelected
    {
        private readonly ReadOnlyObservableCollection<T> collection;
        private readonly ReactiveService<ListInput, ListOutput<T>> subject = new();
        private T? selection;

        public ListViewModel(string key = "")
        {
            Key = key;
            subject.In
                .Select(a => a as ChangeSetInput<T>)
                .WhereNotNull()
                .Select(a =>
                {
                    return a.ChangeSet;
                })
            .Bind(out collection)
            .Subscribe(a =>
            {
                subject.Out.OnNext(new ListOutput<T>(selection, collection.Count));
            });


            subject
               .In
                .Select(a => a.Value as ChangeSetInput<T>)
                .WhereNotNull()
                .Select(a =>
                {
                    return a.ChangeSet;
                })
               .Filter(a =>
               {
                   return a.IsSelected;
               })
               .Transform(a => Observable.Return(a))
               .MergeMany(a => a)
               .Subscribe(a =>
               {
                   Selection = a;
               });
        }

        public override object? Selection
        {
            get
            {
                if (selection == null)
                {
                }
                return selection;
            }
            set
            {
                if (value == null && selection != null)
                    return;
                if (value == null)
                    value = collection.FirstOrDefault();
                if (value == null || value == selection)
                    return;

                if (selection == null)
                {

                }
                if (value != null)
                {
                    var change = value != null ? (value as T)! with { IsSelected = true } : null;
                    this.RaiseAndSetIfChanged(ref selection, change);
                    subject.Out.OnNext(new ListOutput<T>(change, collection.Count));
                }
            }
        }

        public T? SelectionT => selection;

        public override ICollection Collection => collection;

        public Guid Guid { get; }

        public string Key { get; }

        public virtual void OnNext(ListInput value) => subject.OnNext(value);
        public virtual void OnCompleted() => subject.OnCompleted();
        public virtual void OnError(Exception error) => subject.OnError(error);
        public virtual IDisposable Subscribe(IObserver<ListOutput<T>> observer) => subject.Subscribe(observer);
    }


    public class ListViewModel<T, TKey> : ListViewModel<T>, ISubject<ListInput, ListOutput<T>> where T : Record, ISelected where TKey : notnull
    {
        private readonly ReadOnlyObservableCollection<T> collection;
        private readonly ReactiveService<ListInput, ListOutput<T>> subject = new();

        public ListViewModel(string key) : base(key)
        {
            Random random = new();

            subject
                .In
                .OfType<ChangeSetInput<T, TKey>>().Select(a => a.ChangeSet)
                     .ToCollection()
                     .Select(a => a.Count)
                     .Subscribe(a =>
                     {
                         subject.Out.OnNext(new ListOutput<T>(this.SelectionT, a));
                     });

            subject
                .In

                .OfType<ChangeSetInput<T, TKey>>().Select(a => a.ChangeSet)
                .Filter(subject
                .In
                .OfType<FilterInput<T>>().Select(a => new Func<T, bool>(c => a.Filter.Filter(c))))
                .Sort(subject
                .In
                .OfType<ComparerInput<T>>().Select(a => a.Comparer))
                .Page(subject
                .In
                .OfType<PageRequestInput>().Select(a => a.PageRequest))
                .Bind(out collection)
                .Subscribe();

            subject
                .In
                .OfType<ChangeSetInput<T, TKey>>().Select(a => a.ChangeSet)
                .Filter(a =>
                {
                    return a.IsSelected;
                })
               .Transform(a => Observable.Return(a))
               .MergeMany(a => a)
               .Subscribe(a =>
               {
                   Selection = a;
               });
        }

        public override ICollection Collection => collection;

        public override void OnCompleted() => subject.OnCompleted();

        public override void OnError(Exception error) => subject.OnError(error);

        public override void OnNext(ListInput value) => subject.OnNext(value);

        public override IDisposable Subscribe(IObserver<ListOutput<T>> observer) => subject.Subscribe(observer);
    }
}
