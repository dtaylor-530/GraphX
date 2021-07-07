using DynamicData;
using Graph.Bayesian.WPF.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;

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
 
    }

    public record PropertyChange(Guid Key, string ParentId, string Name, DateTime UpDate, object Value) : Change(UpDate, Value), IKey<Guid>, IComparable, IComparable<PropertyChange>
    {
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

    public record DynamicChange(int CurrentIndex, object Current, ChangeReason Reason, object Key, object G);

    public record Change(DateTime UpDate, object Value);

    public record Input<T, TKey>(TKey Key, T Value) : Input(Key?.ToString(), Value), IInput<T, TKey>;

    public record Input(string? StringKey, object? ObjectValue);

    public interface IInput<T, TKey> : IInput, IKey<TKey> { T Value { get; } }

    public interface IInput { }

    public record OutState<T, TKey>(ChangeSet<GroupedItem<System.Reactive.Timestamped<T>, HistoryModel.Group, TKey>, DateTime> ChangeSet);

    public record Result<TOut, TFailure>(DateTime DateTime, bool Success, TOut? Value, TFailure? Failure) : Result(DateTime, Success, Value, Failure) where TFailure : Failure;

    public record ManyResult<TOut, TFailure>(DateTime DateTime, bool Success, IReadOnlyCollection<TOut>? Value, TFailure? Failure) : ManyResult(DateTime, Success, Value, Failure) where TFailure : Failure;

    public record Result(DateTime DateTime, bool Success, object? ObjectValue, object? ObjectFailure);

    public record ManyResult(DateTime DateTime, bool Success, IEnumerable? EnumerableValue, object? ObjectFailure) : Result(DateTime, Success, EnumerableValue, ObjectFailure);

    public record OperationResult<T, TKey>(
        DateTime DateTime,
        bool Success,
        ChangeSet<GroupedItem<T, HistoryModel.Group, TKey>, TKey>? ChangeSet,
        IReadOnlyCollection<DynamicChange> Array,
        Failure<OperationFailureReason>? Failure,
        T? Current) :
        ManyResult<DynamicChange, Failure<OperationFailureReason>>(DateTime, Success, Array, Failure)
    where TKey : notnull;

    public enum OperationFailureReason
    {
        None, MovedToFarForward, MovedTooFarBack, WeirdConfiguration
    }

    public record Failure<T, TReason>(T InValue, TReason Reason) : Failure(InValue, Reason) where T : IEnumerable<IInput?>;

    public record Failure<TReason>(IEnumerable<IInput> InValue, TReason Reason) : Failure<IEnumerable<IInput>, TReason>(InValue, Reason);

    public record Failure(IEnumerable<IInput> Value, object FailureReason);

    public record Output<TModel, TChanges>(TModel Model, TChanges Changes, Failure? Failure);

    public record GroupedItem<T, TGroupKey, TKey>(TGroupKey GroupKey, TKey Key, T Value);

    public record GroupedItem<T, TKey>(HistoryModel.Group GroupKey, TKey Key, T Value) : GroupedItem<T, HistoryModel.Group, TKey>(GroupKey, Key, Value);

    public record GroupViewItem(string GroupKey, string Key, bool IsSelected, IComparable Value) : Record, ISelected;
}
