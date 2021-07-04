using System;
using DynamicData;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;

namespace Graph.Bayesian.WPF.Infrastructure
{
    public interface ITypesDictionary { Dictionary<Type, List<string>> Types { get; } }

    public record TypeRecord(Type Key, IReadOnlyCollection<string> Values, bool IsSelected) : Record, ISelected;

    public static class TypeRecordHelper
    {
        public static IObservable<string> AllTypes(this IObservable<IChangeSet<TypeRecord, Type>> observable)
        {
            return observable
               .ToCollection()
               .SelectMany(a => a)
               .SelectMany(c => c.Values)
               .Distinct();
        }

        public static IObservable<string> WhereTypeIs(this IObservable<IChangeSet<TypeRecord, Type>> observable, Type key) => observable
               .ToCollection()
               .Where(ac => ac.Any(a => a.Key == key))
               .SelectMany(a => a.Where(sa => sa.Key == key))
               .SelectMany(c => c.Values)
               .Distinct();

        public static IObservable<string> WhereTypeIs<T>(this IObservable<IChangeSet<TypeRecord, Type>> observable)
        {
            return WhereTypeIs(observable, typeof(T));
        }
    }

    public class TypesService : ReactiveService<(Type type, string id), IChangeSet<TypeRecord, Type>>, ITypesDictionary
    {
        public TypesService()
        {
            var changeSet = ObservableChangeSet.Create<TypeRecord, Type>(sourceCache =>
            {
                Dictionary<Type, List<string>> TypesDictionary = new();
                return In.Select(a =>
                {
                    var (type, id) = a;
                    (TypesDictionary[type] = TypesDictionary.GetValueOrDefault(type, new List<string>())).Add(id);
                    return (record: new TypeRecord(type, TypesDictionary[type], false), TypesDictionary);
                }).Subscribe(a =>
                {
                    Types = a.TypesDictionary;
                    this.RaisePropertyChanged(nameof(Types));
                    sourceCache.AddOrUpdate(a.record);
                });
            }, a => a.Key);

            changeSet.Subscribe(Out.OnNext);

            //changeSet.Bind(out collection).Subscribe();
        }

        public Dictionary<Type, List<string>> Types { get; private set; }
    }
}
