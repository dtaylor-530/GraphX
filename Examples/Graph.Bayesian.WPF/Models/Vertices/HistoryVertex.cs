using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using Graph.Bayesian.WPF.Infrastructure;
using ReactiveUI;

namespace Graph.Bayesian.WPF.Models
{
    public class HistoryVertex : Vertex
    {
        public HistoryService<PropertyChange, DateTime> History { get; } = HistoryServiceCreator<PropertyChange, DateTime>
            .Create((a, b) => a?.UpDate > b.UpDate ? PositionType.Posterior : PositionType.Anterior);

        public HistoryVertex()
        {
            InMessages
               .OfType<PropertyChangeMessage>()
               .Subscribe(a =>
               {
                   History.OnNext(new Input<PropertyChange>(a.Change));
               });

            InMessages
               .OfType<MovementMessage>()
               .Where(a => a.To == this.ID.ToString())
               .Subscribe(msg =>
               {
                   History.OnNext(new Input<Movement>(msg.Movement));
               });

            History
               .Where(a => a.Success)
               .SelectMany(a => a.Value.Where(a => a.Current.GroupKey.Equals(PositionalModel.Group.Current)).Select(a => a.Current))
               .Subscribe(vv =>
               {
                   OutMessages.OnNext(new HistoryMessage<PropertyChange>(this.ID.ToString(), vv.Value.ParentId.ToString(), DateTime.Now, vv.Value));
               });
        }
    }

    public enum PositionType
    {
        Anterior,
        Posterior,
    }

    public enum Movement
    {
        Forward,
        Backward,
        Clear
    }


    public record PropertyChange(DateTime Key, string ParentId, string Name, DateTime UpDate, object Value) : Change(UpDate, Value), IKey<DateTime>;

    public record Change(DateTime UpDate, object Value);

    public record Input<T>(T Value) : IInput<T>;

    public interface IInput<T> : IInput { T Value { get; } }

    public interface IInput { }

    public record PropertyChangeMessage(string From, string To, DateTime Sent, PropertyChange Change) : Message( From,To, Sent, Change);

    public record OutState<T>(ChangeSet<GroupedItem<Timestamped<T>, PositionalModel.Group>, DateTime> ChangeSet);

    public record Result<TOut, TFailure>(DateTime DateTime, bool Success, TOut? Value, TFailure? Failure) where TFailure : Failure;

    public record OperationResult<T, TKey>(
        DateTime DateTime,
        bool Success,
        ChangeSet<GroupedItem<T, PositionalModel.Group>, TKey>? Value,
        Failure<OperationFailureReason>? Failure) :
        Result<ChangeSet<GroupedItem<T, PositionalModel.Group>, TKey>, Failure<OperationFailureReason>>(DateTime, Success, Value, Failure)
    where TKey : notnull;

    public enum OperationFailureReason
    {
        None, MovedToFarForward, MovedTooFarBack, WeirdConfiguration
    }

    public record GroupedItem<T, TKey>(TKey GroupKey, T Value);

    public record Failure<T, TReason>(T InValue, TReason Reason) : Failure(InValue) where T : IEnumerable<IInput?>;

    public record Failure<TReason>(IEnumerable<IInput> InValue, TReason Reason) : Failure<IEnumerable<IInput>, TReason>(InValue, Reason);

    public record Failure(IEnumerable<IInput> Value);

    public record Output<TModel, TChanges>(TModel Model, TChanges Changes, Failure? Failure);

    public record GroupedItem(string Key, ICollection Collection);

    public record GroupItem(string GroupKey, object Value);

    public class HistoryServiceCreator<T, TKey> : HistoryService<T, TKey>
        where T : IEquatable<T>, IKey<TKey>
        where TKey : notnull
    {
        private readonly Func<T?, T, PositionType> func;

        private HistoryServiceCreator(Func<T?, T, PositionType> func)
        {
            this.func = func;
        }

        protected override PositionType GetPositionType(T? current, T value)
        {
            return func(current, value);
        }

        public static HistoryService<T, TKey> Create(Func<T?, T, PositionType> func)
        {
            return new HistoryServiceCreator<T, TKey>(func);
        }
    }

