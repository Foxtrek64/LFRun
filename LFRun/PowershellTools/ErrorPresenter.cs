using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using LFRun.Utilities;

namespace LFRun.PowershellTools
{
    public class ErrorPresenter
    {
        public string Command { get; }
        public IResult Result { get; }

        public ErrorPresenter(string command, IResult result)
        {
            Command = string.IsNullOrWhiteSpace(command)
                ? throw new ArgumentNullException(nameof(command))
                : command;
            Result = result ?? throw new ArgumentNullException(nameof(result));
        }

        public void Show()
        {
            TypeSwitch.Do(
                Result.Exception,
                TypeSwitch.Case<DirectoryNotFoundException>(() =>
                {
                    MessageBox.Show(
                        $"{Command} is unavailable. If the location is on this PC, make sure the device or drive is connected or the disc is inserted, and then try again. If the location is on the network, make sure you're connected to the network or Internet, and then try again. If the location still can't be found, it might have been moved or deleted.",
                        "Location is not available", MessageBoxButton.OK, MessageBoxImage.Error);
                }),
                // TypeSwitch.Case<Win32Exception>()
                TypeSwitch.Default(() =>
                {
                    string message = Debugger.IsAttached ? $"Exception Type: {Result.Exception.GetType()}" : "";
                    message += Environment.NewLine;
                    message +=
                        $"Windows cannot find '{Command}'. Make sure you typed the name correctly, and then try again.";

                    MessageBox.Show(message,
                        Command, MessageBoxButton.OK, MessageBoxImage.Error);
                }));
        }
    }
}
