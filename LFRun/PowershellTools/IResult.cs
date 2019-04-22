using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFRun.PowershellTools
{
    public interface IResult
    {
        bool Success { get; }
        string Message { get; }
        Exception Exception { get; }
    }
}
