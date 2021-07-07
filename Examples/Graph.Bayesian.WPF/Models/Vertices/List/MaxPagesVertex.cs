using System;
using System.Linq;
using System.Reactive.Linq;
using Graph.Bayesian.WPF.Infrastructure;
using ReactiveUI.Validation.Extensions;

namespace Graph.Bayesian.WPF.Models.Vertices.Pagination
{
    public class MaxPagesVertex : LimitVertex
    {
        public MaxPagesVertex()
        {
            var a = this.WhenAnyPropertyHasChanged(nameof(Input)).Select(a => Input > 0);
            this.ValidationRule(vm => vm.Input, a, "Passwords must match.");

            Input = 4;
        }

        protected override Message CreateMessage(int value)
        {
            return new PaginationMaxPagesMessage(this.ID.ToString(), string.Empty, DateTime.Now, value);
        }
    }

}