    public abstract class HistoryService<T, TKey> : IObserver<IInput>, IObservable<OperationResult<T, TKey>>
        where T : IEquatable<T>, IKey<TKey>
        where TKey : notnull
    {
        private readonly Subject<IInput> valuesSubject = new();
        private readonly Subject<OperationResult<T, TKey>> subject = new();
        private readonly ReadOnlyObservableCollection<GroupedItem> collection;

        protected HistoryService()
        {
            valuesSubject
               .Scan(Create(),
                  (a, b) =>
                  {
                      return OperationResult(a.Item1, b, GetPositionType) switch
                      {
                          Failure<OperationFailureReason> failure => (a.Item1, null, failure),
                          ChangeSet<GroupedItem<T, PositionalModel.Group>, TKey> changeSet => (a.Item1, changeSet, null),
                          _ => throw new ArgumentOutOfRangeException(),
                      };
                  })
               .Select(a => new OperationResult<T, TKey>(DateTime.Now, a.Item3 == null, a.Item2, a.Item3))
               .Do(a => subject.OnNext(a))
               .Select(a => a.Value)
               .WhereNotNull()
               .Group(a => a.GroupKey)
               .Transform(a => Group(a))
               .Bind(out collection)
               .Subscribe();
        }

        public ReadOnlyObservableCollection<GroupedItem> Collection => collection;

        private static GroupedItem Group(IGroup<GroupedItem<T, PositionalModel.Group>, TKey, PositionalModel.Group> a)
        {
            a.Cache.Connect().Transform(a=>new GroupItem(a.GroupKey.ToString(), a.Value)).Bind(out ReadOnlyObservableCollection<GroupItem> coll).Subscribe();
            return new GroupedItem(a.Key.ToString(), coll);
        }

        private static object OperationResult(PositionalModel<T, TKey> model, IInput input, Func<T?, T, PositionType> func)
        {
            ChangeSet<GroupedItem<T, PositionalModel.Group>, TKey> changeSet = new();

            switch (input)
            {
                case IInput<T> { Value: { } value } when func(model.Current, value) == PositionType.Anterior:
                    changeSet.AddRange(model.AddToAnterior(value).OrderBy(a=>a.Reason));
                    break;
                case IInput<T> { Value: { } value } when func(model.Current, value) == PositionType.Posterior:
                    changeSet.AddRange(model.AddToPosterior(value));
                    break;
                case (IInput<Movement> { Value: Movement.Forward } message):
                    if (model.CanMoveForward())
                        changeSet.AddRange(model.MoveForeward());
                    else
                        return new Failure<OperationFailureReason>(new[] { input }, OperationFailureReason.MovedToFarForward);
                    break;
                case (IInput<Movement> { Value: Movement.Backward } message):
                    if (model.CanMoveBackward())
                        changeSet.AddRange(model.MoveBackward());
                    else
                        return new Failure<OperationFailureReason>(new[] { input }, OperationFailureReason.MovedTooFarBack);
                    break;
                default:
                    return new Failure<OperationFailureReason>(new[] { input }, OperationFailureReason.WeirdConfiguration);
            }
            return changeSet;
        }

        protected abstract PositionType GetPositionType(T? current, T value);

        private static (
            PositionalModel<T, TKey>,
            ChangeSet<GroupedItem<T, PositionalModel.Group>, TKey>?,
            Failure<OperationFailureReason>?)
            Create() => (new PositionalModel<T, TKey>(a => a.Key), new ChangeSet<GroupedItem<T, PositionalModel.Group>, TKey>(), default);

        public void OnNext(IInput value)
        {
            valuesSubject.OnNext(value);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<OperationResult<T, TKey>> observer)
        {
            return subject.Subscribe(observer);
        }
    }



