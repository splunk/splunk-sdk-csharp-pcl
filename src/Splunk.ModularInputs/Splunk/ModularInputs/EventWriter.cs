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
    public class EventWriter : IDisposable
    {
        #region Private fields
        private static XmlSerializer serializer = new XmlSerializer(typeof(Event));
        private BlockingCollection<Event> eventQueue = new BlockingCollection<Event>();
        private XmlWriter writer;
        private Task eventQueueMonitor = null;
        private object synchronizationObject = new object();
        private bool disposed;
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

            writer = XmlWriter.Create(stdout, new XmlWriterSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Document
            });
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
            if (disposed)
                throw new ObjectDisposedException("EventWriter already disposed.");
            if (eventQueueMonitor == null)
                // Don't start the eventQueueMonitor up until we actually want to write
                // events so we don't get empty <stream/> elements in cases where
                // we don't actually queue anything.
                eventQueueMonitor = Task.Run(() => WriteEventsFromQueue());
            await Task.Run(() => eventQueue.Add(e));
        }
    
        public async void Dispose()
        {
            await Dispose(true);
        }

        protected virtual async Task Dispose(bool disposing)
        {
            if (disposed) return;

            if (eventQueue.IsAddingCompleted)
                return;
            eventQueue.CompleteAdding();
            if (eventQueueMonitor != null)
                await eventQueueMonitor;
            writer.Close();
            this.Progress.Report(new EventWrittenProgressReport
            {
                Timestamp = DateTime.Now,
                WrittenEvent = new Event { }
            });

            disposed = true;
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
            if (disposed) throw new ObjectDisposedException("EventWriter is already disposed.");
            await this.Stderr.WriteAsync(severity + " " + message + this.Stderr.NewLine);
        }

        protected void WriteEventsFromQueue()
        {
            writer.WriteStartDocument();
            writer.WriteStartElement(prefix: null, localName: "stream", ns: null);

            foreach (Event eventToWrite in eventQueue.GetConsumingEnumerable())
            {
                serializer.Serialize(writer, eventToWrite);
                writer.Flush();
                this.Progress.Report(new EventWrittenProgressReport {
                        Timestamp = DateTime.Now,
                        WrittenEvent = eventToWrite
                });

            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
        }
    }
}
