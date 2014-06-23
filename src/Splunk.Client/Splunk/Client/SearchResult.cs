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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    
    /// <summary>
    /// Represents a single record on a <see cref="SearchResultStream"/>.
    /// </summary>
    public class SearchResult : ExpandoAdapter
    {
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

        /// <summary>
        /// Gets the XML markup for the <c>_raw</c> field value.
        /// </summary>
        /// <remarks>
        /// The return value is different than that of the <c>_raw</c> field 
        /// value in that this segmented raw value is an XML fragment that 
        /// includes all markup such as XML tags and escaped characters. For 
        /// example, <c>record["_raw"]</c> field value returns this:
        /// </remarks>
        public string SegmentedRaw { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously reads data into the current <see cref="SearchResult"/>.
        /// </summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/> from which to read.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public async Task ReadXmlAsync(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");

            reader.MoveToElement();
            reader.EnsureMarkup(XmlNodeType.Element, "result");

            this.Object = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)this.Object;

            await reader.ReadEachDescendantAsync("field", async (r) =>
            {
                var key = r.GetRequiredAttribute("k");
                var fieldDepth = r.Depth;
                var values = new List<string>();

                while (await r.ReadAsync())
                {
                    if (r.Depth == fieldDepth)
                    {
                        break;
                    }

                    Debug.Assert(r.Depth > fieldDepth, "This loop should have exited earlier.");
                    r.EnsureMarkup(XmlNodeType.Element, "value", "v");

                    if (r.Name == "value")
                    {
                        if (await r.ReadToDescendantAsync("text"))
                        {
                            values.Add(await r.ReadElementContentAsStringAsync());
                        }
                    }
                    else if (r.Name == "v")
                    {
                        string value = await r.ReadOuterXmlAsync();
                        this.SegmentedRaw = value;
                        values.Add(value);
                    }
                }

                switch (values.Count)
                {
                    case 0: 
                        dictionary.Add(key, null);
                        break;
                    case 1:
                        dictionary.Add(key, values[0]);
                        break;
                    default:
                        dictionary.Add(key, new ReadOnlyCollection<string>(values));
                        break;
                }
            });
        }

        /// <summary>
        /// Gets a string representation of the current instance.
        /// </summary>
        /// <returns>
        /// A string instance representing the current instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder("Result(");

            foreach (KeyValuePair<string, object> pair in (IDictionary<string, object>)this.Object)
            {
                builder.Append(pair.Key);
                builder.Append(": ");
                builder.Append(pair.Value);
                builder.Append(", ");
            }

            builder.Length = builder.Length - 2;
            builder.Append(")");

            return builder.ToString();
        }

        #endregion

        #region Privates/internals

        readonly SearchResultMetadata metadata;

        internal SearchResult(SearchResultMetadata metadata)
        {
            this.metadata = metadata;
        }

        internal SearchResultMetadata Metadata
        {
            get { return this.metadata; }
        }

        #endregion
    }
}
