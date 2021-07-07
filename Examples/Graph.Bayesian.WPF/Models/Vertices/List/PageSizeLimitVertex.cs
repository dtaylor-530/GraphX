using System;
using System.Linq;
using System.Reactive.Linq;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.ViewModel;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace Graph.Bayesian.WPF.Models.Vertices.Pagination
{
    public class PageSizeLimitVertex : LimitVertex
    {
        public PageSizeLimitVertex()
        {

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
                                return (input <= pagination.ItemsCounts && input>0);
                            });

            this.ValidationRule(vm => vm.Input, isValid, "Passwords must match.");

            Input = 10;
        }

        protected override Message CreateMessage(int value)
        {
            return new PaginationLimitMessage(this.ID.ToString(), string.Empty, DateTime.Now, value);
        }
    }

}
