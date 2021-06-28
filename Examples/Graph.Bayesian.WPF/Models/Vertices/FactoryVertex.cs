using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.Models.Vertices;
using Graph.Bayesian.WPF.ViewModel;

namespace Graph.Bayesian.WPF.Models
{

    public class FactoryVertex : Vertex
    {
        private readonly Subject<IFactory> subject = new();

        public FactoryVertex()
        {
            subject
                .SelectMany(service => service as IObservable<Catalogue>)
                .JoinRight(TypesChangeSet.SelectOfType(typeof(CatalogueVertex)))
                .Select(a =>
                {
                    return new CatalogueMessage(this.ID.ToString(), a.Item2, a.Item1);
                })
               .Subscribe(OutMessages.OnNext);

            InMessages
                .OfType<OrderMessage>()
                .Subscribe(a =>
                {
                    var (@from, to, dateTime, value) = a;
                    Order = value;
                    LastOrderChange = DateTime.Now;
                    OnPropertyChanged(nameof(Order));
                    OnPropertyChanged(nameof(LastOrderChange));
                });

            InMessages
                .OfType<OrderMessage>()
                .WithLatestFrom(subject)
                .Subscribe(a =>
                {
                    a.Second.OnNext(a.First.Order);
                });

            subject
                .SelectMany(service => service as IObservable<Product>)
                .Select(a =>
                {
                    return new ProductMessage(this.ID.ToString(), string.Empty, a);
                })
                .Subscribe(OutMessages.OnNext);
        }

        public Order Order { get; private set; }

        public DateTime LastOrderChange { get; private set; }

        public void OnNext(IFactory value)
        {
            subject.OnNext(value);
        }
    }
}
