using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.Models.Vertices.History;
using Graph.Bayesian.WPF.ViewModel;
using ReactiveUI;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public class HistoryVertex : Vertex
    {
        private readonly HistoryService<PropertyChange, Guid> historyService = HistoryServiceCreator<PropertyChange, Guid>
            .Create((a, b) => a?.UpDate < b.UpDate ? PositionType.Anterior : PositionType.Posterior);
        private readonly GroupService<PropertyChange, Guid> groupService = new(a => a.GroupKey == HistoryModel.Group.Current);
        private readonly SelectService<PropertyChange, Guid> selectService = new();
        private readonly ReadOnlyObservableCollection<ListViewModel<GroupViewItem>> collection;
        private readonly ReadOnlyObservableCollection<GroupViewItem> selectCollection;

        public HistoryVertex()
        {
            groupService
                .Select(a => a)
                .Sort(SortExpressionComparer<ListViewModel<GroupViewItem>>.Ascending(a =>
                {
                    return Enum.Parse(typeof(HistoryModel.Group), a.Key) switch
                    {
                        HistoryModel.Group.Ante => 3,
                        HistoryModel.Group.Current => 2,
                        HistoryModel.Group.Post => 1,
                        _ => throw new NotImplementedException("Not Implemented"),
                    };
                }))
                .ObserveOnDispatcher()
                .Bind(out collection)
                .Subscribe();

            selectService
                .ToObservableChangeSet()
                .ObserveOnDispatcher()
                .Bind(out selectCollection)
                .Subscribe();

            groupService
                .Subscribe(selectService);

            selectService
                .Select(a =>
                {

                    if (a.Value is PropertyChange change)
                        return new Input<PropertyChange, Guid>(change.Key, change);
                    return null;

                })
                .WhereNotNull()
                .Subscribe(a =>
                {
         
                        historyService.OnNext(a);
               
                });

            historyService
                .Subscribe(groupService);

            In
               .OfType<PropertyChangeMessage>()
               .Subscribe(a =>
               {
                   historyService.OnNext(new Input<PropertyChange, Guid>(a.Change.Key, a.Change));
               });

            In
               .OfType<MovementMessage>()
               //.Where(a => a.To == ID.ToString())
               .Subscribe(msg =>
               {
                   historyService.OnNext(new Input<Movement, Guid>(Guid.NewGuid(), msg.Movement));
               });


            historyService
               .Subscribe(vv =>
               {
                   Out.OnNext(new HistoryCurrentMessage<PropertyChange, Guid>(ID.ToString(), string.Empty, DateTime.Now, vv));
               });

        }

        public ReadOnlyObservableCollection<ListViewModel<GroupViewItem>> Collection => collection;

        public ReadOnlyObservableCollection<GroupViewItem> SelectCollection => selectCollection;
    }
}
