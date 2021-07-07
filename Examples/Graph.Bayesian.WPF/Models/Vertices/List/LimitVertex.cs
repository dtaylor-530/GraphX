using Graph.Bayesian.WPF.Infrastructure;

namespace Graph.Bayesian.WPF.Models.Vertices.Pagination
{
    public abstract class LimitVertex : Vertex
    {
        private int input;

        public LimitVertex()
        {
        }

        public int Input
        {
            get => input;
            set
            {
                input = value;
                OnPropertyChanged(nameof(Input));
                Out.OnNext(CreateMessage(value));
            }
        }

        protected abstract Message CreateMessage(int value);
    }
}
