using AutoBogus;
using DynamicData;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.ViewModel;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Graph.Bayesian.WPF.Models.Vertices.Pagination
{

    public record SelectableKeyRecord(string Key, bool IsSelected) : SelectableRecord(IsSelected);

    public class EnumerableInputVertex : Vertex
    {
        private int input = 50;

        public EnumerableInputVertex()
        {
            var clickCommand = ReactiveCommand.Create<Unit, int>(a =>
            {
                return input;
            });
            Create(clickCommand)
                .Subscribe(a =>
                {
                    Out.OnNext(new ListInputMessage(ID.ToString(), string.Empty, DateTime.Now, new ChangeSetInput<SelectableKeyRecord, string>(a)));
                });

            ClickCommand = clickCommand;
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

        static IObservable<IChangeSet<SelectableKeyRecord, string>> Create(IObservable<int> count)
        {
            var personFaker = new AutoFaker<SelectableKeyRecord>()
                .RuleFor(fake => fake.Key, fake => fake.Random.Word())
                .RuleFor(fake => fake.IsSelected, fake => false);

            return ObservableChangeSet.Create<SelectableKeyRecord, string>(o =>
            {
                return count.Subscribe(c =>
                {
                    o.Edit(a =>
                    {
                        a.Clear();
                        foreach (var x in Enumerable.Range(0, c).Select(a => personFaker.Generate()))
                        {
                            a.AddOrUpdate(x);
                        }
                    });
                });
            }, a => a.Key);
        }

        public override ICommand ClickCommand { get; }

    }
}
