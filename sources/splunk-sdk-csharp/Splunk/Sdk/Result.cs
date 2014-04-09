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

namespace Splunk.Sdk
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
    /// The <see cref="Result"/> class wraps an individual event or result that
    /// was returned by the <see cref="ResultsReader"/> class.
    /// An event maps each field name to an instance of
    /// <see cref="Result.FieldValue"/> class, which is a list of zero of more
    /// values.
    /// </summary>
    public class Result : Dictionary<string, Field>
    {
        /// <summary>
        /// Gets the XML markup for the <c>_raw</c> field value.
        /// <remarks>
        /// <para>
        /// The return value is different than that of the <c>_raw</c> field 
        /// value in that this segmented raw value is an XML fragment that 
        /// includes all markup such as XML tags and escaped characters. For 
        /// example, <c>record["_raw"]</c> field value returns this:
        /// </para>
        /// <example>
        /// <![CDATA[
        /// "http://localhost:8000/en-US/app/search/flashtimeline?q=search%20search%20index%3D_internal%20%7C%20head%2010&earliest=rt-1h&latest=rt"
        /// ]]>
        /// </example>
        /// <para>
        /// While <c>record.SegmentedRaw</c> returns this:</para>
        /// <example>
        /// <v xml:space="preserve" trunc="0">"http://localhost:8000/en-US/app/<sg h="1">search</sg>/flashtimeline?q=<sg h="1">search</sg>%20<sg h="1">search</sg>%20index%3D_internal%20%7C%20head%2010&amp;earliest=rt-1h&amp;latest=rt"</v>
        /// </example>
        /// </remarks>
        /// </summary>
        public string SegmentedRaw { get; internal set; }

        #region Methods

        public async Task ReadXmlAsync(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");

            if (reader.ReadState == ReadState.Initial)
            {
                await reader.ReadAsync();

                if (reader.NodeType == XmlNodeType.XmlDeclaration)
                {
                    await reader.ReadAsync();
                }
            }
            else
            {
                reader.MoveToElement();
            }

            if (!(reader.NodeType == XmlNodeType.Element && reader.Name == "entry"))
            {
                throw new InvalidDataException(); // TODO: Diagnostics
            }

            await reader.ReadEachDescendantAsync("field", async () =>
            {
                var key = reader["k"];

                if (key == null)
                {
                    throw new XmlException("'field' attribute 'k' not found");
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
