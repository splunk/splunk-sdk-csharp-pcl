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

//// TODO:
//// [O] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Represents an enumerable, observable stream of <see cref="SearchResult"/> 
    /// records.
    /// </summary>
    public sealed class SearchResultStream : Observable<SearchResult>, IDisposable, IEnumerable<SearchResult>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResultStream"/> class.
        /// </summary>
        /// <param name="response">
        /// The object for reading search results.
        /// </param>
        SearchResultStream(Response response)
        {
            this.response = response;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the current <see cref="SearchResultStream"/> 
        /// is yielding the final results from a search job.
        /// </summary>
        public bool IsFinal
        {
            get { return this.metadata.IsFinal; }
        }

        /// <summary>
        /// Gets the read-only list of field names that may appear in a 
        /// <see cref="SearchResult"/>.
        /// </summary>
        /// <remarks>
        /// Be aware that any given result may contain a subset of these fields.
        /// </remarks>
        public IReadOnlyList<string> FieldNames
        {
            get { return this.metadata.FieldNames; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates a new <see cref="SearchResultStream"/> 
        /// instance using the specified <see cref="Response"/>.
        /// </summary>
        /// <param name="response">
        /// An object from which the search results are read. 
        /// </param>
        /// <returns>
        /// A <see cref="SearchResultStream"/> object.
        /// </returns>
        internal static async Task<SearchResultStream> CreateAsync(Response response)
        {
            var stream = new SearchResultStream(response);
            await stream.ReadMetadataAsync();
            return stream;
        }

        /// <summary>
        /// Releases all disposable resources used by the current <see cref="SearchResultStream"/>.
        /// </summary>
        public void Dispose()
        {
            this.response.Dispose();
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
        public IEnumerator<SearchResult> GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.enumerated, 1, 0) != 0)
            {
                throw new InvalidOperationException("Stream has been enumerated; The enumeration operation may not execute again.");
            }

            XmlReader reader = this.response.XmlReader;

            while (reader.ReadState <= ReadState.Interactive)
            {
                var result = this.ReadResultAsync().Result;
                yield return result;
            }

            this.enumerated = 2;
        }

        /// <inheritdoc cref="GetEnumerator">
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
 
        /// <summary>
        /// Asynchronously pushes <see cref="SearchResult"/> objects to observers 
        /// and then completes.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        protected override async Task PushObservations()
        {
            if (Interlocked.CompareExchange(ref this.enumerated, 1, 0) != 0)
            {
                throw new InvalidOperationException("Stream has been enumerated; The push operation may not execute again.");
            }

            XmlReader reader = this.response.XmlReader;

            while (reader.ReadState <= ReadState.Interactive)
            {
                this.OnNext(await this.ReadResultAsync());
            }

            this.OnCompleted();
            this.enumerated = 2;
        }

        #endregion

        #region Privates/internals

        readonly Response response;
        int enumerated;
        Metadata metadata;

        /// <summary>
        /// Asynchronously reads the metadata for the current chunk of search results.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing this asychronous operation.
        /// </returns>
        async Task ReadMetadataAsync()
        {
            var metadata = new Metadata();

            await metadata.ReadXmlAsync(this.response.XmlReader);
            this.metadata = metadata;
        }

        /// <summary>
        /// Reads the next <see cref="SearchResult"/> in the current <see cref=
        /// "SearchResultStream"/> stream.
        /// </summary>
        /// <returns>
        /// A <see cref="Task<SearchResult>"/> if the next result was read 
        /// successfully; <c>null</c> if there are no more records to read.
        /// </returns>
        async Task<SearchResult> ReadResultAsync()
        {
            var reader = this.response.XmlReader;
            reader.MoveToElement();

            while (reader.ReadState == ReadState.Interactive)
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "results")
                {
                    await this.ReadMetadataAsync();
                }

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "result")
                {
                    var result = new SearchResult();

                    await result.ReadXmlAsync(reader);
                    await reader.ReadAsync();

                    return result;
                }

                reader.EnsureMarkup(XmlNodeType.EndElement, "results");
                await reader.ReadAsync();
            }

            return null;
        }

        #endregion

        #region Types

        class Metadata
        {
            #region Properties

            /// <summary>
            /// Gets a value indicating whether this <see cref="SearchPreview"/> 
            /// contains the final results from a search job.
            /// </summary>
            public bool IsFinal
            { get; private set; }

            /// <summary>
            /// Gets the read-only list of field names that may appear in a 
            /// <see cref="SearchResult"/>.
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
            /// Asynchronously reads data into the current <see cref="SearchResultStream"/>.
            /// </summary>
            /// <param name="reader">
            /// The <see cref="XmlReader"/> from which to read.
            /// </param>
            /// <returns>
            /// A <see cref="Task"/> representing the operation.
            /// </returns>
            public async Task ReadXmlAsync(XmlReader reader)
            {
                var fieldNames = new List<string>();

                this.FieldNames = fieldNames;
                this.IsFinal = true;

                if (!await reader.MoveToDocumentElementAsync("results"))
                {
                    return;
                }

                string preview = reader.GetRequiredAttribute("preview");
                this.IsFinal = !BooleanConverter.Instance.Convert(preview);

                if (!await reader.ReadAsync())
                {
                    return;
                }

                reader.EnsureMarkup(XmlNodeType.Element, "meta");
                await reader.ReadAsync();
                reader.EnsureMarkup(XmlNodeType.Element, "fieldOrder");

                await reader.ReadEachDescendantAsync("field", async (r) =>
                {
                    await r.ReadAsync();
                    var fieldName = await r.ReadContentAsStringAsync();
                    fieldNames.Add(fieldName);
                });

                await reader.ReadEndElementSequenceAsync("fieldOrder", "meta");

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "messages")
                {
                    //// Skip messages

                    await reader.ReadEachDescendantAsync("msg", (r) =>
                    {
                        return Task.FromResult(true);
                    });

                    reader.EnsureMarkup(XmlNodeType.EndElement, "messages");
                    await reader.ReadAsync();
                }
            }

            #endregion
        }

        #endregion
    }
}
