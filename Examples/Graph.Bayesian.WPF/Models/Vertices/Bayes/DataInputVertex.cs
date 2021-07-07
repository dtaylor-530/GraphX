using System;
using System.Reactive;
using System.Windows.Input;
using Graph.Bayesian.WPF.Infrastructure;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public class DataInputVertex : Vertex
    {
        private int input;

        public DataInputVertex()
        {
            ClickCommand = ReactiveUI.ReactiveCommand.Create<Unit, Unit>(a =>
            {
                Out.OnNext(new DataMessage<DataVertex.IntData>(ID.ToString(), string.Empty, DateTime.Now, new(ID, Input)));
                return a;
            });
        }

        public int Input
        {
            get => input;
            set
            {
                input = value;
                OnPropertyChanged();
            }
        }


        public override ICommand ClickCommand { get; }

    }
}
