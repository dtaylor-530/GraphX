using System;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.Models.Edges;


namespace Graph.Bayesian.WPF.Models.Vertices
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class MainVertex : Vertex
    {
        public MainVertex()
        {
            GraphVertex graphVertex = new();
            graphVertex.OnNext(new GraphMessage(string.Empty, string.Empty, DateTime.Now, CreateGraph(out var a, out var b, out _, out var d)));

            NavigationVertex = d;
            GraphVertex = graphVertex;
            MasterDetailVertex = new MasterDetailVertex(a, b);
        }

        public GraphVertex GraphVertex { get; }

        public MasterDetailVertex MasterDetailVertex { get; }

        public NavigationVertex NavigationVertex { get; }



        public static Graph CreateGraph(out ViewModelOutputVertex viewModelOutputVertex, out SelectionVertex selectionVertex, out CacheVertex cacheVertex, out NavigationVertex navigationVertex)
        {
            var dataGraph = new Graph();

            var factoryVertex = new FactoryVertex();
            factoryVertex.OnNext(new ViewModelFactory(factoryVertex.ID.ToString()));
            dataGraph.AddVertex(factoryVertex);

            var factory2Vertex = new FactoryVertex();
            factory2Vertex.OnNext(new ViewModelCreationFactory(factory2Vertex.ID.ToString()));
            dataGraph.AddVertex(factory2Vertex);


            viewModelOutputVertex = new ViewModelOutputVertex();
            dataGraph.AddVertex(viewModelOutputVertex);

            var catalogueVertex = new CatalogueVertex();
            dataGraph.AddVertex(catalogueVertex);
            catalogueVertex.OnNext(new ListService<ProductToken>(a => a.ProductId));

            var catalogue3Vertex = new CatalogueVertex();
            dataGraph.AddVertex(catalogue3Vertex);
            catalogue3Vertex.OnNext(new ListService<ProductToken>(a => a.ProductId));


            selectionVertex = new SelectionVertex();
            dataGraph.AddVertex(selectionVertex);
            selectionVertex.OnNext(new ListService<Selection>(a => a.Guid.ToString()));


            cacheVertex = new CacheVertex();
            dataGraph.AddVertex(cacheVertex);


            var storageVertex = new StorageVertex();
            dataGraph.AddVertex(storageVertex);


            navigationVertex = new NavigationVertex();
            dataGraph.AddVertex(navigationVertex);


            var listEditorVertex = new ListEditorVertex();
            dataGraph.AddVertex(listEditorVertex);


            var saveVertex = new SaveVertex();
            dataGraph.AddVertex(saveVertex);




            var dataEdge5 = new UnFilteredEdge(cacheVertex, viewModelOutputVertex);
            dataGraph.AddEdge(dataEdge5);


            //var dataEdge2 = new OneWayToTargetEdge(catalogueVertex, cacheVertex);
            //dataGraph.AddEdge(dataEdge2);

            var dataEdge13 = new Edge(factoryVertex, catalogueVertex, filterSource: new IdFilter(factoryVertex.ID.ToString()), filterTarget: new NoFilter());
            dataGraph.AddEdge(dataEdge13);



            var dataEdge1 = new Edge(factoryVertex, cacheVertex, filterSource: new IdFilter(factoryVertex.ID.ToString()), filterTarget: new NoFilter());
            dataGraph.AddEdge(dataEdge1);

            var dataEdge25 = new Edge(factory2Vertex, cacheVertex, filterSource: new IdFilter(factory2Vertex.ID.ToString()), filterTarget: new NoFilter());
            dataGraph.AddEdge(dataEdge25);

            //var dataEdge6 = new UnFilteredEdge(factory2Vertex, viewModelOutputVertex);
            //dataGraph.AddEdge(dataEdge6);

            var dataEdge15 = new Edge(factory2Vertex, catalogueVertex, filterSource: new IdFilter(factory2Vertex.ID.ToString()), filterTarget: new NoFilter());
            dataGraph.AddEdge(dataEdge15);


            var dataEdge16 = new Edge(factory2Vertex, catalogue3Vertex, filterSource: new IdFilter(factory2Vertex.ID.ToString()), filterTarget: new NoFilter());
            dataGraph.AddEdge(dataEdge16);

            var dataEdge14 = new OneWayToTargetEdge(listEditorVertex, selectionVertex);
            dataGraph.AddEdge(dataEdge14);

            var dataEdge22 = new UnFilteredEdge(selectionVertex, cacheVertex);
            dataGraph.AddEdge(dataEdge22);

            var dataEdge132 = new Edge(factoryVertex, selectionVertex, filterSource: new IdFilter(factoryVertex.ID.ToString()), filterTarget: new NoFilter());
            dataGraph.AddEdge(dataEdge132);

            var dataEdge133 = new UnFilteredEdge(cacheVertex, storageVertex);
            dataGraph.AddEdge(dataEdge133);


            var dataEdge134 = new UnFilteredEdge(saveVertex, storageVertex);
            dataGraph.AddEdge(dataEdge134);



            return dataGraph;
        }
    }
}
