using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Graph.Bayesian.WPF.Models;
using GraphX.Controls;

namespace Graph.Bayesian.WPF.View {
   /// <summary>
   /// Interaction logic for ModelView.xaml
   /// </summary>
   public partial class ModelView : UserControl {
      public ModelView() {
         InitializeComponent();
         ZoomControl.SetViewFinderVisibility(zoomctrl, Visibility.Collapsed);

      }


      public Models.Graph Graph {
         get => (Models.Graph)GetValue(GraphProperty);
         set => SetValue(GraphProperty, value);
      }

      public static readonly DependencyProperty GraphProperty =
         DependencyProperty.Register("Graph", typeof(Models.Graph), typeof(ModelView), new PropertyMetadata(null, Changed));

      private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e) {
         (d as ModelView).Area.LogicCore = GraphBuilder.CreateLogicCore(e.NewValue as Models.Graph);
         GraphBuilder.ConfigureArea((d as ModelView).Area);
         (d as ModelView).Area.RelayoutGraph();
         (d as ModelView).zoomctrl.ZoomToFill();
      }
   }
}
