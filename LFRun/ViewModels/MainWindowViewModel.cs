using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Win32;

namespace LFRun.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly MainWindow _mainWindow;

        public ICommand AboutCommand { get; }

        public ICommand CancelCommand { get; }

        public ICommand ExecuteCommand { get; }

        public ICommand SaveHistoryCheckedCommand { get; }

        private string _runButtonText = "Run";

        public string RunButtonText
        {
            get => _runButtonText;
            set => SetProperty(ref _runButtonText, value);
        }

        private bool _saveHistory;

        public bool SaveHistory
        {
            get => _saveHistory;
            set => SetProperty(ref _saveHistory, value);
        }

        private ObservableCollection<CollectionViewSource> _history;

        public ObservableCollection<CollectionViewSource> History
        {
            get => _history;
            set => SetProperty(ref _history, value);
        }

        private bool _showMenu = false;
        public bool ShowMenu
        {
            get => _showMenu;
            set => SetProperty(ref _showMenu, value);
        }

        private const string _configRegistryKey = @"Software\LuzFaltex\LFRun";

        public MainWindowViewModel()
        {
            _mainWindow = (MainWindow)Application.Current.MainWindow;

            AboutCommand = new RelayCommand(
                param => new AboutWindow().ShowDialog());

            CancelCommand = new RelayCommand(
                _ => _mainWindow.Close());

            ExecuteCommand = new RelayCommand<string>(
                ExecuteUserInput,
                param => !string.IsNullOrWhiteSpace(param));

            SaveHistoryCheckedCommand = new RelayCommand(
                _ => WriteRegistry("SaveHistory", SaveHistory ? 1 : 0, RegistryValueKind.DWord));

            _saveHistory = GetShouldSaveHistory();

        }

        public void ExecuteUserInput(string command)
        {
            MessageBox.Show(command);
        }

        private static bool GetShouldSaveHistory()
        {
            object shouldSave = ReadRegistry("SaveHistory");

            if (null == shouldSave)
                WriteRegistry("SaveHistory", 1, RegistryValueKind.DWord);

            return true;
        }

        private static object ReadRegistry(string valueName, object defaultValue = null)
        {
            using (RegistryKey reg = Registry.CurrentUser.OpenSubKey(_configRegistryKey))
            {
                return reg?.GetValue(valueName, defaultValue);
            }
        }

        private static void WriteRegistry(string valueName, object value, RegistryValueKind kind)
        {
            valueName = valueName ?? throw new ArgumentNullException(nameof(valueName));
            value = value ?? throw new ArgumentNullException(nameof(value));

            using (RegistryKey reg = Registry.CurrentUser.OpenSubKey(_configRegistryKey, true))
            {
                var reg2 = reg ?? Registry.CurrentUser.CreateSubKey(_configRegistryKey, true);

                reg2.SetValue(valueName, value, kind);
            }
        }
    }
}
