using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Graph.Bayesian.WPF.Infrastructure;
using ReactiveUI;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public class DataVertex : Vertex
    {
        public record IntData(long Id, int Value);

        public DataVertex()
        {
            In
                .OfType<TimerMessage>()
                .DistinctUntilChanged(a => a.Rate)
                .Subscribe(a =>
                {

                });

            In
               .OfType<TimerMessage>()
               .DistinctUntilChanged(a => a.Rate)
               .JoinRight(Types.WhereTypeIs<DataVertex>())
                .Subscribe(a =>
                {
                    Out.OnNext(a.Item1 with { From = ID.ToString(), To = a.Item2, Sent = DateTime.Now });
                });

            //ClickCommand = ReactiveCommand.Create<Unit, Unit>(a =>
            //{
            //    this.InMessages.OnNext(new DataMessage<IntData>(this.ID.ToString(), ID.ToString(), default, DateTime.Now, default));
            //    this.InMessages.OnNext(new ClickMessage(this.ID.ToString(), this.ID.ToString()));
            //    return a;
            //});

            In
                .OfType<DataMessage<IntData>>()
                .Subscribe(a =>
                {
                    var (@from, to, dateTime, (Id, value)) = a;
                    Data = value;
                    LastDataChange = DateTime.Now;
                    OnPropertyChanged(nameof(Data));
                    OnPropertyChanged(nameof(LastDataChange));
                });

            In
               .OfType<DataMessage<IntData>>()
               //.Where(a => a.Data.LastId != this.ID.ToString())
               .JoinRight(Types.WhereTypeIs(typeof(DataVertex)))
               //.CombineLatest(InMessages.OfType<TimerMessage>().Select(a => a.Rate).DistinctUntilChanged().StartWith(0))
               .Select(a =>
               {
                   var ((from, to, dateTime, (Id, value)), id) = a;
                   if (id != from)
                       return a.Item1 with { From = ID.ToString(), To = id, Sent = DateTime.Now };
                   /*new DataMessage<IntData>(this.ID.ToString(), id, DateTime.Now, new(Id, Data));*/
                   return null;
               })
               .WhereNotNull()
               .DistinctUntilChanged()
               .Subscribe(Out.OnNext);


            In.OfType<IdMessage>()
               .Subscribe(message =>
               {

               });
        }

        public int Data { get; private set; }

        public DateTime LastDataChange { get; private set; }

    }
}
