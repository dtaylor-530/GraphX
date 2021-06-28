using Graph.Bayesian.WPF.Infrastructure;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public enum ListEditType
    {
        Add,
        Subtract,
        Up,
        Down
    }

    public record ListEditMessage(string From, string To, ListEdit ListEdit) : Message(From, To, DateTime.Now, ListEdit);
    public record ListEdit(ListEditType ListEditType);
    public record ListEditMove(ListEditType ListEditType, int Count) : ListEdit(ListEditType);

    public class ListEditorVertex : Vertex
    {
        public ListEditorVertex()
        {
            var add = ReactiveCommand.Create(() => new ListEdit(ListEditType.Add));
            var subtract = ReactiveCommand.Create(() => new ListEdit(ListEditType.Subtract));
            var up = ReactiveCommand.Create(() => new ListEditMove(ListEditType.Up, 1));
            var down = ReactiveCommand.Create(() => new ListEditMove(ListEditType.Down, 1));

            add.Merge(subtract).Merge(up).Merge(down).Subscribe(a =>
            {
                OutMessages.OnNext(new ListEditMessage(this.ID.ToString(), string.Empty, a));
            });

            Add = add; Subtract = subtract; Up = up; Down = down;
        }

        public ICommand Add { get; }
        public ICommand Subtract { get; }
        public ICommand Up { get; }
        public ICommand Down { get; }

    }
}
