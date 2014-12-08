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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// A search result metadata.
    /// </summary>
    sealed class SearchResultMetadata
    {
        #region Fields

        /// <summary>
        /// The missing.
        /// </summary>
        public static readonly SearchResultMetadata Missing = new SearchResultMetadata()
        {
            FieldNames = new ReadOnlyCollection<string>(new string[0]),
        };

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this <see cref="SearchPreview"/>
        /// contains the final results from a search job.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is final, <c>false</c> if not.
        /// </value>
        public bool IsFinal
        { get; private set; }

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
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously reads data into the current
        /// <see cref="SearchResultStream"/>.
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

            this.FieldNames = new ReadOnlyCollection<string>(fieldNames);
            this.IsFinal = true;

            if (!await reader.MoveToDocumentElementAsync("results").ConfigureAwait(false))
            {
                return;
            }

            string preview = reader.GetRequiredAttribute("preview");
            this.IsFinal = !BooleanConverter.Instance.Convert(preview);

            if (!await reader.ReadAsync().ConfigureAwait(false))
            {
                return;
            }

            reader.EnsureMarkup(XmlNodeType.Element, "meta");
            await reader.ReadAsync().ConfigureAwait(false);
            reader.EnsureMarkup(XmlNodeType.Element, "fieldOrder");

            if (!reader.IsEmptyElement)
            {
                await reader.ReadEachDescendantAsync("field", async (r) =>
                {
                    await r.ReadAsync().ConfigureAwait(false);
                    var fieldName = await r.ReadContentAsStringAsync().ConfigureAwait(false);
                    fieldNames.Add(fieldName);
                }).ConfigureAwait(false);

                await reader.ReadEndElementSequenceAsync("fieldOrder", "meta").ConfigureAwait(false);
            }

            if (reader.NodeType == XmlNodeType.Element && reader.Name == "messages")
            {
                //// Skip messages

                await reader.ReadEachDescendantAsync("msg", (r) =>
                {
                    return Task.FromResult(true);
                }).ConfigureAwait(false);

                reader.EnsureMarkup(XmlNodeType.EndElement, "messages");
                await reader.ReadAsync().ConfigureAwait(false);
            }
        }

        #endregion
    }

}
