using System;
using System.Linq;
using System.Windows;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.Models;
using Graph.Bayesian.WPF.Models.Edges;
using Graph.Bayesian.WPF.Models.Vertices;
using Graph.Bayesian.WPF.View;
using Graph.Bayesian.WPF.Vertices;

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

            //var kernel = new StandardKernel();
            //kernel.Bind<IViewFor<GraphVertex>>().To<GraphView>();
            //kernel.Bind<IViewFor<MainViewModel>>().To<MainView>();
            ////kernel.Bind<SecondaryViewModel>().ToSelf();
            //kernel.UseNinjectDependencyResolver();

            MainContentControl.Content = new MainVertex();
        }
    }
}
