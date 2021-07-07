using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows.Input;
using DynamicData;
using Fasterflect;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.Models.Vertices.History;
using Graph.Bayesian.WPF.ViewModel;
using GraphX.Common.Models;
using PropertyTools.DataAnnotations;
using ReactiveUI;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Formatters;
using ReactiveUI.Validation.Formatters.Abstractions;
using ReactiveUI.Validation.Helpers;
using Splat;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    /* DataVertex is the data class for the vertices. It contains all custom vertex data specified by the user.
     * This class also must be derived from VertexBase that provides properties and methods mandatory for
     * correct GraphX operations.
     * Some of the useful VertexBase members are:
     *  - ID property that stores unique positive identfication number. Property must be filled by user.
     */
    public class Vertex : VertexBase, ISubject<Message>, IReactiveValidationObject
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
            typesService.RemoveKey().Select(a=>new ChangeSetInput<TypeRecord>(a)).Subscribe(TypesViewModel.OnNext);

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


            Formatter = 
                         Locator.Current.GetService<IValidationTextFormatter<string>>() ??
                         SingleLineFormatter.Default;


            ValidationContext = new ValidationContext();

            this.ListenToValidationStatusChanges();
            //    ValidationContext.Validations
            //        .ToObservableChangeSet()
            //        .ToCollection()
            //        .Select(components => components
            //            .Select(component => component
            //                .ValidationStatusChange
            //                .Select(_ => component))
            //            .Merge()
            //            .StartWith(ValidationContext))
            //        .Switch()
            //        .Subscribe(OnValidationStatusChange);
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

        [System.ComponentModel.Browsable(true)]
        [HeaderPlacement(HeaderPlacement.Collapsed)]
        public string TypeName => type.Value;

        public bool IsSelected { get; private set; }

        public bool IsEnabled { get; private set; } = true;

        //public string LastId { get; protected set; }
        [MonitorPropertyChange(false)]
        public int UpdatedCount { get; private set; }

        [System.ComponentModel.Browsable(true)]
        [HeaderPlacement(HeaderPlacement.Collapsed)]
        [MonitorPropertyChange(false)]
        public bool LastUpdated { get; private set; }

        [System.ComponentModel.Browsable(true)]
        [HeaderPlacement(HeaderPlacement.Collapsed)]
        [MonitorPropertyChange(false)]
        public Message? LastMessage { get; private set; }

        [System.ComponentModel.Browsable(true)]
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

        #region Validation

        public event PropertyChangingEventHandler? PropertyChanging;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public ValidationContext ValidationContext { get; } = new ValidationContext();

        public bool HasErrors => !this.ValidationContext.GetIsValid();

        public IValidationTextFormatter<string> Formatter { get; }

        public HashSet<string> MentionedPropertyNames { get; } = new HashSet<string>();

        public void RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            this.PropertyChanging?.Invoke(this, args);
        }

        public void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            this.OnPropertyChanged(args.PropertyName);
        }

        public void RaiseErrorsChanged(string propertyName = "")
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string? propertyName) => ValidationObjectHelper.GetErrors(this, propertyName);

        #endregion Validation


    }



}
