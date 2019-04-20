using System.Windows;
using System.Windows.Input;

namespace LFRun.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Window _mainWindow;

        public ICommand AboutCommand { get; }

        public ICommand CancelCommand { get; }

        public ICommand ExecuteCommand { get; }

        private string _runButtonText = "Run";

        public string RunButtonText
        {
            get => _runButtonText;
            set => SetProperty(ref _runButtonText, value);
        }

        public MainWindowViewModel()
        {
            _mainWindow = (MainWindow)Application.Current.MainWindow;

            AboutCommand = new RelayCommand(
                param => new AboutWindow().ShowDialog(),
                param => true);

            CancelCommand = new RelayCommand(
                _ => _mainWindow.Close(),
                _ => true);

            ExecuteCommand = new RelayCommand<string>(
                param => MessageBox.Show($"User said: {param}"),
                param => !string.IsNullOrWhiteSpace(param));
        }
    }
}
