using TCPChat.Shared.ViewModels;

namespace TCPChat.WPF
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
        }
    }
}
