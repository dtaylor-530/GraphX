using Graph.Bayesian.WPF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Graph.Bayesian.WPF.Models.Vertices.History
{
    public class MovementVertex : Vertex
    {
        public MovementVertex()
        {
            var clickCommand = ReactiveUI.ReactiveCommand.Create<Unit, Unit>(unit =>
            {
                return Unit.Default;
            });
            clickCommand
                .CombineLatest(In.OfType<PropertyChangeMessage>()).Subscribe(a =>
                {
                    var (_, b) = a;
                    Out.OnNext(new MovementMessage(this.ID.ToString(), string.Empty, DateTime.Now, Movement.Forward));
                });
            Foreward = clickCommand;

            var click2Command = ReactiveUI.ReactiveCommand.Create<Unit, Unit>(unit =>
            {
                return Unit.Default;
            });
            click2Command
                .CombineLatest(In.OfType<PropertyChangeMessage>()).Subscribe(a =>
                {
                    var (_, b) = a;
                    Out.OnNext(new MovementMessage(this.ID.ToString(), string.Empty, DateTime.Now, Movement.Backward));
                });
            Backward = click2Command;
        }

        public ICommand Foreward { get; }

        public ICommand Backward { get; }
    }
}