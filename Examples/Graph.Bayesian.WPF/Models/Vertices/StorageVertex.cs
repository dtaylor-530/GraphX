using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.Vertices;
using ReactiveUI;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public record IsDirty(Guid Guid, bool Value);

    public record IsDirtyMessage(string From, string To, DateTime Sent, IReadOnlyCollection<IsDirty> Value) : Message(From, To, Sent, Value);

    public record SaveLoadRequest(IReadOnlyCollection<Guid> Guid);

    public record SaveLoadResponse(IReadOnlyCollection<Guid> Guid);

    public record SaveViewModelResponse(IReadOnlyCollection<Guid> Guid, bool Success) : SaveLoadResponse(Guid);

    public record SaveViewModelRequest(IReadOnlyCollection<Guid> Guid) : SaveLoadRequest(Guid);

    public record LoadViewModelRequest(IReadOnlyCollection<Guid> Guid) : SaveLoadRequest(Guid);

    public record LoadViewModelResponse(IReadOnlyCollection<Guid> Guid, bool Success, IViewModel ViewModel) : SaveLoadResponse(Guid);

    public record SaveLoadViewModelResponseMessage(string From, string To, DateTime Sent, SaveLoadResponse Request) : Message(From, To, Sent, Request);

    public record SaveLoadViewModelRequestMessage(string From, string To, DateTime Sent, SaveLoadRequest Request) : Message(From, To, Sent, Request);


    public class StorageVertex : Vertex
    {
        //readonly ListService<Product> cacheService = new(a => a.Guid.ToString());
        //readonly ListViewModel<Product> listViewModel;

        public StorageVertex()
        {
            //listViewModel = new();

            //cacheService
            //   .Subscribe(listViewModel.OnNext);

            In
                .OfType<SaveLoadViewModelRequestMessage>()           
                .MergeDifferent(In.OfType<IsDirtyMessage>())
                .MergeDifferent(In.OfType<IChangeSet<IViewModel>>())
                //.JoinRight(TypesChangeSet.SelectOfType<ViewModelOutputVertex>())
                .Select(tuple =>

                     tuple switch
                     {
                         ((SaveLoadViewModelRequestMessage { Request:{ Guid: { } guid } }, null), null) =>
                             (Message)new SaveLoadViewModelResponseMessage(this.ID.ToString(), string.Empty, DateTime.Now, new SaveViewModelResponse(guid, true)),
                         //(LoadViewModelRequest   { Guid: { } guid }) =>
                         //    new OrderMessage(this.ID.ToString(), factoryId, DateTime.Now, new Order(guid, request.ProductId, factoryId)),
                         //(null, { Value: { ProductId: { } productId, ViewModel: { } viewModel } product }) =>
                         //    new ViewModelResponseMessage(this.ID.ToString(), string.Empty, DateTime.Now, new ViewModelResponse(product.Guid, productId, product.FactoryId, viewModel)),
                         //(SelectionRequestMessage, ProductMessage) => null,
                         _ => throw new NotImplementedException()
                     }
                )
                .WhereNotNull()
                .Subscribe(a => Out.OnNext(a));


            //OutMessages.OfType<OrderMessage>()
            //    .Subscribe(orderMessage =>
            //    {
            //        Orders.Add(orderMessage.Order.ProductId);
            //    });

            //InMessages.OfType<ProductMessage>()
            //    .Subscribe(productMessage =>
            //    {
            //        Products.Add(productMessage.Value);
            //        //listViewModel.OnNext(new ChangeSet<Product>(new[] { new Change<Product>(ListChangeReason.Add, productMessage.Value) }));
            //    });
        }


        //public ObservableCollection<Product> Products { get; } = new();

        //public ObservableCollection<string> Orders { get; } = new();
    }

    public class StorageService
    {

    }
}
