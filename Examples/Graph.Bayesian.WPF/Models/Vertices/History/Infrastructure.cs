using DynamicData;
using Graph.Bayesian.WPF.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Bayesian.WPF.Models.Vertices.History
{

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


    public record PropertyChange(Guid Key, string ParentId, string Name, DateTime UpDate, object Value) : Change(UpDate, Value), IKey<Guid>, IComparable, IComparable<PropertyChange>
    {

        //public DateTime Key => UpDate;

        public int CompareTo(object? obj)
        {
            if (obj is PropertyChange other)
                return this.UpDate.CompareTo(other.UpDate);
            return 0;
        }

        public int CompareTo(PropertyChange? other)
        {
            return this.UpDate.CompareTo(other.UpDate);
        }
    }

    public record Change(DateTime UpDate, object Value);

    public record Input<T, TKey>(TKey Key,T Value) : IInput<T, TKey>;

    public interface IInput<T, TKey> : IInput, IKey<TKey> { T Value { get; } }

    public interface IInput { }


    public record OutState<T, TKey>(ChangeSet<GroupedItem<System.Reactive.Timestamped<T>, PositionalModel.Group, TKey>, DateTime> ChangeSet);

    public record Result<TOut, TFailure>(DateTime DateTime, bool Success, TOut? Value, TFailure? Failure) where TFailure : Failure;

    public record OperationResult<T, TKey>(
        DateTime DateTime,
        bool Success,
        ChangeSet<GroupedItem<T, PositionalModel.Group, TKey>, TKey>? Value,
        Failure<OperationFailureReason>? Failure) :
        Result<ChangeSet<GroupedItem<T, PositionalModel.Group, TKey>, TKey>, Failure<OperationFailureReason>>(DateTime, Success, Value, Failure)
    where TKey : notnull;

    public enum OperationFailureReason
    {
        None, MovedToFarForward, MovedTooFarBack, WeirdConfiguration
    }


    public record Failure<T, TReason>(T InValue, TReason Reason) : Failure(InValue) where T : IEnumerable<IInput?>;

    public record Failure<TReason>(IEnumerable<IInput> InValue, TReason Reason) : Failure<IEnumerable<IInput>, TReason>(InValue, Reason);

    public record Failure(IEnumerable<IInput> Value);

    public record Output<TModel, TChanges>(TModel Model, TChanges Changes, Failure? Failure);

    public record GroupedItem(string Key, ICollection Collection);

    public record GroupedItem<T, TGroupKey, TKey>(TGroupKey GroupKey, TKey Key, T Value);

    public record GroupViewItem(string GroupKey, string Key, bool IsSelected, IComparable Value) : Record, ISelected;


}
