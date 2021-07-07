using System;
using System.Linq;
using System.Reactive.Linq;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.ViewModel;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace Graph.Bayesian.WPF.Models.Vertices.Pagination
{
    //public class CountVertex : LimitVertex
    //{
    //    protected override Message CreateMessage(int value)
    //    {
    //        return new PaginationCountMessage(this.ID.ToString(), string.Empty, DateTime.Now, value);
    //    }
    //}

    public class CurrentPageVertex : LimitVertex
    {
        public CurrentPageVertex()
        {
            In
                .OfType<MovementMessage>()
                .Select(a => a.Movement)
                .Subscribe(a =>
                {
                    switch (a)
                    {
                        case History.Movement.Forward:
                            Input++;
                            break;
                        case History.Movement.Backward:
                            Input--;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });



            var isValid = In
                            .OfType<ListInputMessage>()
                            .Select(a => a.ListInput)
                            .OfType<PageRequestInput>()
                            .Select(a => a.PageRequest)
                            .CombineLatest(this.WhenAnyValue(a => a.Input))
                            .Select(a =>
                            {
                                var input = this.Input;
                                var (pagination, va) = a;
                                return (input <= pagination.End && input >= pagination.Start);
                            });

            this.ValidationRule(vm => vm.Input, isValid, "Passwords must match.");

            Input = 1;
        }


        protected override Message CreateMessage(int value)
        {
            return new PaginationCurrentMessage(this.ID.ToString(), string.Empty, DateTime.Now, value);
        }
    }

}
