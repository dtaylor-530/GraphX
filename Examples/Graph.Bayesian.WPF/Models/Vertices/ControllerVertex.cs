using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Graph.Bayesian.WPF.Infrastructure;

namespace Graph.Bayesian.WPF.Models.Vertexs 
{
   class ControllerVertex : Vertex {

      public ObservableConcurrentQueue<ReceivedMessage> QueuedMessages { get; }= new();
      public int IsNothingInQueue { get; private set; } 

      public ControllerVertex()
      {
         InMessages
            .OfType<ReceivedMessage>()
            .Subscribe(a =>
            {
               QueuedMessages.Enqueue(a);
            });

         InMessages
            .OfType<TimerMessage>()
            .Subscribe(a => {

               if (QueuedMessages.TryDequeue(out var message)) {
                  OutMessages.OnNext(new ProceedMessage(this.ID.ToString(), message.From));
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
