using System.Windows;
using System.Windows.Input;

namespace LFRun
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public ProjectAttributes ProjectAttributes { get; }

        public ICommand CloseCommand { get; }

        public AboutWindow()
        {
            CloseCommand = new RelayCommand(
                _ => Close(),
                _ => true);

            ProjectAttributes = new ProjectAttributes();
            InitializeComponent();
        }
    }
}
