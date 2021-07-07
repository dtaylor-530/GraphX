using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.ViewModel;

namespace Graph.Bayesian.WPF.Models.Vertices.Pagination
{

    public record PaginationLimitMessage(string From, string To, DateTime Sent, int Limit) : Message(From, To, Sent, Limit);
    public record PaginationCurrentMessage(string From, string To, DateTime Sent, int Current) : Message(From, To, Sent, Current);
    public record PaginationMaxPagesMessage(string From, string To, DateTime Sent, int MaxPages) : Message(From, To, Sent, MaxPages);

    public class PaginationVertex : Vertex
    {
        public PaginationVertex()
        {


            _ = In.OfType<PaginationLimitMessage>()
                .CombineLatest(
                    In.OfType<ListOutputMessage>(),
                    In.OfType<PaginationCurrentMessage>(),
                    In.OfType<PaginationMaxPagesMessage>())
                .Subscribe(de =>
                  {
                      var (a, b, c, d) = de;
                      Output = PaginationHelper.Paginate(b.ListOutput.Count, c.Current, a.Limit, d.MaxPages);
                      Out.OnNext(new ListInputMessage(this.ID.ToString(), string.Empty, DateTime.Now, new PageRequestInput(Output)));
                      OnPropertyChanged(nameof(Output));
                  });
        }
        public Pagination? Output { get; private set; }
    }
}
