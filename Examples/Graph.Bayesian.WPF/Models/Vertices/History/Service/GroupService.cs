using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.ViewModel;
using ReactiveUI;

namespace Graph.Bayesian.WPF.Models.Vertices.History
{

    public class SelectService<TValue, TKey> : Service<IChangeSet<ListViewModel<GroupViewItem>, HistoryModel.Group>, GroupViewItem>
    {
        // GroupViewItem? changeSet = null;

        public SelectService()
        {
            In
                .MergeMany(a => a)
                // Spurious messages can come in too quickly hence the arbitary delay
                .Throttle(TimeSpan.FromSeconds(0.5))
                .Select(a=>a?.Selected)
                .WhereNotNull()
                .ObserveOnDispatcher()
                .Subscribe(a =>
                {
                    Out.OnNext(a);

                });

        }
    }


    public class GroupService<T, TKey> : Service<OperationResult<T, TKey>, IChangeSet<ListViewModel<GroupViewItem>, HistoryModel.Group>>
       where T : IComparable
       where TKey : notnull
    {
        public GroupService(Func<GroupedItem<T, HistoryModel.Group, TKey>, bool> selected)
        {

            In
               .Select(result => result.ChangeSet)
               .Group(grp => grp.GroupKey)
               .Transform(grp => CreateGroup(grp, selected))
               .Subscribe(item =>
               {
                   Out.OnNext(item);
               });
        }

        private static ListViewModel<GroupViewItem> CreateGroup(IGroup<GroupedItem<T, HistoryModel.Group, TKey>, TKey, HistoryModel.Group> group, Func<GroupedItem<T, HistoryModel.Group, TKey>, bool> selected)
        {
            var viewModel = new ListViewModel<GroupViewItem, TKey>(group.Key.ToString());
            viewModel.OnNext(new ComparerInput<GroupViewItem>(SortExpressionComparer<GroupViewItem>.Descending(a => a.Value)));
            ToObservable(group, selected).Select(a => new ChangeSetInput<GroupViewItem, TKey>(a)).Subscribe(viewModel);


            return viewModel;

            static IObservable<IChangeSet<GroupViewItem, TKey>> ToObservable(IGroup<GroupedItem<T, HistoryModel.Group, TKey>, TKey, HistoryModel.Group> group, Func<GroupedItem<T, HistoryModel.Group, TKey>, bool> selected)
            {
                return group
                        .Cache
                        .Connect()
                        .Transform(a => new GroupViewItem(a.GroupKey.ToString(), a.Key.ToString(), selected(a), a.Value));

            }
        }
    }
}
