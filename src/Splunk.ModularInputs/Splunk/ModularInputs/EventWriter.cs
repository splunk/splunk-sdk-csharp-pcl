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
        public readonly TextWriter Stdout;
        public readonly TextWriter Stderr;
        public readonly IProgress<EventWrittenProgressReport> Progress;
        #endregion


        #region Constructors

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

        /// <param name="eventElement">
        public async Task QueueEventForWriting(Event e)
        {
            if (disposed)
                throw new ObjectDisposedException("EventWriter already disposed.");
            lock (synchronizationObject)
            {
                if (eventQueueMonitor == null)
                    eventQueueMonitor = Task.Run(() => WriteEventsFromQueue());
            }
            await Task.Run(() => eventQueue.Add(e));
        }
    
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (synchronizationObject)
            {
                if (disposed) return;

                if (eventQueue.IsAddingCompleted)
                    return;
                eventQueue.CompleteAdding();
                if (eventQueueMonitor != null)
                    eventQueueMonitor.Wait();
                writer.Close();
                this.Progress.Report(new EventWrittenProgressReport
                {
                    Timestamp = DateTime.Now,
                    WrittenEvent = new Event { }
                });

                disposed = true;
            }
        }


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
