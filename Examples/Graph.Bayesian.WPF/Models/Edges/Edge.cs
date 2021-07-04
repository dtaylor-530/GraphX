using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.Models.Vertices;
using GraphX.Common.Models;
using ReactiveUI;

namespace Graph.Bayesian.WPF.Models
{

    public interface IGraphObject
    {
        
    }

    public enum State
    {
        NotActive = 0,
        SourceWasActive = 1,
        TargetWasActive = 2,
        SourceActive = 4,
        TargetActive = 8,
    }


    /* DataEdge is the data class for the edges. It contains all custom edge data specified by the user.
     * This class also must be derived from EdgeBase class that provides properties and methods mandatory for
     * correct GraphX operations.
     * Some of the useful EdgeBase members are:
     *  - ID property that stores unique positive identfication number. Property must be filled by user.
     *  - IsSelfLoop boolean property that indicates if this edge is self looped (eg have identical Target and Source vertices) 
     *  - RoutingPoints collection of points used to create edge routing path. If Null then straight line will be used to draw edge.
     *      In most cases it is handled automatically by GraphX.
     *  - Source property that holds edge source vertex.
     *  - Target property that holds edge target vertex.
     *  - Weight property that holds optional edge weight value that can be used in some layout algorithms.
     */

    public class Edge : EdgeBase<Vertex>, IObserver<Message>, IReactiveObject
    {
        //public enum Direction
        //{
        //    SourceToTarget, TargetToSource
        //}

        private readonly ReplaySubject<Message> inMessages = new();
        private State isActive = State.NotActive;
        private TimeSpan durationToTarget = TimeSpan.FromMilliseconds(rateInMilliSeconds), durationToSource = TimeSpan.FromMilliseconds(rateInMilliSeconds);
        private const int rateInMilliSeconds = 300;
        private readonly ObservableAsPropertyHelper<TimeSpan> duration;

        public State Active
        {
            get => isActive;
            private set
            {
                isActive = value;
                states.OnNext(value);
                OnPropertyChanged();
            }
        }

        Subject<State> states = new();

        //public record DirectionMessage(Direction Direction, Message Message);
        public Edge(Vertex source, Vertex target, double weight = 1, Filter? filterSource = null, Filter? filterTarget = null, bool isRateSensitiveToTarget = false, bool isRateSensitiveToSource = false)
         : base(source, target, weight)
        {

            ID = IDFactory.Get;

            duration = states
                .Select(a => a switch
                {
                    State.TargetActive => durationToTarget,
                    State.SourceActive => durationToSource,
                    _ => duration?.Value ?? TimeSpan.FromMilliseconds(rateInMilliSeconds),
                }).ToProperty(this, a => a.Duration);


            Angle = 90;

            SourceFilter = filterSource ?? new IdFilter(source.ID.ToString());
            TargetFilter = filterTarget ?? new IdFilter(target.ID.ToString());

            source.OnNext(new IdMessage(target.ID.ToString(), source.ID.ToString(), target.GetType()));
            target.OnNext(new IdMessage(source.ID.ToString(), target.ID.ToString(), source.GetType()));

            source.Subscribe(TargetFilter);
            target.Subscribe(SourceFilter);

            TargetFilter
                   .DistinctUntilChanged()
               .Subscribe(message =>
               {
                   DurationToTarget = (message is IRate { Rate: { } irate }) ? TimeSpan.FromMilliseconds(irate) : durationToTarget;
                   Active = State.TargetActive;

                   if (DurationToTarget > default(TimeSpan) &&   isRateSensitiveToTarget)
                   {
                       Observable.Return((WasActive: State.TargetWasActive, message))
                       .Delay(DurationToTarget)
                       .ObserveOnDispatcher()
                       .Subscribe(a =>
                       {
                           Active = a.WasActive;
                           target.OnNext(a.message);
                       });
                   }
                   else
                   {
                       Active = State.TargetWasActive;
                       target.OnNext(message);
                   }
               });


            SourceFilter
               .DistinctUntilChanged()
               .Subscribe(message =>
               {

                   Active = State.SourceActive;
                   DurationToSource = (message is IRate { Rate: { } irate }) ? TimeSpan.FromMilliseconds(irate) : durationToSource;

                   if (durationToSource > default(TimeSpan) &&   isRateSensitiveToSource)
                   {
                       Observable.Return((WasActive: State.SourceWasActive, message))
                       .Delay(DurationToSource)
                       .ObserveOnDispatcher()
                       .Subscribe(a =>
                       {
                           Active = a.WasActive;
                           source.OnNext(a.message);
                       });
                   }
                   else
                   {
                       Active = State.SourceWasActive;
                       source.OnNext(message);
                   }
               });

            inMessages
               .OfType<ClickMessage>()
               .Subscribe(a =>
               {
                   Text = DateTime.Now.ToString("T");
               });
        }

        public TimeSpan DurationToTarget
        {
            get => durationToTarget; set
            {
                durationToTarget = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan DurationToSource
        {
            get => durationToSource; set
            {
                durationToSource = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan Duration => duration.Value;



        public Filter SourceFilter { get; }

        public Filter TargetFilter { get; }
        /// <summary>
        /// Default parameterless constructor (for serialization compatibility)
        /// </summary>
        public Edge()
            : base(null, null, 1)
        {
        }

        /// <summary>
        /// Custom string property for example
        /// </summary>
        public string Text { get; set; }


        public int Angle { get; private set; }

        public override string ToString() => Text;

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Message value)
        {
            inMessages.OnNext(value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangingEventHandler? PropertyChanging;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChanging?.Invoke(this, args);
        }

        public void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
    }
}
