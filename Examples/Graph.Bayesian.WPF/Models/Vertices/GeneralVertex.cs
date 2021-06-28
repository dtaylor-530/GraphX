using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Graph.Bayesian.WPF.Infrastructure;

namespace Graph.Bayesian.WPF.Models.Vertexs {


   public class GeneralVertex : Vertex {

      public GeneralVertex() {

         InMessages.Subscribe(a => {

         });

         var subject = new ReplaySubject<string>();

         TypesChangeSet.SelectAll().Subscribe(a => {
            subject.OnNext(a);
         });

         InMessages
            .JoinRight(subject)
            .Subscribe(a => {

               var (dd, ad) = a;

               //LastId = ad;
               //OnPropertyChanged(nameof(LastId));
               OutMessages.OnNext(dd);

            });
      }




   }
}

