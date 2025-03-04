﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Graph.Bayesian.WPF.Infrastructure;
using ReactiveUI;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public record ViewModelResponse(Guid Guid, string ProductId, string FactoryId, Vertex ViewModel);

    public class CacheVertex : Vertex
    {
        //readonly ListService<Product> cacheService = new(a => a.Guid.ToString());
        //readonly ListViewModel<Product> listViewModel;

        public CacheVertex()
        {
            //listViewModel = new();

            //cacheService
            //   .Subscribe(listViewModel.OnNext);

            In
                .OfType<SelectionRequestMessage>()
                .MergeDifferent(In.OfType<ProductMessage>())
                //.JoinRight(TypesChangeSet.SelectOfType<ViewModelOutputVertex>())
                .Select(tuple =>

                     tuple switch
                     {
                         ({ Request: { Guid: { } guid, FactoryId: { } factoryId } request }, null)
                         when Products.SingleOrDefault(da => da.Guid == guid) is { } singleProduct =>
                             (Message)new ViewModelResponseMessage(this.ID.ToString(), string.Empty, DateTime.Now, new ViewModelResponse(guid, singleProduct.ProductId, factoryId, singleProduct.ViewModel)),
                         ({ Request: { Guid: { } guid, FactoryId: { } factoryId } request }, null) =>
                             new OrderMessage(this.ID.ToString(), factoryId, DateTime.Now, new Order(guid, request.ProductId, factoryId)),
                         (null, { Value: { ProductId: { } productId, ViewModel: { } viewModel } product }) =>
                             new ViewModelResponseMessage(this.ID.ToString(), string.Empty, DateTime.Now, new ViewModelResponse(product.Guid, productId, product.FactoryId, viewModel)),
                         (SelectionRequestMessage, ProductMessage) => null,
                         _ => throw new NotImplementedException()
                     }
                )
                .WhereNotNull()
                .Subscribe(a => Out.OnNext(a));


            Out.OfType<OrderMessage>()
                .Subscribe(orderMessage =>
                {
                    Orders.Add(orderMessage.Order.ProductId);
                });

            In.OfType<ProductMessage>()
                .Subscribe(productMessage =>
                {
                    Products.Add(productMessage.Value);
                    //listViewModel.OnNext(new ChangeSet<Product>(new[] { new Change<Product>(ListChangeReason.Add, productMessage.Value) }));
                });
        }


        public ObservableCollection<Product> Products { get; } = new();

        public ObservableCollection<string> Orders { get; } = new();
    }
}
