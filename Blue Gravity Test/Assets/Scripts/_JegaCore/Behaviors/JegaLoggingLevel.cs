using System;

namespace JegaCore
{
    [Flags]
    public enum JegaLoggingLevel {None = 0, Verbose = 1, Logs = 2, Warnings = 4, Errors = 8}
}