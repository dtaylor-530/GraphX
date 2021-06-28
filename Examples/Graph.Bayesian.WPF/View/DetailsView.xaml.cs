using System.Windows;
using System.Windows.Controls;
using Graph.Bayesian.WPF.Models;
using GraphX.Controls;

namespace Graph.Bayesian.WPF.View {
   /// <summary>
   /// Interaction logic for DetailsView.xaml
   /// </summary>
   public partial class DetailsView : UserControl {
      public DetailsView() {
         InitializeComponent();

         ZoomControl.SetViewFinderVisibility(zoomctrl, Visibility.Collapsed);


      }


      public Models.Graph Graph {
         get => (Models.Graph)GetValue(GraphProperty);
         set => SetValue(GraphProperty, value);
      }

      public static readonly DependencyProperty GraphProperty =
         DependencyProperty.Register("Graph", typeof(Models.Graph), typeof(DetailsView), new PropertyMetadata(null, Changed));

      private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
         (d as DetailsView).Area.LogicCore = GraphBuilder.CreateLogicCore(e.NewValue as Models.Graph);
         GraphBuilder.ConfigureArea((d as DetailsView).Area);
         //(d as DetailsView).Area.RelayoutGraph();
         (d as DetailsView).zoomctrl.ZoomToOriginal();
      }
   }
}
