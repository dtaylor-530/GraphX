using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using DynamicData;
using Graph.Bayesian.WPF.Models;

namespace Graph.Bayesian.WPF.Infrastructure
{
    public static class Helper
    {


        /// <summary>
        /// Notifies when any property on the object has changed.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertiesToMonitor">specify properties to Monitor, or omit to monitor all property changes.</param>
        /// <returns>A observable which includes notifying on any property.</returns>
        public static IObservable<(TObject , string? name)> WhenAnyPropertyHasChangedExcept<TObject>(this TObject source, params string[] propertiesNotToMonitor)
            where TObject : INotifyPropertyChanged
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Observable
                .FromEventPattern<PropertyChangedEventHandler?, PropertyChangedEventArgs>(handler => source.PropertyChanged += handler, handler => source.PropertyChanged -= handler)
                .Where(x => propertiesNotToMonitor.Length == 0 || propertiesNotToMonitor.Contains(x.EventArgs.PropertyName) == false)
                .Select(x => (source, x.EventArgs.PropertyName));
        }

        /// <summary>
        /// Notifies when any property on the object has changed.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertiesToMonitor">specify properties to Monitor, or omit to monitor all property changes.</param>
        /// <returns>A observable which includes notifying on any property.</returns>
        public static IObservable<(TObject, string? name)> WhenAnyPropertyHasChanged<TObject>(this TObject source, params string[] propertiesToMonitor)
            where TObject : INotifyPropertyChanged
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Observable
                .FromEventPattern<PropertyChangedEventHandler?, PropertyChangedEventArgs>(handler => source.PropertyChanged += handler, handler => source.PropertyChanged -= handler)
                .Where(x => propertiesToMonitor.Length == 0 || propertiesToMonitor.Contains(x.EventArgs.PropertyName))
                .Select(x => (source, x.EventArgs.PropertyName));
        }


        public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
        {

            var properties = type
            .GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public)
            .Where(ø => ø.CanRead);

            return properties;
        }
        public static IEnumerable<PropertyInfo> GetAllProperties<T>()
        {

            return GetAllProperties(typeof(T));

        }

        public static IObservable<(T?, TR?)> MergeDifferent<T, TR>(this IObservable<T> ts, IObservable<TR> trs)
        {

            return Observable.Merge(
               ts.Select(a => ((T?)a, default(TR?))),
               trs.Select(c => (default(T?), (TR?)c)),
               ts.CombineLatest(trs).Select(a=>(a.First,a.Second))
               /*ts.CombineLatest(trs)*/);
        }


        //public static IObservable<(T?, TR?)> MergeDifferent<T, TR>(this IObservable<T> ts, IObservable<TR> trs)
        //{

        //    return Observable.Merge(
        //       ts.Select(a => ((T?)a, default(TR?))).StartWith((default(T?), default(TR?))),
        //       trs.Select(c => (default(T?), (TR?)c)).StartWith((default(T?), default(TR?))))
        //        .WhereNotNull();
        //}


        public static IObservable<DateTime> SelectTicks(this DispatcherTimer dispatcherTimer)
        {

            return Observable.FromEventPattern<EventHandler, EventArgs>(
               a => dispatcherTimer.Tick += a,
               a => dispatcherTimer.Tick -= a).Select(a => DateTime.Now);

        }

        public static IObservable<string> SelectAll(this IObservable<IChangeSet<TypeRecord, Type>> observable)
        {

            return observable.ToCollection()
               .SelectMany(a => a)
               .SelectMany(c => c.Values)
               .Distinct();
        }

        public static IObservable<string> SelectOfType(this IObservable<IChangeSet<TypeRecord, Type>> observable, Type key)
        {

            return observable.ToCollection()
               .Where(ac => ac.Any(a => a.Key == key))
               .SelectMany(a => a.Where(sa => sa.Key == key))
               .SelectMany(c => c.Values)
               .Distinct();
        }

        public static IObservable<string> SelectOfType<T>(this IObservable<IChangeSet<TypeRecord, Type>> observable)
        {
            return SelectOfType(observable, typeof(T));
        }

        /// <summary>
        /// Ensures late notifications from <see cref="observableLeft"/>
        /// get combined with all notifications in <see cref="observableRight"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <typeparam name="TS"></typeparam>
        /// <param name="observableLeft"></param>
        /// <param name="observableRight"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IObservable<TS> JoinRight<T, TR, TS>(this IObservable<T> observableLeft,
           IObservable<TR> observableRight, Func<T, TR, TS> selector)
        {
            return Observable.Create<TS>(observer =>
            {
                IDisposable? disposable = default;
                return observableLeft.Subscribe(a =>
                {
                    // reset disposable
                    disposable?.Dispose();
                    disposable = observableRight
                       .Subscribe(b => { observer.OnNext(selector(a, b)); });

                });
            });
        }

        /// <summary>
        /// Ensures late notifications from <see cref="observableLeft"/>
        /// get combined with all notifications in <see cref="observableRight"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <typeparam name="TS"></typeparam>
        /// <param name="observableLeft"></param>
        /// <param name="observableRight"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IObservable<(T, TR)> JoinRight<T, TR>(this IObservable<T> observableLeft, IObservable<TR> observableRight)
        {
            return observableLeft.JoinRight<T, TR, (T, TR)>(observableRight, (a, b) => (a, b));
        }
    }
}
