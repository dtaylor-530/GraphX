using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using Fasterflect;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.Models.Vertexs;
using GraphX.Common.Models;
using ReactiveUI;

namespace Graph.Bayesian.WPF.Models
{

    public record TypeRecord(Type Key, IReadOnlyCollection<string> Values);


    /* DataVertex is the data class for the vertices. It contains all custom vertex data specified by the user.
     * This class also must be derived from VertexBase that provides properties and methods mandatory for
     * correct GraphX operations.
     * Some of the useful VertexBase members are:
     *  - ID property that stores unique positive identfication number. Property must be filled by user.
     */
    public class Vertex : VertexBase, IObservable<Message>, IObserver<Message>, INotifyPropertyChanged
    {
        private readonly ReplaySubject<TypeRecord> subject = new();
        private string type;
        private readonly ReadOnlyObservableCollection<TypeRecord> collection;

        protected static Random random { get; } = new();
        protected IObservable<IChangeSet<TypeRecord, Type>> TypesChangeSet { get; }
        protected Dictionary<Type, List<string>> TypesDictionary { get; } = new();

        public Vertex()
        {
            ID = random.Next(0, int.MaxValue);

            TypesChangeSet = ObservableChangeSet.Create<TypeRecord, Type>(sourceCache =>
            {
                return subject.Subscribe(a =>
                {
                    sourceCache.AddOrUpdate(a);
                });
            }, a => a.Key);

            TypesChangeSet.Bind(out collection).Subscribe();

            ClickCommand = ReactiveCommand.Create<Unit, Unit>(a =>
            {
                LastClicked = DateTime.Now;
                OnPropertyChanged(nameof(LastClicked));
                this.InMessages.OnNext(new ClickMessage(this.ID.ToString(), this.ID.ToString()));
                return a;
            });

            InMessages
                .Subscribe(msg =>
                {
                    var (from, to, a, b) = msg;
                    LastMessage = msg;
                    LastMessageUpdate = DateTime.Now;
                    OnPropertyChanged(nameof(LastMessage));
                    OnPropertyChanged(nameof(LastMessageUpdate));
                });

            InMessages
               .OfType<ClickMessage>()
               .JoinRight(TypesChangeSet.SelectOfType(typeof(ClickServiceVertex)))
               .Subscribe(msg =>
               {
                   OutMessages.OnNext(new ClickMessage(this.ID.ToString(), msg.Item2));
               });

            InMessages
               .OfType<IsSelectedMessage>()
               .Where(a => a.Value)
               .JoinRight(TypesChangeSet.SelectOfType(typeof(SelectServiceVertex)))
               .Subscribe(msg =>
               {
                   OutMessages.OnNext(new VertexSelectedMessage(this.ID.ToString(), msg.Item2, this));
               });

            InMessages
               .OfType<IsSelectedMessage>()
               .Subscribe(msg =>
               {
                   var (_, _, isSelected) = msg;
                   IsSelected = isSelected;
                   OnPropertyChanged(nameof(IsSelected));
               });

            InMessages.OfType<IdMessage>()
               .Subscribe(message =>
               {

                   if (Type == nameof(DataVertex))
                   {
                   }

                  (TypesDictionary[message.Type] = TypesDictionary.GetValueOrDefault(message.Type, new List<string>())).Add(message.From);
                   subject.OnNext(new TypeRecord(message.Type, TypesDictionary[message.Type]));

               });

            _ = this
                .WhenAnyPropertyHasChangedExcept(new[] { nameof(UpdatedCount), nameof(LastUpdated), nameof(LastMessage), nameof(LastMessageUpdate) })
                .Scan((0, (string?)""), (a, b) => (++a.Item1, b.name))
                .Skip(1)
                .ObserveOnDispatcher()
                .Subscribe(c =>
                {
                    var (a, name) = c;
                    UpdatedCount = a;
                    OnPropertyChanged(nameof(UpdatedCount));
                    // Double change (i.e true-false) necessary for DataTrigger to work
                    LastUpdated = true;
                    OnPropertyChanged(nameof(LastUpdated));
                    LastUpdated = false;
                    OnPropertyChanged(nameof(LastUpdated));
                    try
                    {
                        var pc = new PropertyChange(DateTime.Now, this.ID.ToString(), name, DateTime.Now, this.GetPropertyValue(name));
                        var msg = new PropertyChangeMessage(this.ID.ToString(), default, DateTime.Now, pc);
                        OutMessages.OnNext(msg);
                    }
                    catch (Exception ex)
                    {

                    }
                });


            //this.WhenAnyPropertyHasChanged()
            //    .Where(a => a.name != null)
            //    .Select(a => new PropertyChangeMessage(this.ID.ToString(), default, DateTime.Now, new PropertyChange(DateTime.Now, this.ID.ToString(), a.name!, DateTime.Now, a.Item1.GetPropertyValue(a.name))))
            //    .Subscribe(a =>
            //    {
            //        // OutMessages.OnNext(a);
            //    });
        }

        public string Type => type ??= this.GetType().Name;

        public ReplayModel<Message> InMessages { get; } = new();

        public ReplayModel<Message> OutMessages { get; } = new();

        public ReadOnlyObservableCollection<TypeRecord> Collection => collection;

        public bool IsSelected { get; private set; }

        //public string LastId { get; protected set; }

        public int UpdatedCount { get; private set; }

        public bool LastUpdated { get; private set; }

        public DateTime LastClicked { get; private set; }

        public Message LastMessage { get; private set; }

        public DateTime LastMessageUpdate { get; private set; }

        public IDisposable Subscribe(IObserver<Message> observer)
        {
            return OutMessages.Subscribe(a =>
            {
                observer.OnNext(a);
            });
        }

        public override string ToString() => type;

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Message message)
        {
            InMessages.OnNext(message);
        }

        public virtual ICommand ClickCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
