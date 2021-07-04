using System;
using Graph.Bayesian.WPF.Vertices;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Graph.Bayesian.WPF.Infrastructure
{
    public class ReactiveService<TIn, TOut> : ReactiveObject, ISubject<TIn, TOut>, IViewModel
    {
        readonly ReplaySubject<TIn> replaySubjectIn = new(1);
        readonly ReplaySubject<TOut> replaySubjectOut = new(1);

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(TIn value)
        {
            replaySubjectIn.OnNext(value);
        }

        public IObservable<TIn> In => replaySubjectIn.AsObservable();

        public IObserver<TOut> Out => replaySubjectOut.AsObserver();

        public IDisposable Subscribe(IObserver<TOut> observer)
        {
            return replaySubjectOut.Subscribe(observer);
        }

        public string Save()
        {
            throw new NotImplementedException();
        }
    }

    public class Service<TIn, TOut> : ISubject<TIn, TOut>
    {
        readonly ReplaySubject<TIn> replaySubjectIn = new();
        readonly ReplaySubject<TOut> replaySubjectOut = new();

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(TIn value)
        {
            replaySubjectIn.OnNext(value);
        }

        protected IObservable<TIn> In => replaySubjectIn.AsObservable();

        protected IObserver<TOut> Out => replaySubjectOut.AsObserver();

        public IDisposable Subscribe(IObserver<TOut> observer)
        {
            return replaySubjectOut.Subscribe(observer);
        }
    }
}
