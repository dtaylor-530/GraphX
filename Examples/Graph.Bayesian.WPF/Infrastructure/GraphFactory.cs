using System;
using System.Linq;
using Graph.Bayesian.WPF.Models;
using Graph.Bayesian.WPF.Models.Edges;
using Graph.Bayesian.WPF.Models.Vertices;
using Graph.Bayesian.WPF.Models.Vertices.History;

namespace Graph.Bayesian.WPF.Infrastructure
{
    class GraphFactory
    {

        public static Models.Graph BuildEmpty()
        {
            var dataGraph = new Models.Graph();
            var vertex = new Vertex();
            dataGraph.AddVertex(vertex);
            var vertex2 = new Vertex();
            dataGraph.AddVertex(vertex2);
            return dataGraph;
        }

        public static Models.Graph Create4()
        {
            var dataGraph = new Models.Graph();

            var historyInputVertex = new HistoryIOVertex();
            dataGraph.AddVertex(historyInputVertex);

            var historyVertex = new HistoryVertex();
            dataGraph.AddVertex(historyVertex);

            var historyCurrentVertex = new HistoryCurrentVertex();
            dataGraph.AddVertex(historyCurrentVertex);    
            
            var movementVertex = new MovementVertex();
            dataGraph.AddVertex(movementVertex);

            var dataEdge5 = new UnFilteredEdge(historyInputVertex, historyVertex);
            dataGraph.AddEdge(dataEdge5);    
            
            var dataEdge6 = new OneWayToTargetEdge(historyVertex, historyCurrentVertex);
            dataGraph.AddEdge(dataEdge6);  
            
            var dataEdge7 = new UnFilteredEdge(movementVertex, historyVertex);
            dataGraph.AddEdge(dataEdge7);

            return dataGraph;
        }

        public static Models.Graph Create3()
        {
            var dataGraph = new Models.Graph();

            var timerVertex = new TimerVertex();
            dataGraph.AddVertex(timerVertex);

            var dataVertex = new DataVertex();
            dataGraph.AddVertex(dataVertex);

            var dataVertex2 = new DataVertex();
            dataGraph.AddVertex(dataVertex2);

            var dataInputVertex = new DataInputVertex();
            dataGraph.AddVertex(dataInputVertex);

            var historyVertex = new HistoryVertex();
            dataGraph.AddVertex(historyVertex);

            var dataEdge6 = new UnFilteredEdge(dataVertex, historyVertex);
            dataGraph.AddEdge(dataEdge6);

            var dataEdge5 = new OneWayToTargetEdge(dataInputVertex, dataVertex);
            dataGraph.AddEdge(dataEdge5);

            var dataEdge7 = new OneWayToTargetEdge(timerVertex, dataVertex);
            dataGraph.AddEdge(dataEdge7);

            var dataEdge3 = new Edge(dataVertex, dataVertex2, isRateSensitiveToSource:true, isRateSensitiveToTarget:true);
            dataGraph.AddEdge(dataEdge3);

            return dataGraph;
        }


        public class ConnectionsFactory
        {

            public static Models.Graph Create5()
            {
                var dataGraph = new Models.Graph();

                var factoryVertex = new FactoryVertex();
                dataGraph.AddVertex(factoryVertex);
                factoryVertex.OnNext(new ViewModelFactory(factoryVertex.ID.ToString()));

                var viewModelOutputVertex = new ViewModelOutputVertex();
                dataGraph.AddVertex(viewModelOutputVertex);

                var catalogueVertex = new CatalogueVertex();
                dataGraph.AddVertex(catalogueVertex);   
                
                var cacheVertex = new CacheVertex();
                dataGraph.AddVertex(cacheVertex);    
                
                var listEditorVertex = new ListEditorVertex();
                dataGraph.AddVertex(listEditorVertex);

                var dataEdge5 = new Edge(cacheVertex, viewModelOutputVertex);
                dataGraph.AddEdge(dataEdge5);

                var dataEdge1 = new Edge(factoryVertex, cacheVertex);
                dataGraph.AddEdge(dataEdge1);   
                
                var dataEdge2 = new OneWayToTargetEdge(catalogueVertex, cacheVertex);
                dataGraph.AddEdge(dataEdge2);

                var dataEdge13 = new OneWayToTargetEdge(factoryVertex, catalogueVertex);
                dataGraph.AddEdge(dataEdge13);     
                
                var dataEdge14 = new OneWayToTargetEdge(listEditorVertex, catalogueVertex);
                dataGraph.AddEdge(dataEdge14);

                return dataGraph;
            }