    public class PositionalModel<T, TKey> : PositionalModel
       where TKey : notnull
       where T : IEquatable<T>
    {
        private readonly Func<T, TKey> _keyFunc;
        //private readonly Func<T, TR> _createFunc;

        private readonly Stack<T> post = new();
        private readonly Stack<T> ante = new();
        private T? current;

        public PositionalModel(Func<T, TKey> keyFunc) //, Func<T, TR> createFunc)
        {
            _keyFunc = keyFunc;
            //_createFunc = createFunc;
        }

        public IReadOnlyCollection<T> Post => post;

        public IReadOnlyCollection<T> Ante => ante;

        public T? Current => current;

        /// <summary>
        /// removes all values from remaining and appends to complete stack
        /// </summary>
        /// <param name="value"></param>
        public IEnumerable<Change<GroupedItem<T, Group>, TKey>> AddToAnterior(T value)
        {
            var create = value;

            // ignore values that match the start of the current set
            if (ante.Any() == false)
            {
            }
            else if (post.Any() && (post.Peek()?.Equals(create) ?? false))
            {
                yield break;
            }
            else
            {
                // move current to complete
                yield return new(ChangeReason.Add, _keyFunc(create), new(Group.Ante, current));
                //replace current
                yield return new(ChangeReason.Remove, _keyFunc(create), new(Group.Current, current ?? throw new Exception("sdfsdsd ")));
            }

            foreach (var remain in post)
            {
                yield return new(ChangeReason.Remove, _keyFunc(remain), new(Group.Post, remain));

            }
            post.Clear();

            ante.Push(current = create);              
            yield return new(ChangeReason.Add, _keyFunc(create), new(Group.Current, create));     
    
        }

        /// <summary>
        /// removes all values from remaining and appends to complete stack
        /// </summary>
        /// <param name="value"></param>
        public IEnumerable<Change<GroupedItem<T, Group>, TKey>> AddToPosterior(T value)
        {

            //var create = _createFunc(value);
            var create = value;

            if (post.Any() == false)
            {          
            }
            // ignore values that match the start of the current set
            else if (ante.Any() && (ante.Peek()?.Equals(create) ?? false))
            {
                yield break;
            }
            else
            {
                //replace current
                yield return new(ChangeReason.Remove, _keyFunc(create), new(Group.Current, current ?? throw new Exception("sdfsdsd ")));
                // move current to complete
                yield return new(ChangeReason.Add, _keyFunc(create), new(Group.Post, current));
            }

            foreach (var complete in ante)
            {
                yield return new(ChangeReason.Remove, _keyFunc(complete), new(Group.Ante, complete));
            }

            ante.Clear();

            post.Push(current = create);
            yield return new(ChangeReason.Add, _keyFunc(create), new(Group.Current, create));
        }

        /// <summary>
        /// removes all values from remaining and appends to complete stack
        /// </summary>
        /// <param name="value"></param>
        public IEnumerable<Change<GroupedItem<T, Group>, TKey>> MoveBackward()
        {
            var value = ante.Pop();
            yield return new(ChangeReason.Add, _keyFunc(value), new(Group.Current, value));
            yield return new(ChangeReason.Remove, _keyFunc(current ?? throw new Exception("sdfsdsd ")), new(Group.Current, current));
            yield return new(ChangeReason.Remove, _keyFunc(value), new(Group.Ante, value));
            yield return new(ChangeReason.Add, _keyFunc(current), new(Group.Post, current));
            post.Push(current = value);
        }

        /// <summary>
        /// removes all values from remaining and appends to complete stack
        /// </summary>
        /// <param name="value"></param>
        public IEnumerable<Change<GroupedItem<T, Group>, TKey>> MoveForeward()
        {
            var value = post.Pop();
            yield return new(ChangeReason.Add, _keyFunc(value), new(Group.Current, value));
            yield return new(ChangeReason.Remove, _keyFunc(current ?? throw new Exception("sdfsdsd ")), new(Group.Current, current));
            yield return new(ChangeReason.Remove, _keyFunc(value), new(Group.Post, value));
            yield return new(ChangeReason.Add, _keyFunc(current), new(Group.Ante, current));
            ante.Push(current = value);
        }

        public bool CanMoveForward()
        {
            return post.Any();
        }

        public bool CanMoveBackward()
        {
            return ante.Any();
        }
    }

    public class PositionalModel
    {
        public enum Group
        {
            Post, Ante, Current
        }
    }
}
