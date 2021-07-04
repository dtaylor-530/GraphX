using System;
using System.Reactive.Linq;
using Graph.Bayesian.WPF.Infrastructure;
using ReactiveUI;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public class TimerVertex : Vertex
    {
        private int rate = 5000;
        //private Queue<int> queue = new();
        //private readonly DispatcherTimer timer = new();
        //private TimeSpan timeSpan;

        public TimerVertex()
        {
            this.WhenAnyValue(a => a.Rate)
                .JoinRight(Types)
                .Subscribe(rate =>
            {
                Out.OnNext(new TimerMessage(ID.ToString(), string.Empty, DateTime.Now, rate.Item1));
            });


            //timer.SelectTicks()
            //   .JoinRight(TypesChangeSet.SelectOfType<ControllerVertex>())
            //   .Subscribe(a => {
            //      var (first, second) = a;
            //      DateTime = first;
            //      OnPropertyChanged(nameof(DateTime));
            //      OutMessages.OnNext(new TimerMessage(this.ID.ToString(), second, DateTime, rate));
            //   });

            //var timer = new DispatcherTimer();
            //var subject = new Subject<DateTime>();

            //Observable.Merge(timer.SelectTicks(), subject)
            //    .Subscribe(a =>
            //    {
            //        TimeSpan = a - DateTime;
            //        DateTime = a;
            //        OnPropertyChanged(nameof(DateTime));

            //    });


            //var ticks = this.WhenAnyValue(a => a.Rate)
            //   .WithLatestFrom(timer.SelectTicks().Select(a => DateTime.Now).StartWith(DateTime.Now))
            //  .Subscribe(a => {

            //     if (TimeSpan.FromMilliseconds(a.First) < DateTime.Now - a.Second) {
            //        subject.OnNext(DateTime.Now);
            //     }
            //     timer.Stop();
            //     timer.Interval = TimeSpan.FromMilliseconds(a.First);
            //     timer.Start();

            //  });
        }

        public int Rate
        {
            get => rate;
            set { rate = value; OnPropertyChanged(); }
        }

        //public TimeSpan TimeSpan {
        //   get => timeSpan;
        //   set { timeSpan = value; OnPropertyChanged(); }
        //}

        //public DateTime DateTime { get; private set; }

    }
}
