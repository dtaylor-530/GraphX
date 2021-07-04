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
    public class GroupService<T, TKey> : Service<OperationResult<T, TKey>, IChangeSet<ListViewModel<GroupViewItem>, PositionalModel.Group>>
       where T : IComparable
       where TKey : notnull
    {
        public GroupService()
        {
            In
               .Select(result => result.Value)
               .WhereNotNull()
               .Group(grp => grp.GroupKey)
               .Transform(grp => CreateGroup(grp))
               .Subscribe(item =>
               {
                   Out.OnNext(item);
               });
        }

        private static ListViewModel<GroupViewItem> CreateGroup(IGroup<GroupedItem<T, PositionalModel.Group, TKey>, TKey, PositionalModel.Group> group)
        {
            var viewModel = new ListViewModel<GroupViewItem, TKey>(group.Key.ToString(), SortExpressionComparer<GroupViewItem>.Descending(a => a.Value));
            ToObservable(group).Subscribe(viewModel);
            return viewModel;

            static IObservable<IChangeSet<GroupViewItem, TKey>> ToObservable(IGroup<GroupedItem<T, PositionalModel.Group, TKey>, TKey, PositionalModel.Group> group)
            {
                return group.Cache
                            .Connect()
                            .Transform(a => new GroupViewItem(a.GroupKey.ToString(), a.Key.ToString(), false, a.Value));

            }
        }
    }
}
