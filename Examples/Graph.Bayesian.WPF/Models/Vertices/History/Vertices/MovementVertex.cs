using Graph.Bayesian.WPF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Graph.Bayesian.WPF.Models.Vertices.History
{
    public record MovementInput(bool CanMoveForeward, bool CanMoveBackward);

    public record MovementInputMessage(string From, string To, DateTime Sent, MovementInput MovementInput) : Message(From, To, Sent, MovementInput);

    public class MovementVertex : Vertex
    {
        public MovementVertex()
        {
            var clickCommand = ReactiveUI.ReactiveCommand.Create<Unit, Unit>(unit =>
            {
                return Unit.Default;
            }, In.OfType<MovementInputMessage>().Select(a => a.MovementInput.CanMoveForeward).StartWith(true));

            clickCommand
                .Subscribe(a =>
                {
                    Out.OnNext(new MovementMessage(this.ID.ToString(), string.Empty, DateTime.Now, Movement.Forward));
                });
            Foreward = clickCommand;

            var click2Command = ReactiveUI.ReactiveCommand.Create<Unit, Unit>(unit =>
            {
                return Unit.Default;
            }, In.OfType<MovementInputMessage>().Select(a => a.MovementInput.CanMoveForeward).StartWith(true));
            click2Command
                .Subscribe(a =>
                {
                    Out.OnNext(new MovementMessage(this.ID.ToString(), string.Empty, DateTime.Now, Movement.Backward));
                });
            Backward = click2Command;
        }

        public ICommand Foreward { get; }

        public ICommand Backward { get; }
    }
}