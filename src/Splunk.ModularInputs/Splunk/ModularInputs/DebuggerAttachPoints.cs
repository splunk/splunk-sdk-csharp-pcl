using System;

namespace Splunk.ModularInputs
{
    /// <summary>
    /// The <see cref="ModularInput"/> class represents the functionality of a
    /// modular input script (that is, an executable).
    /// </summary>
    /// <remarks>
    /// <para>
    /// An application derives from this class to define a modular input. It
    /// must override the <see cref="Scheme"/> and <see cref="StreamEvents"/>
    /// methods. It can optionally override the <see cref="Validate"/> method.
    /// </para>
    /// </remarks>
    /// 

    [Flags]
    public enum DebuggerAttachPoints
    {
        None,
        Scheme,
        ValidateArguments,
        StreamEvents,
        All
    }
}