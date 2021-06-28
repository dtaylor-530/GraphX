using ReactiveUI;

namespace Graph.Bayesian.WPF.View
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView 
    {
        public MainView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                LeftContentControl.Content = this.ViewModel.SelectionVertex;

                MainContentControl.Content = this.ViewModel.ViewModelOutputVertex;

            });
        }
    }
}
