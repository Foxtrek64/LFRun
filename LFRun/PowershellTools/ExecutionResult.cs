using System;
using System.IO;

namespace LFRun.PowershellTools
{
    public sealed class ExecutionResult : IResult
    {
        public bool Success => Exception == null;

        public string Message { get; }

        public Exception Exception { get; }

        private ExecutionResult(string message, Exception exception = null)
        {
            Message = message;
            Exception = exception;
        }

        public static ExecutionResult FromSuccess(string message = "") => new ExecutionResult(message);
        public static ExecutionResult FromError(string message, Exception exception) => new ExecutionResult(message, exception);
    }
}
