/*
 * Copyright 2014 Splunk, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"): you may
 * not use this file except in compliance with the License. You may obtain
 * a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 */

namespace Splunk.ModularInputs
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Structure emitted on an IProgress instance by EventWriter when it has finished
    /// writing an event, or when it has terminated, in which case the event has no properties
    /// set.
    /// </summary>
    public struct EventWrittenProgressReport
    {
        public DateTime Timestamp { get; set; }
        public Event WrittenEvent { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EventWriter
    {
        #region Private fields
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(Event), new XmlRootAttribute("event"));
        private BlockingCollection<Event> eventQueue = new BlockingCollection<Event>();
        private Task eventQueueMonitor = null;
        private object synchronizationObject = new object();
        private bool completed;
        #endregion

        #region Properties
        protected readonly TextWriter Stdout;
        protected readonly TextWriter Stderr;
        protected readonly IProgress<EventWrittenProgressReport> Progress;
        #endregion


        #region Constructors

        /// <summary>
        /// Create a new EventWriter instance writing to the provided streams
        /// and emitting progress reports to the provided IProgress instance
        /// after each event is written.
        /// </summary>
        /// <param name="stdout">A writer on which to write events (usually 
        /// Console.Stdout in production, or a StringWriter in testing).</param>
        /// <param name="stderr">A writer on which to log errors and messages
        /// (usually Console.Stderr in production, or a StringWriter in testing).</param>
        /// <param name="progress">An IProgress instance to write progress reports to.
        /// This is the easiest way to trigger behavior after events are actually
        /// written to Splunk. In production, it will usually be an instance
        /// of Progress&lt;EventWrittenProgressReport&gt;</param>
        public EventWriter(TextWriter stdout, TextWriter stderr,
            IProgress<EventWrittenProgressReport> progress)
        {
            this.Stdout = stdout;
            this.Stderr = stderr;
            this.Progress = progress;
        }

        #endregion

        /// <summary>
        /// Add an event to this EventWriter's shared queue to be written as soon
        /// as possible. This method is thread-safe, so multiple threads can 
        /// concurrently queue events without interfering with each other.
        /// </summary>
        /// <param name="e">The Event to write.</param>
        public async Task QueueEventForWriting(Event e)
        {
            if (completed)
                throw new ObjectDisposedException("EventWriter already disposed.");
            if (eventQueueMonitor == null)
                // Don't start the eventQueueMonitor up until we actually want to write
                // events so we don't get empty <stream/> elements in cases where
                // we don't actually queue anything.
                eventQueueMonitor = Task.Run(() => WriteEventsFromQueue());
            await Task.Run(() => eventQueue.Add(e));
        }

        public async Task CompleteAsync()
        {
            if (completed) return;

            if (eventQueue.IsAddingCompleted)
                return;
            eventQueue.CompleteAdding();
            if (eventQueueMonitor != null)
                await eventQueueMonitor;
            await Stdout.FlushAsync();
            this.Progress.Report(new EventWrittenProgressReport
            {
                Timestamp = DateTime.Now,
                WrittenEvent = new Event { }
            });

            completed = true;
        }

        /// <summary>
        /// Log a message to stderr.
        /// </summary>
        /// <param name="severity">A string specifying the
        /// error level (usually one of "INFO", "DEBUG", "WARNING",
        /// "ERROR", or "FATAL").</param>
        /// <param name="message">The message to log.</param>
        public async Task LogAsync(string severity, string message)
        {
            if (completed) throw new ObjectDisposedException("EventWriter is already disposed.");
            await this.Stderr.WriteAsync(severity + " " + message + this.Stderr.NewLine);
        }

        /// <summary>
        /// Log a message to stderr.
        /// </summary>
        /// <param name="severity">Specifies the error level</param>
        /// <param name="message">The message to log.</param>
        public async Task LogAsync(Severity severity, string message)
        {
            var name = Enum.GetName(typeof(Severity), severity).ToUpper();
            await LogAsync(name, message);
        }

        protected void WriteEventsFromQueue()
        {
            // The original version of this code used the XmlWriter going directly to Stdout, and an XmlSerializer on Event
            // to generate the XML. There are a number of subtle bugs around Splunk's interaction with subprocesses on Windows
            // that required a three chicken exorcism to arrive at this version.
            //
            // The observable problem was that the modular input would run, but no events would show up in Splunk.
            // I wrote the simplest possible modular input, that had static strings defining the scheme and events to stream,
            // and showed that it worked, provided you flushed Console.Out after writing. Then I put the XmlWriter in to produce
            // XML instead of the static strings, and found that no events showed up if the XmlWriter was writing directly
            // to Console.Out. The events would show up if you ran the script from command.exe, though, so it's something about
            // how Splunk is opening subprocesses. If I put a StringWriter in between, with the XmlWriter writing to the StringWriter,
            // and then writing the string from the StringWriter to Console.Out, that works. So I use that workaround here for each event.
            //
            // Next, calling Serialize on an XmlSerializer for the Event objects hung. I demonstrated that by putting in LogAsync calls
            // between each event, and showing that the log entry to splunkd.log right before the Serialize call shows up,
            // but the one after never does. So I ripped that out, and had Event write itself to an XmlWriter directly with a ToXml
            // method.
            Stdout.WriteLine("<stream>");

            foreach (Event eventToWrite in eventQueue.GetConsumingEnumerable())
            {
                var buffer = new StringWriter();
                using (var writer = XmlWriter.Create(buffer, new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment }))
                {
                    eventToWrite.ToXml(writer);
                }
                Stdout.WriteLine(buffer.ToString());
                Stdout.Flush();
                this.Progress.Report(new EventWrittenProgressReport
                {
                    Timestamp = DateTime.Now,
                    WrittenEvent = eventToWrite
                });
            }

            Stdout.WriteLine("</stream>");
            Stdout.Flush();
            this.Progress.Report(new EventWrittenProgressReport
            {
                Timestamp = DateTime.Now
            });

        }
    }
}
