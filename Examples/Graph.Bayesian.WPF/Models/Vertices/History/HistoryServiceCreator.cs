using System;
using DynamicData;

namespace Graph.Bayesian.WPF.Models.Vertices.History
{
    public class HistoryServiceCreator<T, TKey> : HistoryService<T, TKey>
        where T : IEquatable<T>, IKey<TKey>, IComparable
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


}
