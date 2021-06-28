using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Graph.Bayesian.WPF.Infrastructure;

namespace Graph.Bayesian.WPF.Models
{
    public abstract class Filter : ISubject<Message>, INotifyPropertyChanged
    {
        protected Filter()
        {
        }

        public virtual void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public virtual void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public abstract void OnNext(Message value);

        public int SubscribersCount { get; protected set; }

        public abstract IDisposable Subscribe(IObserver<Message> observer);

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

    }

    class IdFilter : Filter
    {
        private readonly ReplaySubject<Message> messages = new(1);

        public IdFilter(string id)
        {
            Id = id;
        }

        public string Id { get; }

        public ObservableCollection<Message> BadMessages { get; } = new();

        public override void OnNext(Message value)
        {
            if (value.To == Id)
                messages.OnNext(value);
            else
                BadMessages.Add(value);
        }

        public override IDisposable Subscribe(IObserver<Message> observer)
        {
            ++SubscribersCount;
            if (SubscribersCount > 1)
                throw new Exception("Maximum # of subscribers exceeded");
            OnPropertyChanged(nameof(SubscribersCount));
            return messages.Subscribe(observer);
        }
    }

    class NoFilter : Filter
    {

        private readonly ReplaySubject<Message> messages = new(1);

        public NoFilter()
        {
        }

        public override void OnNext(Message value)
        {
            messages.OnNext(value);
        }

        public override IDisposable Subscribe(IObserver<Message> observer)
        {
            ++SubscribersCount;
            if (SubscribersCount > 1)
                throw new Exception("Maximum # of subscribers exceeded");
            return messages.Subscribe(a =>
               observer.OnNext(a));
        }
    }

    /// <summary>
    /// Blocks all messages passing through
    /// </summary>
    class AbsoluteFilter : Filter
    {
        public AbsoluteFilter()
        {
        }

        public override void OnNext(Message value)
        {
        }

        public override IDisposable Subscribe(IObserver<Message> observer)
        {
            ++SubscribersCount;
            if (SubscribersCount > 1)
                throw new Exception("Maximum # of subscribers exceeded");
            return Disposable.Empty;
        }
    }
}
