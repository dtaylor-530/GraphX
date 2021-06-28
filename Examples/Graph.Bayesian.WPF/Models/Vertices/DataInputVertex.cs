using System;
using System.Reactive;
using System.Windows.Input;
using Graph.Bayesian.WPF.Infrastructure;

namespace Graph.Bayesian.WPF.Models
{


    public class DataInputVertex : Vertex
    {
        private int input;


        public DataInputVertex()
        {
            ClickCommand = ReactiveUI.ReactiveCommand.Create<Unit, Unit>(a =>
            {
                this.OutMessages.OnNext(new DataMessage<DataVertex.IntData>(this.ID.ToString(), string.Empty, DateTime.Now, new(this.ID, Input)));

                return a;
            });

        }

        public int Input
        {
            get => input;
            set
            {
                input = value;
                this.OnPropertyChanged();
            }
        }


        public override ICommand ClickCommand { get; }

    }
}