            public static Models.Graph Create2(out Vertex connectionVertex, out Vertex generalVertex)
            {
                var dataGraph = new Models.Graph();
                var controllerVertex = new ControllerVertex();
                var timerVertex = new TimerVertex();

                var clickVertex = new ClickServiceVertex();
                var selectVertex = new SelectServiceVertex();
                generalVertex = new GeneralVertex();

                var arr = new Vertex[] { controllerVertex, timerVertex, /*keyVertex, */ clickVertex, selectVertex };

                dataGraph.AddVertex(generalVertex);

                foreach (var vertex in arr)
                {
                    dataGraph.AddVertex(vertex);
                }

                var dataEdge2 = new FilterOnSourceEdge(timerVertex, controllerVertex) { };
                dataGraph.AddEdge(dataEdge2);

                var dataEdge3 = new Edge(timerVertex, clickVertex) { };
                dataGraph.AddEdge(dataEdge3);

                var dataEdge4 = new Edge(timerVertex, selectVertex) { };
                dataGraph.AddEdge(dataEdge4);

                var dataEdge5 = new Edge(controllerVertex, generalVertex) { };
                dataGraph.AddEdge(dataEdge5);

                connectionVertex = new NavigationVertex();
                dataGraph.AddVertex(connectionVertex);

                var dataEdge6 = new Edge(generalVertex, clickVertex) { };
                dataGraph.AddEdge(dataEdge6);

                dataGraph.AddEdge(new Edge(selectVertex, connectionVertex));

                return dataGraph;
            }

            public static Models.Graph Create(Vertex connectionVertex)
            {

                var dataGraph = new Models.Graph();
                //var navigationVertex = new NavigationVertex();

                dataGraph.AddVertex(connectionVertex);
                //dataGraph.AddVertex(navigationVertex);
                //dataGraph.AddEdge(new Edge(navigationVertex, connectionVertex));

                return dataGraph;
            }


            public static Models.Graph Create2(Vertex connectionVertex)
            {

                var dataGraph = new Models.Graph();

                dataGraph.AddVertex(connectionVertex);
                //dataGraph.AddVertex(dataVertex);
                //dataGraph.AddVertex(navigationVertex);
                //dataGraph.AddEdge(new Edge(dataVertex, connectionVertex));

                BuildDataVertexGraph(dataGraph, connectionVertex);

                static void BuildDataVertexGraph(Models.Graph graphExample, Vertex generalVertex)
                {

                    for (int i = 1; i < 3; i++)
                    {

                        var dataVertex = new DataVertex();
                        graphExample.AddVertex(dataVertex);
                        var dataEdge = new FilterOnSourceEdge(dataVertex, generalVertex) { };
                        graphExample.AddEdge(dataEdge);
                    }
                    var vertexList = graphExample.Vertices.OfType<DataVertex>().ToList();

                    for (int i = 0; i < vertexList.Count - 1; i++)
                    {
                        var dataEdge = new Edge(vertexList[i], vertexList[i + 1], isRateSensitiveToTarget:true,isRateSensitiveToSource:true) { Text = $"{vertexList[i]} -> {vertexList[i + 1]}" };
                        graphExample.AddEdge(dataEdge);
                    }

                    //vertexList.Last().OnNext(new DataMessage("", vertexList.Last().ID.ToString(), 0));
                }
                return dataGraph;
            }
        }

  
    }
}
