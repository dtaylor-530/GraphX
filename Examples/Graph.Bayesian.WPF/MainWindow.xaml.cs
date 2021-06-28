using System;
using System.Linq;
using System.Windows;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.Models;
using Graph.Bayesian.WPF.Models.Edges;
using Graph.Bayesian.WPF.Models.Vertices;
using Graph.Bayesian.WPF.View;
using Graph.Bayesian.WPF.ViewModel;

using Ninject;
using ReactiveUI;
using Splat;
using Splat.Ninject;

namespace Graph.Bayesian.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var kernel = new StandardKernel();
            kernel.Bind<IViewFor<GraphViewModel>>().To<GraphView>();
            kernel.Bind<IViewFor<MainViewModel>>().To<MainView>();
            //kernel.Bind<SecondaryViewModel>().ToSelf();
            kernel.UseNinjectDependencyResolver();

            GraphViewModel graphViewModel = new();
            graphViewModel.OnNext(MainWindow.Create5(out var a, out var b, out var c));

            GraphViewModelViewHost.ViewModel = graphViewModel;

            MainViewModelViewHost.ViewModel = new MainViewModel(a, c, b);
        }


        public static Models.Graph Create5(out ViewModelOutputVertex viewModelOutputVertex, out SelectionVertex selectionVertex, out CacheVertex cacheVertex)
        {
            var dataGraph = new Models.Graph();

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
            catalogueVertex.OnNext(new ListService<ProductToken>(a=>a.ProductId));       
            
            var catalogue3Vertex = new CatalogueVertex();
            dataGraph.AddVertex(catalogue3Vertex);
            catalogue3Vertex.OnNext(new ListService<ProductToken>(a=>a.ProductId));

            
            selectionVertex = new SelectionVertex();
            dataGraph.AddVertex(selectionVertex);
            selectionVertex.OnNext(new ListService<Selection>(a => a.Guid.ToString()));


            cacheVertex = new CacheVertex();
            dataGraph.AddVertex(cacheVertex);

            var listEditorVertex = new ListEditorVertex();
            dataGraph.AddVertex(listEditorVertex);


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



            return dataGraph;
        }
    }
}
