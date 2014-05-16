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
//// [O] Documentation
//// [ ] Read messages and represent them in this class (search for "//// Skip messages")


namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    
    /// <summary>
    /// Represents a search result preview on a <see cref="SearchPreviewStream"/>.
    /// </summary>
    public class SearchPreview
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

        /// <summary>
        /// Gets the read-only list of field names that may appear in a search 
        /// event <see cref="SearchResult"/>.
        /// </summary>
        /// <remarks>
        /// Be aware that any given result will contain a subset of these 
        /// fields.
        /// </remarks>
        public IReadOnlyList<SearchResult> SearchResults
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously reads data into the current <see cref="SearchPreview"/>.
        /// </summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/> from which to read.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing this operation.
        /// </returns>
        public async Task ReadXmlAsync(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null);

            //// Intitialize data members

            var fieldNames = new List<string>();
            this.FieldNames = fieldNames;
            
            var searchResults = new List<SearchResult>();
            this.SearchResults = searchResults;

            this.IsFinal = true;

            //// Read the search preview

            if (!await reader.MoveToDocumentElementAsync("results"))
            {
                return; // an empty final result
            }

            string preview = reader["preview"];

            if (preview == null)
            {
                throw new InvalidDataException(string.Format("Expected preview attribute on <results>"));
            }

            this.IsFinal = !BooleanConverter.Instance.Convert(preview);

            await reader.ReadElementSequenceAsync("meta", "fieldOrder");

            await reader.ReadEachDescendantAsync("field", async () =>
            {
                await reader.ReadAsync();
                var fieldName = await reader.ReadContentAsStringAsync();
                fieldNames.Add(fieldName);
            });

            await reader.ReadEndElementSequenceAsync("fieldOrder", "meta");

            if (reader.NodeType == XmlNodeType.Element && reader.Name == "messages")
            {
                //// Skip messages

                await reader.ReadEachDescendantAsync("msg", () =>
                { 
                    return Task.FromResult(true);
                });

                if (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "messages"))
                {
                    throw new InvalidDataException(string.Format("Expected </messages>, not {0}", reader.ToString()));
                }

                await reader.ReadAsync();
            }

            while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "results"))
            {
                var searchResult = new SearchResult();
                
                await searchResult.ReadXmlAsync(reader);
                searchResults.Add(searchResult);
                await reader.ReadAsync();
            }
        }

        /// <summary>
        /// Gets a string representation of the current instance.
        /// </summary>
        /// <returns>
        /// A string instance representing the current instance.
        /// </returns>
        public override string ToString()
        {
            return base.ToString();
        }

        #endregion
    }
}
