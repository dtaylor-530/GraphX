using Graph.Bayesian.WPF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public class SaveVertex : Vertex
    {       
        public SaveVertex()
        {
            var clickCommand = ReactiveUI.ReactiveCommand.Create<Unit, Unit>(unit =>
            {
                return Unit.Default;
            });
            clickCommand
                .CombineLatest(In.OfType<PropertyChangeMessage>()).Subscribe(a =>
            {
                var (_, b) = a;
                //OutMessages.OnNext(new SaveLoadViewModelRequestMessage());
            });
            ClickCommand = clickCommand;
        }

        public override ICommand ClickCommand { get; }
    }
}