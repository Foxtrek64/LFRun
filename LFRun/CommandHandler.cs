using LFRun.PowershellTools;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LFRun
{
    public class CommandHandler
    {
        private ExecutionResult result;
        private readonly ProcessStartInfo psi;

        public CommandHandler(string commandLine)
        {
            /*
            psi = new ProcessStartInfo("powershell", $"-ExecutionPolicy unrestricted \"{psScript.ToString()}\"")
            {
                // CreateNoWindow = true,
                // UseShellExecute = true
            };
            */
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
                    result = process.ExitCode == 0
                        ? ExecutionResult.FromSuccess()
                        : ExecutionResult.FromError(process.StandardOutput.ToString(), null);
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
