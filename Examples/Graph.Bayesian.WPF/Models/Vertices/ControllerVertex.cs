using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Graph.Bayesian.WPF.Infrastructure;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    class ControllerVertex : Vertex
    {

        public ObservableConcurrentQueue<ReceivedMessage> QueuedMessages { get; } = new();
        public int IsNothingInQueue { get; private set; }

        public ControllerVertex()
        {
            In
               .OfType<ReceivedMessage>()
               .Subscribe(a =>
               {
                   QueuedMessages.Enqueue(a);
               });

            In
               .OfType<TimerMessage>()
               .Subscribe(a =>
               {

                   if (QueuedMessages.TryDequeue(out var message))
                   {
                       Out.OnNext(new ProceedMessage(ID.ToString(), message.From));
                   }
                   else
                   {
                       IsNothingInQueue++;
                       OnPropertyChanged(nameof(IsNothingInQueue));
                   }
               });
        }

    }
}
