using DynamicData.Binding;
using Graph.Bayesian.WPF.ViewModel;
using System;

namespace Graph.Bayesian.WPF.Models.Vertices.Pagination
{
    public class SortVertex : Vertex
    {
        private bool isChecked;

        public SortVertex()
        {
            var comparer = isChecked ? SortExpressionComparer<SelectableKeyRecord>.Ascending(a => a.Key) : SortExpressionComparer<SelectableKeyRecord>.Descending(a => a.Key);
            Out.OnNext(new ListInputMessage(this.ID.ToString(), string.Empty, DateTime.Now, new ComparerInput<SelectableKeyRecord>(comparer)));
        }


        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                OnPropertyChanged();
                var comparer = value ? SortExpressionComparer<SelectableKeyRecord>.Ascending(a => a.Key) : SortExpressionComparer<SelectableKeyRecord>.Descending(a => a.Key);
                Out.OnNext(new ListInputMessage(this.ID.ToString(), string.Empty, DateTime.Now, new ComparerInput<SelectableKeyRecord>(comparer)));
            }
        }
    }
}
