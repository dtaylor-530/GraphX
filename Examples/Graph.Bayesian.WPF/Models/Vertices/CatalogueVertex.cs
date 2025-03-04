﻿using System;
using System.Reactive.Linq;
using System.Linq;
using DynamicData;
using ReactiveUI;
using System.Reactive.Subjects;
using Graph.Bayesian.WPF.ViewModel;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    using Infrastructure;

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
           .Select(a => new ChangeSetInput<ProductToken>(a))
           .Subscribe(catalogueViewModel.OnNext);

            In
                .OfType<CatalogueMessage>()
                .Subscribe(a =>
                {
                    var (@from, to, dateTime, value) = a;
                    LastCatalogueChange = DateTime.Now;
                    OnPropertyChanged(nameof(LastCatalogueChange));
                });

            var a = (catalogueViewModel as IObservable<ListOutput<ProductToken>>)
                .Select(a => { return a.Selected; })
                .WhereNotNull();

            var b = In
               .OfType<ListEditMessage>()
               .Select(a => (ListChange)new EditChange(a.ListEdit));

            var c = In
                .OfType<CatalogueMessage>()
                .SelectMany(a => a.Catalogue.Selections);


            _ = a
                .Merge(c)
                .Select(a => (ListChange)new ItemChange<ProductToken>(a))
                .Merge(b)
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
                            Out.OnNext(new OrderMessage(this.ID.ToString(), a.FactoryId, DateTime.Now, new Order(Guid.NewGuid(), a.ProductId, a.FactoryId)));
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
