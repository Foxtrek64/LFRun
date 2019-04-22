using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LFRun.PowershellTools
{
    public static class ExecutionTools
    {
        [DllImport("shell32.dll", SetLastError = true)]
        private static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCommandLine,
            out int pNumArgs);

        public static string[] CommandLineToArgs(string commandLine)
        {
            var argv = CommandLineToArgvW(commandLine, out int argc);
            if (argv == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception();

            try
            {
                var args = new string[argc];
                for (var i = 0; i < args.Length; i++)
                {
                    var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    args[i] = Marshal.PtrToStringUni(p);
                }

                return args;
            }
            finally
            {
                Marshal.FreeHGlobal(argv);
            }
        }

        public static string FindCommand(string command)
        {
            string commandWithPath = null;

            // If it's a path, let's not waste time trying to figure where the executable exists.
            if (File.Exists(command))
            {
                return command;
            }
            else if (Directory.Exists(command))
            {
                FileAttributes attr = File.GetAttributes(command);
                if (attr.HasFlag(FileAttributes.Directory))
                    return $"start {command}";
            }

            // It's probably just a command. Let's try attaching it to a path.
            string pathVar = Environment.GetEnvironmentVariable("PATH");
            if (!Environment.Is64BitProcess)
                pathVar += ";C:\\Windows\\sysnative\\";
                // pathVar += $";{Environment.SystemDirectory}";

            var paths = pathVar.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);

            foreach (string directory in paths)
            {
                var d = new DirectoryInfo(directory);

                if (!d.Exists)
                    // We found a directory that's in the PATH variable that doesn't exist on the machine.
                    // Just move on
                    continue;

                FileInfo fileMatch = d.EnumerateFiles($"{command}.*", SearchOption.TopDirectoryOnly).FirstOrDefault();

                if (null != fileMatch)
                {
                    commandWithPath = fileMatch.FullName;
                    break;
                }
            }

            return commandWithPath ?? command;

        }

        private static string[] SplitCommand(string command)
        {
            var parts = Regex.Matches(command, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToArray();

            return parts;
        }
    }
}
