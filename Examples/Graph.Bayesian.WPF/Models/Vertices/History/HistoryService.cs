using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.Models.Vertices.History;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public abstract class HistoryService<T, TKey> : Service<IInput, OperationResult<T, TKey>>
        where T : IEquatable<T>, IKey<TKey>, IComparable
        where TKey : notnull
    {

        protected HistoryService()
        {
            In
                .Scan(Create(),
                 (a, b) =>
                 {
                     return OperationResult(a.Item1, b, GetPositionType) switch
                     {
                         Failure<OperationFailureReason> failure => (a.Item1, null, failure),
                         ChangeSet<GroupedItem<T, PositionalModel.Group, TKey>, TKey> changeSet => (a.Item1, changeSet, null),
                         _ => throw new ArgumentOutOfRangeException(),
                     };
                 })
                .Select(a => new OperationResult<T, TKey>(DateTime.Now, a.Item3 == null, a.Item2, a.Item3))
                .Subscribe(a => Out.OnNext(a));

            static (
                PositionalModel<T, TKey>,
                ChangeSet<GroupedItem<T, PositionalModel.Group, TKey>, TKey>?,
                Failure<OperationFailureReason>?)
                Create() => (new PositionalModel<T, TKey>(a => a.Key), new ChangeSet<GroupedItem<T, PositionalModel.Group, TKey>, TKey>(), default);
        }

        private static object OperationResult(PositionalModel<T, TKey> model, IInput input, Func<T?, T, PositionType> func)
        {
            Change<GroupedItem<T, PositionalModel.Group, TKey>, TKey>[] changes;

            switch (input)
            {
                case IInput<T, TKey> { Value: { } value } when model.Current is null:
                    changes = model.AddToCurrent(value);
                    break;
                case IInput<T, TKey> { Key: { } key } when model.Keys.Contains(key):
                    changes = model.SelectByKey(key);
                    break;
                case IInput<T, TKey> { Value: { } value } when func(model.Current, value) == PositionType.Anterior:
                    changes = model.AddToAnterior(value);
                    break;
                case IInput<T, TKey> { Value: { } value } when func(model.Current, value) == PositionType.Posterior:
                    changes = model.AddToPosterior(value);
                    break;
                case IInput<Movement, TKey> { Value: Movement.Forward } message:
                    if (model.CanMoveForward())
                        changes = model.MoveForeward().ToArray();
                    else
                        return new Failure<OperationFailureReason>(new[] { input }, OperationFailureReason.MovedToFarForward);
                    break;
                case IInput<Movement, TKey> { Value: Movement.Backward } message:
                    if (model.CanMoveBackward())
                        changes = model.MoveBackward().ToArray();
                    else
                        return new Failure<OperationFailureReason>(new[] { input }, OperationFailureReason.MovedTooFarBack);
                    break;
                default:
                    return new Failure<OperationFailureReason>(new[] { input }, OperationFailureReason.WeirdConfiguration);
            }
            return new ChangeSet<GroupedItem<T, PositionalModel.Group, TKey>, TKey>(changes.OrderBy(a => a.Reason));
        }

        protected abstract PositionType GetPositionType(T? current, T value);
    }
}
