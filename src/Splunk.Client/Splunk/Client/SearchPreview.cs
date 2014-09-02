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
//// [ ] Read messages and represent them in this class (search for "//// Skip messages")

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
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
        /// <value>
        /// <c>true</c> if this object is final, <c>false</c> if not.
        /// </value>
        public bool IsFinal
        {
            get { return this.metadata.IsFinal; }
        }

        /// <summary>
        /// Gets the read-only list of field names that may appear in a
        /// <see cref="SearchResult"/>.
        /// </summary>
        /// <remarks>
        /// Be aware that any given result will contain a subset of these fields.
        /// </remarks>
        /// <value>
        /// A list of names of the fields.
        /// </value>
        public ReadOnlyCollection<string> FieldNames
        {
            get { return this.metadata.FieldNames; }
        }

        /// <summary>
        /// Gets the read-only list of field names that may appear in a search event
        /// <see cref="SearchResult"/>.
        /// </summary>
        /// <remarks>
        /// Be aware that any given result will contain a subset of these fields.
        /// </remarks>
        /// <value>
        /// The results.
        /// </value>
        public ReadOnlyCollection<SearchResult> Results
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
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public async Task ReadXmlAsync(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null);

            //// Intitialize data members
            
            this.metadata = new SearchResultMetadata();
            await metadata.ReadXmlAsync(reader).ConfigureAwait(false);

            var results = new List<SearchResult>();
            this.Results = new ReadOnlyCollection<SearchResult>(results);

            //// Read the search preview

            while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "results"))
            {
                var result = new SearchResult(this.metadata);

                await result.ReadXmlAsync(reader).ConfigureAwait(false);
                results.Add(result);
                await reader.ReadAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets a string representation of the current instance.
        /// </summary>
        /// <returns>
        /// A string instance representing the current instance.
        /// </returns>
        /// <seealso cref="M:System.Object.ToString()"/>
        public override string ToString()
        {
            return base.ToString();
        }

        #endregion

        #region Privates/internals

        SearchResultMetadata metadata;

        #endregion
    }
}
