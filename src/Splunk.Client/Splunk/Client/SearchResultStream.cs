﻿/*
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
        /// <param name="leaveOpen">
        /// Indicates whether the reader should be left open following <see
        /// cref="SearchResultStream.Dispose"/>.
        /// </param>
        SearchResultStream(Response response, bool leaveOpen)
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
        /// Gets a value indicating whether these <see cref="SearchResultStream"/> 
        /// are the final results from a search job.
        /// </summary>
        public bool AreFinal
        { get { return !this.ArePreview; } }

        /// <summary>
        /// Gets a value indicating whether these <see cref="SearchResultStream"/> 
        /// are a preview of the results from an unfinished search job.
        /// </summary>
        public bool ArePreview
        { get; private set; }

        /// <summary>
        /// Gets the read-only list of field names that may appear in a search 
        /// event <see cref="SearchResult"/>.
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
        /// Asynchronously creates a new <see cref="SearchResultStream"/> 
        /// instance using the specified <see cref="Response"/>.
        /// </summary>
        /// <param name="response">
        /// An object from which the search results are read. 
        /// </param>
        /// <param name="leaveOpen">
        /// <c>true</c> to leave the <paramref name="response"/> object open after 
        /// the <see cref="SearchResultStream"/> object is disposed; otherwise, 
        /// <c>false</c>.
        /// </param>
        /// <returns>
        /// A <see cref="SearchResultStream"/> object.
        /// </returns>
        internal static async Task<SearchResultStream> CreateAsync(Response response, bool leaveOpen)
        {
            var fieldNames = new List<string>();
            var arePreview = false;

            var reader = response.XmlReader;

            if (reader.ReadState == ReadState.Initial)
            {
                await reader.ReadAsync();

                if (reader.NodeType == XmlNodeType.XmlDeclaration)
                {
                    try
                    {
                        await reader.ReadAsync();
                    }
                    catch (XmlException)
                    {
                        //// When nothing follows the XmlDeclaration the reader
                        //// fails to detect EOF on the response stream, does
                        //// not update the current XmlNode, and then throws an 
                        //// XmlException because it thinks the XmlNode appears
                        //// on a line other than the first. We catch that here.

                        reader.Dispose();

                        return new SearchResultStream(response, leaveOpen)
                        {
                            ArePreview = false,
                            FieldNames = fieldNames
                        };
                    }
                }
            }
            else
            {
                reader.MoveToElement(); // Ensures we're at an element, not an attribute
            }

            if (!reader.IsStartElement("results"))
            {
                if (!await reader.ReadToFollowingAsync("results"))
                {
                    throw new InvalidDataException(); // TODO: diagnostics
                }
            }

            arePreview = XmlConvert.ToBoolean(reader["preview"]);

            foreach (var name in new string[] { "meta", "fieldOrder" })
            {
                await reader.ReadAsync();

                if (!(reader.NodeType == XmlNodeType.Element && reader.Name == name))
                {
                    throw new InvalidDataException(); // TODO: Diagnostics
                }
            }

            await reader.ReadEachDescendantAsync("field", async () => 
            {
                await reader.ReadAsync();
                var fieldName = await reader.ReadContentAsStringAsync();
                fieldNames.Add(fieldName);
            });

            if (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "fieldOrder"))
            {
                throw new InvalidDataException(); // TODO: Diagnostics
            }

            await reader.ReadAsync();

            if (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "meta"))
            {
                throw new InvalidDataException(); // TODO: Diagnostics
            }

            await reader.ReadAsync();

            if (reader.NodeType == XmlNodeType.Element && reader.Name == "messages")
            {
                reader.ReadEachDescendantAsync("msg", () => { return Task.FromResult(true); }).Wait();

                if (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "messages"))
                {
                    throw new InvalidDataException(); // TODO: Diagnostics
                }
            }

            return new SearchResultStream(response, leaveOpen)
            {
                ArePreview = arePreview,
                FieldNames = fieldNames
            };
        }

        /// <summary>
        /// Releases all disposable resources used by the current <see cref=
        /// "SearchResultStream"/>.
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
        /// cref="SearchResult"/> objects synchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="SearchResult"/> enumerator structure for the <see 
        /// cref="SearchResultStream"/>.
        /// </returns>
        /// <remarks>
        /// You can use the <see cref="GetEnumerator"/> method to
        /// <list type="bullet">
        /// <item><description>
        ///     Perform LINQ to Objects queries to obtain a filtered set of 
        ///     search result records.</description></item>
        /// <item><description>
        ///     Append search results to an existing <see cref="SearchResult"/>
        ///     collection.</description></item>
        /// </list>
        /// </remarks>
        public IEnumerator<SearchResult> GetEnumerator()
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
        /// Pushes <see cref="SearchResult"/> objects to observers and then completes.
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

            for (SearchResult record; (record = await this.ReadResultAsync()) != null; )
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
        /// Reads the next <see cref="SearchResult"/> in the current <see cref=
        /// "SearchResultStream"/> stream.
        /// </summary>
        /// <returns>
        /// A <see cref="SearchResult"/> if the next result was read successfully; 
        /// <c>null</c> if there are no more records to read.
        /// </returns>
        async Task<SearchResult> ReadResultAsync()
        {
            var reader = this.response.XmlReader;

            switch (reader.ReadState)
            {
                case ReadState.Closed:
                case ReadState.EndOfFile:
                    return null;

                case ReadState.Interactive:
                    break;

                default:
                    throw new InvalidDataException(); // TODO: Diagnostics
            }

            reader.MoveToElement();

            if (!(reader.NodeType == XmlNodeType.Element && reader.Name == "result"))
            {
                await reader.ReadAsync();

                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "results")
                {
                    return null;
                }

                if (!(reader.NodeType == XmlNodeType.Element && reader.Name == "result"))
                {
                    throw new InvalidDataException(); // TODO: Diagnostics
                }
            }

            var result = new SearchResult();
            await result.ReadXmlAsync(reader);

            return result;
        }

        #endregion
    }
}
