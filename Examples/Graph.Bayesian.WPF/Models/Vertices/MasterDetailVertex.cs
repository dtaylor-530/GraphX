using Graph.Bayesian.WPF.Infrastructure;
using GraphX.Common.Enums;
using GraphX.Common.Interfaces;
using System;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public class MasterDetailVertex : IIdentifiableGraphDataObject
    {
        private readonly Lazy<long> id = new(() => IDFactory.Get);

        public MasterDetailVertex(ViewModelOutputVertex viewModelOutputVertex,  SelectionVertex selectionVertex)
        {
            ViewModelOutputVertex = viewModelOutputVertex;
            SelectionVertex = selectionVertex;
        }

        public ViewModelOutputVertex ViewModelOutputVertex { get; }

        public SelectionVertex SelectionVertex { get; }

        public long ID { get => id.Value; set => throw new NotImplementedException(); }

        public ProcessingOptionEnum SkipProcessing { get; set; }
    }
}
