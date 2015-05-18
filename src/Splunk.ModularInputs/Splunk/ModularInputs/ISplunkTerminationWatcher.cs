using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splunk.ModularInputs
{
    public interface ISplunkTerminationWatcher
    {
        /// <summary>
        /// A flag indicating if Splunk is in the process of shutting down.
        /// </summary>
        bool ShutdownRequested { get; }
        /// <summary>
        /// A flag indicating if Splunk has terminated.
        /// </summary>
        bool SplunkTerminated { get; }
        /// <summary>
        /// An event handler that fires when Splunk is being shut down.
        /// </summary>
        event EventHandler<EventArgs> ShutdownRequestedEvent;
        /// <summary>
        /// An event that fires if the Splunk process terminates.
        /// </summary>
        event EventHandler<EventArgs> SplunkTerminatedEvent;
    }
}
