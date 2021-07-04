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
        private readonly HistoryService<PropertyChange, Guid> history = HistoryServiceCreator<PropertyChange, Guid>
            .Create((a, b) => a?.UpDate < b.UpDate ? PositionType.Anterior : PositionType.Posterior);
        private readonly GroupService<PropertyChange, Guid> groupService = new();
        private readonly ReadOnlyObservableCollection<ListViewModel<GroupViewItem>> collection;
        private readonly ReadOnlyObservableCollection<GroupViewItem> selectCollection;

        public HistoryVertex()
        {
            Random r = new();
            groupService
                .Select(a => a)
                .Sort(SortExpressionComparer<ListViewModel<GroupViewItem>>.Ascending(a =>
                {
                    return Enum.Parse(typeof(PositionalModel.Group), a.Key) switch
                    {
                        PositionalModel.Group.Ante => 3,
                        PositionalModel.Group.Current => 2,
                        PositionalModel.Group.Post => 1,
                        _ => throw new NotImplementedException("Not Implemented"),
                    };
                }))
                .Bind(out collection)
                .MergeMany(a => a)
                .Do(a =>
                {
                    if (a.Value is PropertyChange change)
                        //    Out.OnNext(new HistoryMessage<PropertyChange>(this.ID.ToString(), string.Empty, DateTime.Now, change));
                        history.OnNext(new Input<PropertyChange, Guid>(change.Key, change));
                })
                .ToObservableChangeSet()
                .Bind(out selectCollection)
                .Subscribe();

            history.Subscribe(groupService);

            In
               .OfType<PropertyChangeMessage>()
               .Subscribe(a =>
               {
                   history.OnNext(new Input<PropertyChange, Guid>(a.Change.Key, a.Change));
               });

            In
               .OfType<MovementMessage>()
               .Where(a => a.To == ID.ToString())
               .Subscribe(msg =>
               {
                   history.OnNext(new Input<Movement, Guid>(Guid.NewGuid(), msg.Movement));
               });


            history
               .Where(a => a.Success)
               .SelectMany(a => a.Value.Where(a => a.Current.GroupKey.Equals(PositionalModel.Group.Current) && a.Reason == ChangeReason.Add).Select(a => a.Current))
               .Subscribe(vv =>
               {
                   Out.OnNext(new HistoryCurrentMessage<PropertyChange>(ID.ToString(), vv.Value.ParentId.ToString(), DateTime.Now, vv.Value));
               });

        }

        public ReadOnlyObservableCollection<ListViewModel<GroupViewItem>> Collection => collection;

        public ReadOnlyObservableCollection<GroupViewItem> SelectCollection => selectCollection;
    }


}
