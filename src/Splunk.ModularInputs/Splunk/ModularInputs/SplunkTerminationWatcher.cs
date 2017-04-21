using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Splunk.ModularInputs;

namespace Splunk.ModularInputs
{
    /// <summary>
    /// This class watches for events realted to Splunk shutting down or
    /// terminating. Hooking these events allows a modular input to gracefully
    /// shutdown (stop sending events to Splunk and save checkpoint data).
    /// </summary>
    public class SplunkTerminationWatcher : ISplunkTerminationWatcher, IDisposable
    {
        private bool eventFired = false;
        private EventWriter writer;
        private ServiceController splunkService;

        public SplunkTerminationWatcher(EventWriter eventWriter)
        {
            this.writer = eventWriter;

            ShutdownRequested = false;
            SplunkTerminated = false;
            Console.CancelKeyPress += TrapControlSignals;

            uint parentProcessId = GetParentProcessId();

            if (parentProcessId > 0)
            {
                Process parentProcess = Process.GetProcessById((int)parentProcessId);
                parentProcess.EnableRaisingEvents = true;
                parentProcess.Exited += parentProcess_Exited;
            }

            splunkService = GetServiceByProcessId(parentProcessId);

            if (splunkService != null)
            {
                // Watch the Splunk service, waiting for it to enter a stopping or stopped state.
                // While this isn't a true fix for the Splunks issues with graceful shutdown of modular
                // inputs, watching the Splunk service state gives us notification of a shutdown about
                // 1-2 seconds before the ctrl+break signal is received.
                Task.Run(() => WatchServiceStateChange(splunkService, ServiceControllerStatus.StopPending));
                Task.Run(() => WatchServiceStateChange(splunkService, ServiceControllerStatus.Stopped));
            }
        }

        /// <summary>
        /// Handle trapping of ctrl+break and ctrl+c signals.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrapControlSignals(object sender, ConsoleCancelEventArgs e)
        {
            if (e.SpecialKey == ConsoleSpecialKey.ControlBreak || e.SpecialKey == ConsoleSpecialKey.ControlC)
            {
                if (writer != null)
                    writer.LogAsync(Severity.Info, "Splunk signaled shutdown via control signal.").Wait();

                FireShutdownEvent();
                e.Cancel = true;
                return;
            }
            e.Cancel = false;
        }

        /// <summary>
        /// Watch for state change of a windows service.
        /// </summary>
        /// <param name="parentProcessId"></param>
        private void WatchServiceStateChange(ServiceController service, ServiceControllerStatus stateToWatch)
        {
            service.WaitForStatus(stateToWatch);
            if (writer != null)
                writer.LogAsync(Severity.Info, string.Format("Splunk windows service {0} state detected.", Enum.GetName(typeof(ServiceControllerStatus), stateToWatch))).Wait();
            FireShutdownEvent();
        }

        private ServiceController GetServiceByProcessId(uint processId)
        {
            SelectQuery query = new SelectQuery("Win32_Service", string.Format("ProcessId={0}", processId), new string[] { "Name" });
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(new ManagementScope("root\\CIMV2"), query);
            System.Management.ManagementObjectCollection.ManagementObjectEnumerator enumerator = searcher.Get().GetEnumerator();
            if (enumerator.MoveNext() == false)
            {
                //Couldn't find a service with the given process ID
                return null;
            }
            string serviceName = (string)enumerator.Current["Name"];

            return new ServiceController(serviceName);
        }

        private uint GetParentProcessId()
        {
            int myProcessId = Process.GetCurrentProcess().Id;
            SelectQuery query = new SelectQuery("Win32_Process", string.Format("ProcessId={0}", myProcessId), new string[] { "ParentProcessId" });
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(new ManagementScope("root\\CIMV2"), query);
            System.Management.ManagementObjectCollection.ManagementObjectEnumerator enumerator = searcher.Get().GetEnumerator();
            if (enumerator.MoveNext() == false)
            {
                //Couldn't hook the parent process. There may not be one.
                return 0;
            }
            return (uint)enumerator.Current["ParentProcessId"];
        }

        void parentProcess_Exited(object sender, EventArgs e)
        {
            if (writer != null)
                writer.LogAsync(Severity.Info, "Splunk process termination detected.").Wait();
            SplunkTerminated = true;
            if (SplunkTerminatedEvent != null)
            {
                foreach (EventHandler<EventArgs> handler in SplunkTerminatedEvent.GetInvocationList())
                {
                    handler.BeginInvoke(this, new EventArgs(), null, null);
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void FireShutdownEvent(){
            if (eventFired == false)
            {
                ShutdownRequested = true;
                if (ShutdownRequestedEvent != null)
                {
                    foreach (EventHandler<EventArgs> handler in ShutdownRequestedEvent.GetInvocationList())
                    {
                        handler.BeginInvoke(this, new EventArgs(), null, null);
                    }
                }
                eventFired = true;
            }
        }

        public bool ShutdownRequested { get; private set; }
        public bool SplunkTerminated { get; private set; }
        public event EventHandler<EventArgs> ShutdownRequestedEvent;
        public event EventHandler<EventArgs> SplunkTerminatedEvent;

        public void Dispose()
        {
            if(splunkService != null)
                splunkService.Dispose();
        }
    }
}
