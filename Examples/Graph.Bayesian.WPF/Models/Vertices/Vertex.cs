using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DynamicData;
using Fasterflect;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.Models.Vertices.History;
using Graph.Bayesian.WPF.ViewModel;
using GraphX.Common.Models;
using PropertyTools.DataAnnotations;
using ReactiveUI;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    /* DataVertex is the data class for the vertices. It contains all custom vertex data specified by the user.
     * This class also must be derived from VertexBase that provides properties and methods mandatory for
     * correct GraphX operations.
     * Some of the useful VertexBase members are:
     *  - ID property that stores unique positive identfication number. Property must be filled by user.
     */
    public class Vertex : VertexBase, ISubject<Message>, System.ComponentModel.INotifyPropertyChanged
    {
        private readonly Lazy<string> type;
        private readonly TypesService typesService;
        private readonly Lazy<string[]> ignoreProperties;
        protected Subject<string> propertyChanges = new();

        protected IObservable<IChangeSet<TypeRecord, Type>> Types => typesService.AsObservable();

        public Vertex()
        {
            ignoreProperties = new(() =>
            {
                var arr = this.GetType()
                                .FilterPropertiesByAttribute<MonitorPropertyChangeAttribute>(a => a?.RegisterChanges == false)
                                .Select(a => a.Name)
                                .ToArray();

                return arr;
            });
            typesService = new();
            TypesViewModel = new();
            ID = IDFactory.Get;
            type = new(() => GetType().Name);
            typesService.RemoveKey().Subscribe(TypesViewModel.OnNext);

            ClickCommand = ReactiveCommand.Create<Unit, Unit>(a =>
            {
                //LastClicked = DateTime.Now;
                //OnPropertyChanged(nameof(LastClicked));
                In.OnNext(new ClickMessage(ID.ToString(), ID.ToString()));
                return a;
            });


            var a = In
               .OfType<ClickMessage>()
               .JoinRight(Types.WhereTypeIs(typeof(ClickServiceVertex)))
               .Select(a => (Message)new ClickMessage(ID.ToString(), a.Item2));


            var b = In
                .OfType<IsSelectedMessage>()
                .Where(a => a.Value)
                .JoinRight(Types.WhereTypeIs(typeof(SelectServiceVertex)))
                .Select(a => (Message)new VerticeselectedMessage(ID.ToString(), a.Item2, this));

            a.Merge(b)
                .Subscribe(msg =>
                {
                    Out.OnNext(msg);
                });

            var values = ignoreProperties.Value;
            In
               .Subscribe(msg => NewMessage(msg));

            _ = this
                //.WhenAnyPropertyHasChangedExcept(new[] { nameof(UpdatedCount), nameof(LastUpdated), nameof(LastMessage), nameof(LastMessageUpdate) })
                .WhenAnyPropertyHasChangedExcept(ignoreProperties.Value)
                .Select(a => a.name)
                .Merge(propertyChanges)
                .Scan((0, (string?)""), (a, b) => (++a.Item1, b))
                //.Skip(1)
                .ObserveOnDispatcher()
                .Subscribe(c =>
                {
                    PropertyHasChanged(c);
                });
        }

        private void PropertyHasChanged((int, string) c)
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
                var pc = new PropertyChange(Guid.NewGuid(), ID.ToString(), name, DateTime.Now, this.GetPropertyValue(name));
                var msg = new PropertyChangeMessage(ID.ToString(), default, DateTime.Now, pc);
                Out.OnNext(msg);
            }
            catch (Exception ex)
            {

            }
        }

        private void NewMessage(Message msg)
        {
            Update(msg);

            switch (msg)
            {
                case IdMessage { From: { } from, Type: { } type }:
                    {
                        typesService.OnNext((type, from));
                        Update(msg);
                        break;
                    }
                case IsSelectedMessage { Value: { } value }:
                    {
                        IsSelected = value;
                        OnPropertyChanged(nameof(IsSelected));
                        Update(msg);
                        break;
                    }
                default:
                    Update(msg);
                    break;
            }

            void Update(Message msg)
            {
                LastMessage = msg;
                LastMessageUpdate = DateTime.Now;
                OnPropertyChanged(nameof(LastMessage));
                OnPropertyChanged(nameof(LastMessageUpdate));
            }
        }

        public string[] IgnoreProperties => ignoreProperties.Value;

        public ReplayModel<Message> In { get; } = new();

        public ReplayModel<Message> Out { get; } = new();

        public ListViewModel<TypeRecord> TypesViewModel { get; }

        public ITypesDictionary TypesDictionary => typesService;

        [Browsable(true)]
        [HeaderPlacement(HeaderPlacement.Collapsed)]
        public string TypeName => type.Value;

        public bool IsSelected { get; private set; }

        public bool IsEnabled { get; private set; }

        //public string LastId { get; protected set; }
        [MonitorPropertyChange(false)]
        public int UpdatedCount { get; private set; }

        [Browsable(true)]
        [HeaderPlacement(HeaderPlacement.Collapsed)]
        [MonitorPropertyChange(false)]
        public bool LastUpdated { get; private set; }

        [Browsable(true)]
        [HeaderPlacement(HeaderPlacement.Collapsed)]
        [MonitorPropertyChange(false)]
        public Message? LastMessage { get; private set; }

        [Browsable(true)]
        [HeaderPlacement(HeaderPlacement.Collapsed)]
        [MonitorPropertyChange(false)]
        public DateTime LastMessageUpdate { get; private set; }


        public override string ToString() => type.Value;

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
            In.OnNext(message);
        }

        public IDisposable Subscribe(IObserver<Message> observer)
        {
            return Out.Subscribe(a =>
            {
                observer.OnNext(a);
            });
        }

        public virtual ICommand ClickCommand { get; }

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }



}
