using System;
using System.Reactive.Linq;
using System.Linq;
using DynamicData;
using ReactiveUI;
using System.Reactive.Subjects;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    using Infrastructure;
    using ViewModel;

    public record CatalogueMessage(string From, string To, Catalogue Catalogue) : Message(From, To, DateTime.Now, Catalogue);

    public class CatalogueListViewModel : ListViewModel<ProductToken> { };

    public class CatalogueVertex : Vertex, IObserver<IListService<ProductToken>>
    {
        private readonly CatalogueListViewModel catalogueViewModel = new();
        private readonly Subject<IListService<ProductToken>> catalogueSubject = new();

        public CatalogueVertex()
        {

           catalogueSubject
          .WhereNotNull()
          .SelectMany(a => a)
          .Subscribe(catalogueViewModel.OnNext);

            InMessages
                .OfType<CatalogueMessage>()
                .Subscribe(a =>
                {
                    var (@from, to, dateTime, value) = a;
                    LastCatalogueChange = DateTime.Now;
                    OnPropertyChanged(nameof(LastCatalogueChange));
                });

            InMessages
                .OfType<CatalogueMessage>()
                .SelectMany(a => a.Catalogue.Selections)
                .Merge(catalogueViewModel
                .Select(a => { return a; })
                .WhereNotNull())
                .Select(a => (ListChange)new ItemChange<ProductToken>(a))
                .Merge(InMessages
               .OfType<ListEditMessage>()
               .Select(a => (ListChange)new EditChange(a.ListEdit)))
                .CombineLatest(catalogueSubject)
                .Subscribe(a => a.Second.OnNext(a.First));

 

            catalogueSubject
                .Subscribe(b =>
                        b.Filter(a => a.IsSelected)
                        .ToCollection()
                        .SelectMany(a => a)
                        .DistinctUntilChanged()
                        .Subscribe(a =>
                        {
                            OutMessages.OnNext(new OrderMessage(this.ID.ToString(), a.FactoryId, DateTime.Now, new Order(Guid.NewGuid(), a.ProductId, a.FactoryId)));
                            LastOrderChange = DateTime.Now;
                            OnPropertyChanged(nameof(LastOrderChange));
                        }));
        }

        public CatalogueListViewModel Catalogue => catalogueViewModel;

        public DateTime LastCatalogueChange { get; private set; }

        public DateTime LastOrderChange { get; private set; }

        public void OnNext(IListService<ProductToken> value)
        {
            catalogueSubject.OnNext(value);
        }
    }



}
