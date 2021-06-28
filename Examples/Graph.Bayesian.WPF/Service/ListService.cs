using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.ViewModel;
using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Linq;
using DynamicData;
using System.Reactive.Subjects;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public record ListChange;
    public record EditChange(ListEdit Edit) : ListChange;
    public record ItemChange<T>(T Item) : ListChange where T : Record, ISelected;

    public interface IListService<T> : ISubject<ListChange, IChangeSet<T>> where T : ISelected
    {
    }

    public class ListService<T> : Service<ListChange, IChangeSet<T>>, IListService<T> where T : Record, ISelected
    {
        public ListService(Func<T, string> keyFunc)
        {
            SourceList<T> sourceList = new();

            _ = sourceList
              .Connect()
              .Subscribe(Out.OnNext);

            var dis2 = In
                        .Scan((default(ListChange), default(ItemChange<T>)), (a, b) => (b, b is ItemChange<T> change ? change : a.Item2))
                        .Subscribe(c =>
                        {
                            try
                            {
                                (ListChange change, ItemChange<T> itemChange) = c;
                                switch (change)
                                {
                                    case ItemChange<T> { Item: { } first }:
                                        {
                                            if (sourceList.Items.SingleOrDefault(ae => keyFunc(ae) == keyFunc(first)) is not { } single)
                                            {
                                                sourceList.Add(first);
                                            }
                                            else
                                            {
                                                // set all other IsSelected values to false
                                                if (first.IsSelected)
                                                    foreach (var prod in sourceList.Items)
                                                    {
                                                        if (keyFunc(prod) == keyFunc(first))
                                                            sourceList.Replace(single, first);
                                                        else if (keyFunc(prod) != keyFunc(first))
                                                            sourceList.Replace(prod, prod with { IsSelected = false });
                                                    }
                                                else
                                                    sourceList.Replace(single, first);
                                            }
                                            break;
                                        }
                                    case EditChange { Edit: { } first } when itemChange is { Item: { } item }:

                                        {
                                            var ee = first switch
                                            {
                                                ListEditMove move => new Action(() =>
                                                {
                                                    var index = sourceList.Items.IndexOf(item);

                                                    if (index == -1)
                                                        return;

                                                    sourceList.Move(index, NewMethod(index, sourceList.Count, move));
                                                }),
                                                ListEdit edit =>
                                                  edit.ListEditType switch
                                                  {
                                                      ListEditType.Subtract => new Action(() => { sourceList.Remove(item); }),
                                                      ListEditType.Add => new Action(() => { sourceList.Add(item); }),
                                                      _ => throw new NotImplementedException(),
                                                  }
                                            };
                                            ee.Invoke();
                                            break;
                                        }
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        });
        }

        private static int NewMethod(int index, int count, ListEditMove move)
        {
            var newIndex = index + (move.ListEditType switch
                                     {
                                         ListEditType.Up => -1,
                                         ListEditType.Down => 1,
                                         _ => throw new Exception("dfsd")
                                     });


            return (count + newIndex) % count;

        }
    }
}
