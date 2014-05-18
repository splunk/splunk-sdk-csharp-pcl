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
//// [O]  Documentation


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
    /// Represents a single record on a <see cref="SearchResultStream"/>.
    /// </summary>
    public class SearchResult : Dictionary<string, Field>
    {
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

        #region Methods

        /// <summary>
        /// Asynchronously reads data into the current <see cref="SearchResult"/>.
        /// </summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/> from which to read.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing this operation.
        /// </returns>
        public async Task ReadXmlAsync(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");
            reader.MoveToElement();

            if (!(reader.NodeType == XmlNodeType.Element && reader.Name == "result"))
            {
                throw new InvalidDataException(); // TODO: Diagnostics : unexpected start tag
            }

            await reader.ReadEachDescendantAsync("field", async () =>
            {
                var key = reader["k"];

                if (key == null)
                {
                    throw new InvalidDataException("'field' attribute 'k' not found"); // TODO: Diagnostics : missing attribute value
                }

                var fieldDepth = reader.Depth;
                var values = new List<string>();

                while (await reader.ReadAsync())
                {
                    if (reader.Depth == fieldDepth)
                    {
                        break;
                    }

                    Debug.Assert(reader.Depth > fieldDepth, "This loop should have exited earlier.");

                    // TODO: Replace calls to IsStartElement because it is blocking

                    if (reader.IsStartElement("value"))
                    {
                        if (await reader.ReadToDescendantAsync("text"))
                        {
                            values.Add(await reader.ReadElementContentAsStringAsync());
                        }
                    }
                    else if (reader.IsStartElement("v"))
                    {
                        string value = await reader.ReadOuterXmlAsync();
                        this.SegmentedRaw = value;
                        values.Add(value);
                    }
                }

                this.Add(key, new Field(values.ToArray()));
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

            foreach (KeyValuePair<string, Field> pair in this)
            {
                builder.Append(pair.Key);
                builder.Append(" -> ");
                builder.Append(pair.Value);
                builder.Append(", ");
            }

            builder.Length = builder.Length - 2;
            builder.Append(")");

            return builder.ToString();
        }

        #endregion
    }
}
