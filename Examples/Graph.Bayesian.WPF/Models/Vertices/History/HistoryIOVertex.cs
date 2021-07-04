using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using AutoBogus;
using Fasterflect;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.Models.Vertices.History;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public class HistoryIOVertex : Vertex
    {
        private Person? output;
        private (DateTime, string, object, object)? lastChange;

        public HistoryIOVertex()
        {
            Random random = new();
            var properties = typeof(Person).GetProperties().ToArray();
            output = AutoFaker.Generate<Person>();
            Output = output.DeepClone();

            var clickCommand = ReactiveUI.ReactiveCommand.Create<Unit, Unit>(unit =>
            {
                Output = output.DeepClone();
                OnPropertyChanged(nameof(Output));
                propertyChanges.OnNext(nameof(Output));
                var name = properties[random.Next(0, properties.Length)].Name;
                var oldValue = output.GetPropertyValue(name);
                var value = AutoFaker.Generate<Person>().GetPropertyValue(name);
                LastChange = new(DateTime.Now, name, oldValue, value);
                output.SetPropertyValue(name, value);
                return Unit.Default;
            });



            In
                .OfType<HistoryMessage<PropertyChange>>()
                .Select(a => a.Data)
                .Select(a => a.Value)
                .Subscribe(a =>
                {
                    if (a.Equals(Output) == false)
                    {
                        Output = a.DeepClone();
                        OnPropertyChanged(nameof(Output));
                    }                   
                });


            ClickCommand = clickCommand;
        }

        [MonitorPropertyChange(false)]
        public (DateTime, string, object, object)? LastChange
        {
            get => lastChange;
            private set
            {
                lastChange = value;
                OnPropertyChanged();
            }
        }

        [MonitorPropertyChange(false)]
        public object? Output { get; private set; }


        public override ICommand ClickCommand { get; }
    }


    public class Person : IEquatable<Person>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool Equals(Person? other)
        {
            return this.FirstName == other?.FirstName && this.LastName == other.LastName;
        }

        public override bool Equals(object? obj)
        {
            return this.Equals((Person?)obj);
        }

        public override int GetHashCode()
        {
            return this.FirstName.GetHashCode() * this.LastName.GetHashCode();
        }

        public override string? ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}
