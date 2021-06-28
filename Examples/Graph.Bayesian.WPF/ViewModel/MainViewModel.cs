using Graph.Bayesian.WPF.Models;
using Graph.Bayesian.WPF.Models.Vertices;

namespace Graph.Bayesian.WPF.ViewModel
{
    public class MainViewModel
    {
        public MainViewModel(ViewModelOutputVertex viewModelOutputVertex, CacheVertex cacheVertex, SelectionVertex selectionVertex)
        {
            ViewModelOutputVertex = viewModelOutputVertex;
            CacheVertex = cacheVertex;
            SelectionVertex = selectionVertex;
        }

        public ViewModelOutputVertex ViewModelOutputVertex { get; }

        public CacheVertex CacheVertex { get; }

        public SelectionVertex SelectionVertex { get; }
    }
}
