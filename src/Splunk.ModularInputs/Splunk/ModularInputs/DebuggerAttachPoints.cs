using System;

namespace Splunk.ModularInputs
{
    /// <summary>
    /// The <see cref="DebuggerAttachPoints"/> contains a list of points in the
    /// modular input pipeline that one can can specify to have the input pause
    /// until a debugger attaches
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// 

    [Flags]
    public enum DebuggerAttachPoints
    {
        None=1,
        Scheme,
        ValidateArguments,
        StreamEvents,
        All
    }
}