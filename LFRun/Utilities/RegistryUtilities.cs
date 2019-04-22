using Microsoft.Win32;
using System;

namespace LFRun.Utilities
{
    public static class RegistryUtilities
    {
        private const string _configRegistryKey = @"Software\LuzFaltex\LFRun";

        public static void EnsureRegistryContract(string contractName, object initialValue, RegistryValueKind kind)
        {
            var contract = ReadRegistry(contractName);

            if (!contract.HasValue)
                WriteRegistry(contractName, initialValue, kind);
        }

        public static RegistryResult ReadRegistry(string valueName, object defaultValue = null)
        {
            using (RegistryKey reg = Registry.CurrentUser.OpenSubKey(_configRegistryKey))
            {
                return new RegistryResult(reg?.GetValue(valueName, defaultValue));
            }
        }

        public static void WriteRegistry(string valueName, object value, RegistryValueKind kind)
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

    public struct RegistryResult
    {
        public object Result { get; }

        public bool HasValue => !(null == Result);

        public RegistryResult(object result)
        {
            Result = result;
        }

        public T As<T>()
        {
            return (T) Result;
        }
    }
}
