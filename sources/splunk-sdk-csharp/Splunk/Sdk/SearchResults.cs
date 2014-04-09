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
// [O] Contracts
// [O] Documentation

namespace Splunk.Sdk
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// The <see cref="SearchResults"/> class represents a stream of Splunk 
    /// search events.
    /// </summary>
    public sealed class SearchResults : Observable<Result>, IDisposable, IEnumerable<Result>
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
        SearchResults(Response response, bool leaveOpen)
        {
            this.response = response;

            if (leaveOpen)
            {
                this.disposed = true;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether these <see cref="SearchResults"/> 
        /// are the final results from a search job.
        /// </summary>
        public bool AreFinal
        { get { return !this.ArePreview; } }

        /// <summary>
        /// Gets a value indicating whether these <see cref="SearchResults"/> 
        /// are a preview of the results from an unfinished search job.
        /// </summary>
        public bool ArePreview
        { get; private set; }

        /// <summary>
        /// Gets the read-only list of field names that may appear in a search 
        /// event <see cref="Result"/>.
        /// </summary>
        /// <remarks>
        /// Be aware that any given result will contain a subset of these 
        /// fields.
        /// </remarks>
        public IReadOnlyList<string> FieldNames
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new <see cref="SearchResults"/> instance asyncrhonously
        /// using the specified <see cref="XmlReader"/> object and optionally 
        /// leaves the reader open.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="leaveOpen"><c>true</c> to leave the reader open after 
        /// the <see cref="SearchResults"/> object is disposed; otherwise, 
        /// <c>false</c>.</param>
        /// <returns>A <see cref="SearchResults"/> object for streaming Splunk 
        /// search event records.</returns>
        internal static async Task<SearchResults> CreateAsync(Response response, bool leaveOpen)
        {
            response.XmlReader.MoveToElement(); // Ensures we're at an element, not an attribute

            if (!response.XmlReader.IsStartElement("results"))
            {
                if (!await response.XmlReader.ReadToFollowingAsync("results"))
                {
                    throw new InvalidDataException();  // TODO: diagnostics
                }
            }

            var isPreview = XmlConvert.ToBoolean(response.XmlReader["preview"]);
            var fieldNames = new List<string>();

            if (await response.XmlReader.ReadToDescendantAsync("meta"))
            {
                if (await response.XmlReader.ReadToDescendantAsync("fieldOrder"))
                {
                    await response.XmlReader.ReadEachDescendantAsync("field", async () => fieldNames.Add(await response.XmlReader.ReadElementContentAsStringAsync()));
                    await response.XmlReader.SkipAsync();
                }
                await response.XmlReader.SkipAsync();
            }

            return new SearchResults(response, leaveOpen)
            {
                ArePreview = isPreview,
                FieldNames = fieldNames
            };
        }

        /// <summary>
        /// Releases unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.response.Dispose();
                this.disposed = true;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through search result <see 
        /// cref="Result"/> objects synchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="Result"/> enumerator structure for the <see 
        /// cref="SearchResults"/>.
        /// </returns>
        /// <remarks>
        /// You can use the <see cref="GetEnumerator"/> method to
        /// <list type="bullet">
        /// <item><description>
        ///     Perform LINQ to Objects queries to obtain a filtered set of 
        ///     search result records.</description></item>
        /// <item><description>
        ///     Append search results to an existing <see cref="Result"/>
        ///     collection.</description></item>
        /// </list>
        /// </remarks>
        public IEnumerator<Result> GetEnumerator()
        {
            if (this.enumerated)
            {
                throw new InvalidOperationException(); // TODO: diagnostics
            }

            this.enumerated = true;

            for (var task = this.ReadResultAsync(); task.Result != null; task = this.ReadResultAsync())
            {
                yield return task.Result;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        /// <summary>
        /// Pushes <see cref="Result"/> objects to observers and then completes.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing this asychronous operation.
        /// </returns>
        override protected async Task PushObservations()
        {
            if (this.enumerated)
            {
                throw new InvalidOperationException(); // TODO: diagnostics
            }

            this.enumerated = true;

            for (Result record; (record = await this.ReadResultAsync()) != null; )
            { 
                this.OnNext(record); 
            }

            this.OnCompleted();
        }

        #endregion

        #region Privates/internals

        readonly Response response;
        bool disposed;
        bool enumerated;

        /// <summary>
        /// Reads the next <see cref="Result"/> in the <see cref=
        /// "SearchResults"/> stream.
        /// </summary>
        /// <returns>
        /// A <see cref="Result"/> if the next result was read successfully; 
        /// <code>null</code> if there are no more records to read.
        /// </returns>
        async Task<Result> ReadResultAsync()
        {
            if (!await this.response.XmlReader.ReadToNextSiblingAsync("result"))
            {
                return null;
            }

            var result = new Result();

            await this.response.XmlReader.ReadEachDescendantAsync("field", async () =>
            {
                var key = this.response.XmlReader["k"];

                if (key == null)
                {
                    throw new XmlException("'field' attribute 'k' not found");
                }

                var fieldDepth = this.response.XmlReader.Depth;
                var values = new List<string>();

                while (await this.response.XmlReader.ReadAsync())
                {
                    if (this.response.XmlReader.Depth == fieldDepth)
                    {
                        break;
                    }

                    Debug.Assert(this.response.XmlReader.Depth > fieldDepth, "The loop should have exited earlier.");

                    if (this.response.XmlReader.IsStartElement("value"))
                    {
                        if (await this.response.XmlReader.ReadToDescendantAsync("text"))
                        {
                            values.Add(await this.response.XmlReader.ReadElementContentAsStringAsync());
                        }
                    }
                    else if (this.response.XmlReader.IsStartElement("v"))
                    {
                        string value = await this.response.XmlReader.ReadOuterXmlAsync();
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
