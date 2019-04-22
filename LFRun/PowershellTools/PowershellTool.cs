using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Debugger = System.Diagnostics.Debugger;

namespace LFRun.PowershellTools
{
    public class PowershellTool
    {
        public string Script { get; }

        public PowershellTool(string commandLine)
        {
            string[] commandParts = ExecutionTools.CommandLineToArgs(commandLine);
            // string command = ExecutionTools.GetFullyQualifiedExecutable(commandParts[0]);

            const string sysnative = ";C:\\Windows\\sysnative\\";

            StringBuilder psScript = new StringBuilder();

            if (Debugger.IsAttached)
                psScript.AppendLine("Set-PSDebug -Trace 1;");

            if (Environment.Is64BitProcess)
                psScript.AppendLine(
                    $"Set-Location -Path '{Environment.GetFolderPath(Environment.SpecialFolder.System)}';");
            else
                psScript.AppendLine(
                    $"Set-Location -Path '{sysnative}'");

            psScript.Append($"Start-Process -FilePath '{commandParts[0]}' ");
            if (commandParts.Length > 1)
                psScript.Append($"-ArgumentList '{string.Join(" ", commandParts.Skip(1))}' ");
            psScript.AppendLine("-Verb RunAs;");

            if (Debugger.IsAttached)
                psScript.AppendLine("Read-Host -Prompt 'Press ENTER to exit'");

            Script = psScript.ToString();
        }

        public async Task<ExecutionResult> Execute()
        {
            ExecutionResult executionResult;

            using (PowerShell shell = PowerShell.Create())
            {
                shell.AddScript(Script);

                var results =
                    await Task<PSDataCollection<PSObject>>.Factory.FromAsync(shell.BeginInvoke(),
                        param => shell.EndInvoke(param));

                if (shell.HadErrors)
                {
                    MessageBox.Show(string.Join(Environment.NewLine, results), "Something went wrong!");

                    var ex = new Exception("Something went wrong!");
                    executionResult = ExecutionResult.FromError(ex.Message, ex);
                }
                else
                {
                    executionResult = ExecutionResult.FromSuccess();
                }
            }

            return executionResult;
        }
    }
}
