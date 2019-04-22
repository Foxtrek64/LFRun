using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using LFRun.Commands;
using LFRun.PowershellTools;
using LFRun.Utilities;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace LFRun.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand AboutCommand { get; }

        public ICommand CancelCommand { get; }

        public ICommand ExecuteCommand { get; }

        public ICommand ShowMenuCommand { get; }

        public ICommand InputComboBoxLoaded { get; }

        public ICommand SaveHistoryMenuItemLoaded { get; }

        public ICommand MenuItemLostFocusCommand { get; }

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
            set
            {
                if (SetProperty(ref _saveHistory, value))
                    RegistryUtilities.WriteRegistry("SaveHistory", value ? 1 : 0, RegistryValueKind.DWord);
            }
        }

        private string _inputComboBoxText = "";

        public string InputComboBoxText
        {
            get => _inputComboBoxText;
            set => SetProperty(ref _inputComboBoxText, value);
        }

        private readonly ObservableCollection<CommandRecord> _history;

        public IReadOnlyCollection<CommandRecord> History { get; }

        private bool _showMenu = false;
        public bool ShowMenu
        {
            get => _showMenu;
            set => SetProperty(ref _showMenu, value);
        }

        public MainWindowViewModel()
        {
            AboutCommand = new RelayCommand(
                param => new AboutWindow().ShowDialog());

            CancelCommand = new RelayCommand(
                _ => Application.Current.MainWindow.Close());

            ExecuteCommand = new RelayCommand(
                async _ =>
                {
                    Application.Current.MainWindow.Hide();
                    // var result = new CommandHandler(InputComboBoxText).Execute();
                    var result = await new PowershellTool(InputComboBoxText).Execute();

                    if (result.Success)
                    {
                        var command = new CommandRecord(InputComboBoxText);

                        // Contains uses the default equality check for CommandRecord.
                        // The default equality check for CommandRecord is timestamp-agnostic.
                        // Rather, it compares the command string.
                        // So, we abuse this - if the command string exists, delete it. We'll replace it in the next step.
                        if (_history.Contains(command))
                            _history.Remove(command);

                        _history.Add(new CommandRecord(InputComboBoxText));

                        if (SaveHistory)
                        {
                            List<CommandRecord> history =
                                History
                                    .OrderBy(o => o.CommandDate)
                                    .ToList()
                                    .GetRange(0, Math.Min(History.Count, 50));

                            string json = JsonConvert.SerializeObject(history);

                            RegistryUtilities.WriteRegistry("History", json, RegistryValueKind.String);
                        }

                        Application.Current.MainWindow.Close();
                    }
                    else
                    {
                        Application.Current.MainWindow.Show();
                        MessageBox.Show($"User ran: {InputComboBoxText}{Environment.NewLine}{Environment.NewLine}{result.Exception.ToString()}", "An error has occurred");
                    }
                },
                _ => !string.IsNullOrWhiteSpace(InputComboBoxText));

            InputComboBoxLoaded = new RelayCommand(
                _ => GetCommandHistory()
                    .ForEach(_history.Add));

            SaveHistoryMenuItemLoaded = new RelayCommand(
                _ => SaveHistory = Convert.ToBoolean(RegistryUtilities.ReadRegistry("SaveHistory", 1).As<int>()));

            MenuItemLostFocusCommand = new RelayCommand(
                _ => ShowMenu = false);

            _history = new ObservableCollection<CommandRecord>();

            History = new ReadOnlyObservableCollection<CommandRecord>(_history);
        }

        public static List<CommandRecord> GetCommandHistory()
            => JsonConvert.DeserializeObject<List<CommandRecord>>(
                RegistryUtilities.ReadRegistry("History", "[]").As<string>());
    }
}
