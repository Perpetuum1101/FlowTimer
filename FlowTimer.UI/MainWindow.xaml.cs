using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using FlowTimer.UI.Model;

namespace FlowTimer.UI
{
    public partial class MainWindow : Window
    {
        public ViewModel Vm;
        public MainWindow()
        {
            Vm = new ViewModel(Application.Current.Dispatcher);
            DataContext = Vm;
            InitializeComponent();
        }

        public async void OnWindowClosing(object sender, CancelEventArgs e)
        {
            await Vm.OnCloseAsync();
        }
    }
}
