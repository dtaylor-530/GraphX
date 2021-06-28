using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Bayesian.WPF.Infrastructure {
   public class ReplayModel<T> : ISubject<T>
   {
      private readonly ObservableCollection<T> collection = new ();
      private readonly Subject<T> subject = new();

      public ReplayModel() {
      }

      public ObservableCollection<T> Collection => collection;

      public void OnCompleted()
      {
         throw new NotImplementedException();
      }

      public void OnError(Exception error)
      {
         throw new NotImplementedException();
      }

      public void OnNext(T value) {

         subject.OnNext(value);
         collection.Add(value);
      }

      public IDisposable Subscribe(IObserver<T> observer)
      {
         foreach (var value in collection)
         {
            observer.OnNext(value);
         }

         return subject.Subscribe(observer);
      }
   }
}
