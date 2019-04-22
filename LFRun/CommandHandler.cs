using LFRun.PowershellTools;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace LFRun
{
    public class CommandHandler
    {
        private ExecutionResult result;
        private readonly ProcessStartInfo psi;

        public CommandHandler(string commandLine)
        {
            var commandParts = ExecutionTools.CommandLineToArgs(commandLine);
            string command = ExecutionTools.FindCommand(commandParts[0]);

            psi = new ProcessStartInfo(command)
            {
                Arguments = string.Join(" ", commandParts.Skip(1)),
                CreateNoWindow = true,
                UseShellExecute = false,
                Verb = "runas"
            };
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <returns>True if successful; otherwise, false</returns>
        public ExecutionResult Execute()
        {
            using (Process process = new Process())
            {
                process.StartInfo = psi;

                try
                {
                    process.Start();

                    if (process.HasExited && process.ExitCode != 0)
                        result = ExecutionResult.FromError(process.StandardOutput.ToString(), null);
                    else
                        result = ExecutionResult.FromSuccess();
                }
                catch (Win32Exception we)
                {
                    // If the user cancelled the request
                    if (we.NativeErrorCode == 1223)
                        // Swallow the error - we want to treat this as a success state.
                        result = ExecutionResult.FromSuccess();
                    else
                        // Some other Win32 error - treat it as normal.
                        result = ExecutionResult.FromError(we.Message, we);

                }
                catch (Exception e)
                {
                    result = ExecutionResult.FromError(e.Message, e);
                }

                return result;
            }
        }
    }
}
