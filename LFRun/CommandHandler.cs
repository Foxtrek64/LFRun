using LFRun.PowershellTools;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace LFRun
{
    public class CommandHandler
    {
        private ExecutionResult result;
        private readonly ProcessStartInfo psi;

        public CommandHandler(string command)
        {
            string[] commandParts = ExecutionTools.CommandLineToArgs(command);
            psi = new ProcessStartInfo(commandParts[0])
            {
                Arguments = string.Join(" ", commandParts.Skip(1)),
                Verb = "runas",
                CreateNoWindow = true,
                UseShellExecute = true
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
                    process.WaitForExit();
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
