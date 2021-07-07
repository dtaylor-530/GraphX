using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.ViewModel;
using System;
using System.Linq;
using System.Reactive.Linq;
using static Graph.Bayesian.WPF.Models.Vertices.Pagination.EnumerableInputVertex;

namespace Graph.Bayesian.WPF.Models.Vertices.Pagination
{
    public record ListInputMessage(string From, string To, DateTime Sent, ListInput ListInput) : Message(From, To, Sent, ListInput);
    public record ListOutputMessage(string From, string To, DateTime Sent, ListOutput ListOutput) : Message(From, To, Sent, ListOutput);

    public class ListVertex : Vertex
    {
        private readonly ListViewModel<SelectableKeyRecord, string> listViewModel = new("Main");

        public ListVertex()
        {
            Out.OnNext(new ListOutputMessage(this.ID.ToString(), string.Empty, DateTime.Now, new ListOutput(0)));

            In
                .OfType<ListInputMessage>()
                .Subscribe(a =>
                {
                    listViewModel.OnNext(a.ListInput);
                });

            listViewModel.Subscribe(a =>
            {
                Out.OnNext(new ListOutputMessage(this.ID.ToString(), string.Empty, DateTime.Now, a));
            });
        }


        public ListViewModel ListViewModel => listViewModel;
    }
}
