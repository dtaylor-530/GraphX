using System;
using System.Reactive.Linq;
using System.Linq;
using DynamicData;
using ReactiveUI;
using System.Reactive.Subjects;
using Graph.Bayesian.WPF.ViewModel;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    using Infrastructure;

    public record SelectionRequest(Guid Guid, string ProductId, string FactoryId);
    public record SelectionMessage(string From, string To, Selections Catalogue) : Message(From, To, DateTime.Now, Catalogue);

    public class SelectionVertex : Vertex, IObserver<IListService<Selection>>
    {
        private readonly ListViewModel<Selection> catalogueViewModel = new();
        private readonly Subject<IListService<Selection>> catalogueSubject = new();

        public SelectionVertex()
        {
            var dis = catalogueSubject
                .WhereNotNull()
                .Switch()
                .Select(a => a)
                .Select(a => new ChangeSetInput<Selection>(a))
                .Subscribe(catalogueViewModel.OnNext);

            In
                .OfType<SelectionMessage>()
                .Subscribe(a =>
                {
                    var (@from, to, dateTime, value) = a;
                    LastSelectionsChange = DateTime.Now;
                    OnPropertyChanged(nameof(LastSelectionsChange));
                });

            In
                .OfType<SelectionMessage>()
                .SelectMany(a => a.Catalogue.Value)
                .Merge((catalogueViewModel as IObservable<ListOutput<Selection>>)
                .Select(a => { return a.Selected; })
                .WhereNotNull())
                 .DistinctUntilChanged(a => a.Guid)
                .Select(a => (ListChange)new ItemChange<Selection>(a))
                .Merge(In
                .OfType<ListEditMessage>()
                .Select(a => (ListChange)new EditChange(a.ListEdit)))
                .Merge(In
                .OfType<CatalogueMessage>()
                .SelectMany(a => a.Catalogue.Selections).Select(a => new ItemChange<Selection>(new Selection(Guid.NewGuid(), a.ProductId, a.FactoryId, false))))
                .CombineLatest(catalogueSubject)
                .Subscribe(a =>
                {
                    var aaa = dis;
                    a.Second.OnNext(a.First);
                });

            In
                .OfType<ViewModelResponseMessage>()
                .Select(a => a.Response)
                .Select(a => new Selection(a.Guid, a.ProductId, a.FactoryId, false))
                .Distinct(a => a.Guid)
                .Select(a => (ListChange)new ItemChange<Selection>(a))
                .CombineLatest(catalogueSubject)
                .Subscribe(a =>
                {
                    a.Second.OnNext(a.First);
                });

            catalogueSubject
                .SelectMany(b =>
                        b.Filter(a => a.IsSelected)
                        .ToCollection()
                        .SelectMany(a => a)
                        .DistinctUntilChanged())
                .Subscribe(a =>
                {
                    Out.OnNext(new SelectionRequestMessage(this.ID.ToString(), a.FactoryId, DateTime.Now, new SelectionRequest(a.Guid, a.ProductId, a.FactoryId)));
                    LastOrderChange = DateTime.Now;
                    OnPropertyChanged(nameof(LastOrderChange));
                });
        }

        public ListViewModel ListViewModel => catalogueViewModel;

        public DateTime LastSelectionsChange { get; private set; }

        public DateTime LastOrderChange { get; private set; }

        public void OnNext(IListService<Selection> value)
        {
            catalogueSubject.OnNext(value);
        }
    }
}
