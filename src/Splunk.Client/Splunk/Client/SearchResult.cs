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

namespace Splunk.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Represents a single record on a <see cref="SearchResultStream"/>.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.ExpandoAdapter"/>
    public class SearchResult : ExpandoAdapter
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether the current
        /// <see cref="SearchResultStream"/>
        /// is yielding the final results from a search job.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is final, <c>false</c> if not.
        /// </value>
        public bool IsFinal
        {
            get { return this.metadata.IsFinal; }
        }

        /// <summary>
        /// Gets the read-only list of field names that may appear in a <see cref="SearchResult"/>.
        /// </summary>
        /// <remarks>
        /// Be aware that any given result may contain a subset of these fields.
        /// </remarks>
        /// <value>
        /// A list of names of the fields.
        /// </value>
        public ReadOnlyCollection<string> FieldNames
        {
            get { return this.metadata.FieldNames; }
        }

        /// <summary>
        /// Gets the XML markup for the <c>_raw</c> field value.
        /// </summary>
        /// <remarks>
        /// This value is different than that of the <c>_raw</c> field value in that
        /// it is an XML fragment that includes all markup.
        /// </remarks>
        /// <value>
        /// The segmented raw.
        /// </value>
        public XElement SegmentedRaw
        { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public dynamic Tags
        {
            get { return this.tagsObject; }
        }

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

            this.tagsObject = new ExpandoObject();
            var tagsDictionary = (IDictionary<string, object>)this.tagsObject;

            this.SegmentedRaw = null;

            await reader.ReadEachDescendantAsync("field", async (r) =>
            {
                ImmutableSortedSet<string>.Builder tags = null;

                var key = r.GetRequiredAttribute("k");
                var values = new List<object>();

                var fieldDepth = r.Depth;

                while (await r.ReadAsync().ConfigureAwait(false))
                {
                    if (r.Depth == fieldDepth)
                    {
                        break;
                    }

                    Debug.Assert(r.Depth > fieldDepth, "This loop should have exited earlier.");
                    r.EnsureMarkup(XmlNodeType.Element, "value", "v");

                    if (r.Name == "value")
                    {
                        string value = null;

                        while (await r.ReadAsync().ConfigureAwait(false))
                        {
                            if (r.NodeType == XmlNodeType.EndElement)
                            {
                                r.EnsureMarkup(XmlNodeType.EndElement, "value");
                                break;
                            }

                            r.EnsureMarkup(XmlNodeType.Element, "text", "tag");
                            var isEmptyElement = r.IsEmptyElement;
                            var elementName = r.Name;
                            string content;

                            if (isEmptyElement)
                            {
                                content = string.Empty;
                            }
                            else
                            {
                                await r.ReadAsync().ConfigureAwait(false);
                                content = r.Value;
                            }

                            if (elementName == "tag")
                            {
                                if (tags == null)
                                {
                                    tags = ImmutableSortedSet.CreateBuilder<string>();
                                }
                                tags.Add(content);
                            }
                            else
                            {
                                value = content;
                            }

                            if (!isEmptyElement)
                            {
                                await r.ReadAsync().ConfigureAwait(false);
                                r.EnsureMarkup(XmlNodeType.EndElement, elementName);
                            }
                        }

                        if (tags != null && tags.Count > 0)
                        {
                            values.Add(new TaggedFieldValue(value ?? string.Empty, tags.ToImmutable()));
                            tags.Clear();
                        }
                        else
                        {
                            values.Add(value ?? string.Empty);
                        }
                    }
                    else
                    {
                        Debug.Assert(this.SegmentedRaw == null);
                        Debug.Assert(key == "_raw");

                        string value = await r.ReadOuterXmlAsync().ConfigureAwait(false);
                        this.SegmentedRaw = XElement.Parse(value);
                        values.Add(this.SegmentedRaw.Value);
                    }
                }

                if (key.StartsWith("tag::"))
                {
                    var valueSet = ImmutableSortedSet.ToImmutableSortedSet<string>(values.Cast<string>());
                    tagsDictionary.Add(key.Substring("tag::".Length), valueSet);
                    dictionary.Add(key, valueSet);
                }
                else
                {
                    switch (values.Count)
                    {
                        case 0:
                            dictionary.Add(key, string.Empty);
                            break;
                        case 1:
                            dictionary.Add(key, values[0]);
                            break;
                        default:
                            dictionary.Add(key, new ReadOnlyCollection<object>(values));
                            break;
                    }
                }

            }).ConfigureAwait(false);
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
            var dictionary = (IDictionary<string, object>)this.Object;

            if (dictionary.Count == 0)
            {
                return "SearchResult()";
            }

            var builder = new StringBuilder("SearchResult(");

            Action<object> quote = o =>
            {
                builder.Append('"');
                builder.Append(o.ToString().Replace(@"""", @"\"""));
                builder.Append('"');
            };

            foreach (KeyValuePair<string, object> pair in (IDictionary<string, object>)this.Object)
            {
                builder.Append(pair.Key);
                builder.Append(": ");

                var value = pair.Value;

                if (value is string || value is TaggedFieldValue)
                {
                    quote(value);
                }
                else
                {
                    builder.Append('[');

                    foreach (var item in (IEnumerable)value)
                    {
                        builder.Append(item);
                        builder.Append(", ");
                    }

                    builder.Length = builder.Length - 2;
                    builder.Append(']');
                }

                builder.Append(", ");
            }

            builder.Length = builder.Length - 2;
            builder.Append(")");

            return builder.ToString();
        }

        #endregion

        #region Privates/internals

        readonly SearchResultMetadata metadata;

        ExpandoObject tagsObject;

        /// <summary>
        /// Initializes a new instance of the Splunk.Client.SearchResult class.
        /// </summary>
        /// <param name="metadata">
        /// The metadata.
        /// </param>
        internal SearchResult(SearchResultMetadata metadata)
        {
            this.metadata = metadata;
        }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        internal SearchResultMetadata Metadata
        {
            get { return this.metadata; }
        }

        #endregion
    }
}
