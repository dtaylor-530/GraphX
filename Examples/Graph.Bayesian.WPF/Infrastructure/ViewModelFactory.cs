using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Fasterflect;
using Graph.Bayesian.WPF.Models.Vertices;

namespace Graph.Bayesian.WPF.Infrastructure
{

    public interface ISelected { bool IsSelected { get; init; } }

    public record Record;
    public record Order(Guid ProductGuid, string ProductId, string FactoryId) : Record;
    public record ProductToken(string ProductId, string FactoryId, bool IsSelected) : Record, ISelected;
    public record Selection(Guid Guid, string ProductId, string FactoryId, bool IsSelected) : Record, ISelected;
    public record Catalogue(string FactoryId, IReadOnlyCollection<ProductToken> Selections);
    public record Selections(IReadOnlyCollection<Selection> Value);
    public record Product(Guid Guid, string ProductId, string FactoryId, Vertex? ViewModel) : Record;

    public class ViewModelFactory : Service<Order, Product>, IFactory
    {
        readonly ReplaySubject<Catalogue> catalogues = new();
        readonly Lazy<GraphFactory> graphFactory = new(() => (GraphFactory)typeof(GraphFactory).CreateInstance());

        public ViewModelFactory(string id)
        {
            var methods = typeof(GraphFactory).GetMethods().Where(a => a.Name.Contains("Create"));
            var productsSelection = methods.Select(a => new ProductToken(a.Name, id, false)).ToArray();

            catalogues.OnNext(new Catalogue(id, productsSelection));

            In.Subscribe(a =>
            {
                var graph = (Models.Graph?)methods.Single(ae => ae.Name == a.ProductId).Invoke(graphFactory.Value, Array.Empty<object>());
                var graphViewModel = new GraphVertex();
                graphViewModel.OnNext(new GraphMessage(string.Empty, graphViewModel.ID.ToString(), DateTime.Now, graph));
                Out.OnNext(new Product(a.ProductGuid, a.ProductId, a.FactoryId, graphViewModel));
            });
        }

        public IDisposable Subscribe(IObserver<Catalogue> observer)
        {
            return catalogues.Subscribe(observer);
        }
    }

    public class ViewModelCreationFactory : Service<Order, Product>, IFactory
    {
        readonly ReplaySubject<Catalogue> catalogues = new();

        public ViewModelCreationFactory(string id)
        {
            catalogues.OnNext(new Catalogue(id, new ProductToken[] { new ProductToken("New", id, false) }));

            In.Subscribe(a =>
            {
                var graph = GraphFactory.BuildEmpty();
                var graphViewModel = new GraphVertex();
                graphViewModel.OnNext(new GraphMessage(string.Empty, graphViewModel.ID.ToString(), DateTime.Now, graph));
                Out.OnNext(new Product(Guid.NewGuid(), a.ProductId, a.FactoryId, graphViewModel));
            });
        }

        public IDisposable Subscribe(IObserver<Catalogue> observer)
        {
            return catalogues.Subscribe(observer);
        }
    }

    public interface IFactory : IObserver<Order>, IObservable<Product>, IObservable<Catalogue>
    {
    }
}
