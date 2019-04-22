using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace LFRun.PowershellTools
{
    public static class PowershellTools
    {
        /// <summary>
        /// Attempts to execute a command on the command line.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        public static ExecutionResult ExecuteCommand(string command)
        {
            using (PowerShell shell = PowerShell.Create())
            {
                // TODO: Set execution policy
                // TODO: Set Working Directory
            }

            return ExecutionResult.FromSuccess();
        }
    }
}
