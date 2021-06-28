using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.ViewModel;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public class FactoryOrderVertex : Vertex
    {
        public FactoryOrderVertex()
        {
            var add = ReactiveCommand.Create(() => new ListEdit(ListEditType.Add));

            add
                .JoinRight(TypesChangeSet.SelectOfType<FactoryVertex>())
                .Subscribe(a =>
                {
                    OutMessages.OnNext(new OrderMessage(this.ID.ToString(), string.Empty, DateTime.Now, new Order(Guid.NewGuid(), string.Empty, a.Item2)));
                });

            Add = add;
        }

        public ICommand Add { get; }
    }
}
