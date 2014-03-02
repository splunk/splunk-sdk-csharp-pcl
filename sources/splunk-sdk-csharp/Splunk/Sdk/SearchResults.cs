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

// TODO:
//
// [ ] At present you can iterate over SearchResults just one time using
//     ReadRecords. If you call ReadRecords more than once, it will throw
//     an InvalidOperationException.
//
//     Consider these alternatives:
//
//     1. Let ReadRecords be called more than once, but 
//        return nothing everytime after the last Record is returned.
//
//     2. Turn the SearchResults class into a lazy sequence evaluator. Record
//        instances would be read sequentially and held in memory until 
//        Dispose was called. Benefit: Repeated iteration over Record 
//        instances. What are the use cases?
//
//        * Syncrhonous alternative to having multiple Subscribers. One would
//          process a set of records many times; once per presentation of the
//          Record set.
//
//        * LINQ-based data binding scenarios. May be useful for creating one 
//          or more views over a set of results. First blush investigation says 
//          XAML works best over collections that can be based on LINQ queries.
//          One might create a set of LINQ queries that work over a one or 
//         many sets of SearchResults.
//
// [O] Contracts
// [O] Documentation

namespace Splunk.Sdk
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// The <see cref="SearchResults"/> class represents a stream of Splunk 
    /// search events.
    /// </summary>
    public sealed class SearchResults : Observable<Record>, IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResults"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="reader"/> for reading search 
        /// results.</param>
        /// <param name="leaveOpen">Indicates whether the reader should be left
        /// open following <see cref="SearchResults.Dispose"/>.
        /// </param>
        SearchResults(XmlReader reader, bool leaveOpen)
        {
            this.reader = reader;

            if (leaveOpen)
            {
                GC.SuppressFinalize(this);
                this.disposed = true;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the read-only list of field names that may appear in a search 
        /// event <see cref="Record"/>.
        /// </summary>
        /// <remarks>
        /// Be aware that any given result will contain a subset of these 
        /// fields.
        /// </remarks>
        public IReadOnlyList<string> FieldNames
        { get; private set; }

        /// <summary>
        /// Gets a value indicating whether these <see cref="SearchResults"/> 
        /// are a preview of the results from an unfinished search job.
        /// </summary>
        public bool IsPreview
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new <see cref="SearchResults"/> instance asyncrhonously
        /// using the specified <see cref="XmlReader"/> object and optionally 
        /// leaves the reader open.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="leaveOpen"><code>true</code> to leave the reader open
        /// after the <see cref="SearchResults"/> object is disposed; otherwise, 
        /// <code>false</code>.</param>
        /// <returns>A <see cref="SearchResults"/> object for streaming Splunk 
        /// search event records.</returns>
        internal static async Task<SearchResults> CreateAsync(XmlReader reader, bool leaveOpen)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");
            Contract.Requires<InvalidOperationException>(reader.NodeType == XmlNodeType.Element && reader.Name == "results");

            var isPreview = XmlConvert.ToBoolean(reader["preview"]);
            var fieldNames = new List<string>();

            if (await reader.ReadToDescendantAsync("meta"))
            {
                if (await reader.ReadToDescendantAsync("fieldOrder"))
                {
                    await reader.ReadEachDescendantAsync("field", async () => fieldNames.Add(await reader.ReadElementContentAsStringAsync()));
                    await reader.SkipAsync();
                }
                await reader.SkipAsync();
            }

            return new SearchResults(reader, leaveOpen)
            {
                IsPreview = isPreview,
                FieldNames = fieldNames
            };
        }

        /// <summary>
        /// Releases unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Returns an enumerator that iterates through search result <see 
        /// cref="Record"/> objects synchronously.
        /// </summary>
        /// <returns>
        /// An <see cref="Record"/> enumerator structure for the <see cref="SearchResults"/>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// You can use the <see cref="ReadRecords"/> method to
        ///   <list type="bullet">
        ///     <item> 
        ///       <description>Perform LINQ to Objects queries to obtain a 
        ///       filtered set of search result records.</description> 
        ///     </item>
        ///     <item> 
        ///       <description>Append search results to an existing 
        ///       <see cref="Record"/>collection.</description>
        ///     </item>
        ///   </list></para>
        /// </remarks>
        public IEnumerator<Record> ReadRecords()
        {
            if (this.enumerated)
            {
                throw new InvalidOperationException(); // TODO: diagnostics
            }

            this.enumerated = true;

            for (var task = this.ReadRecordAsync(); task.Result != null; task = this.ReadRecordAsync())
            {
                yield return task.Result;
            }
        }

        /// <summary>
        /// Iterates through <see cref="Record"/> objects asycrhonously 
        /// notifying observers as records are created.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing this asychronous operation.
        /// </returns>
        public async Task ReadRecordsAsync()
        {
            if (this.enumerated)
            {
                throw new InvalidOperationException(); // TODO: diagnostics
            }

            this.enumerated = true;

            for (var record = await this.ReadRecordAsync(); record != null; record = await this.ReadRecordAsync())
            {
                foreach (var observer in this.Observers)
                {
                    observer.OnNext(record);
                }
            }
            foreach (var observer in this.Observers)
            {
                observer.OnCompleted();
            }
        }

        #endregion

        #region Privates/internals

        readonly XmlReader reader;
        bool disposed;
        bool enumerated;

        void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.reader.Dispose();
                this.disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Reads the next <see cref="Record"/> in the <see cref=
        /// "SearchResults"/> stream.
        /// </summary>
        /// <returns>
        /// A <see cref="Record"/> if the next result was read successfully; 
        /// <code>null</code> if there are no more records to read.
        /// </returns>
        async Task<Record> ReadRecordAsync()
        {
            if (!await this.reader.ReadToNextSiblingAsync("result"))
            {
                return null;
            }

            var result = new Record();

            await this.reader.ReadEachDescendantAsync("field", async () =>
            {
                var key = this.reader["k"];

                if (key == null)
                {
                    throw new XmlException("'field' attribute 'k' not found");
                }

                var fieldDepth = this.reader.Depth;
                var values = new List<string>();

                while (await this.reader.ReadAsync())
                {
                    if (this.reader.Depth == fieldDepth)
                    {
                        break;
                    }

                    Debug.Assert(reader.Depth > fieldDepth, "The loop should have exited earlier.");

                    if (this.reader.IsStartElement("value"))
                    {
                        if (await this.reader.ReadToDescendantAsync("text"))
                        {
                            values.Add(await this.reader.ReadElementContentAsStringAsync());
                        }
                    }
                    else if (this.reader.IsStartElement("v"))
                    {
                        string value = await this.reader.ReadOuterXmlAsync();
                        result.SegmentedRaw = value;
                        values.Add(value);
                    }
                }

                result.Add(key, new Field(values.ToArray()));
            });

            return result;
        }

        #endregion
    }
}
