using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Graph.Bayesian.WPF.Infrastructure;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public class GeneralVertex : Vertex
    {
        public GeneralVertex()
        {
            In.Subscribe(a =>
            {

            });

            var subject = new ReplaySubject<string>();

            Types.AllTypes().Subscribe(a =>
            {
                subject.OnNext(a);
            });

            In
               .JoinRight(subject)
               .Subscribe(a =>
               {

                   var (dd, ad) = a;

                //LastId = ad;
                //OnPropertyChanged(nameof(LastId));
                Out.OnNext(dd);

               });
        }
    }
}

