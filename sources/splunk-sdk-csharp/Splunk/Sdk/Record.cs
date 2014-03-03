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
    using System.Collections.Generic;
    using System.Text;
    
    /// <summary>
    /// The <see cref="Record"/> class wraps an individual event or result that
    /// was returned by the <see cref="ResultsReader"/> class.
    /// An event maps each field name to an instance of
    /// <see cref="Record.FieldValue"/> class, which is a list of zero of more
    /// values.
    /// </summary>
    public class Record : Dictionary<string, Field>
    {
        /// <summary>
        /// Gets the XML markup for the <code>_raw</code> field value.
        /// <remarks>
        /// <para>
        /// The return value is different than that of the <code>_raw</code> 
        /// field value in that this segmented raw value is an XML fragment
        /// that includes all markup such as XML tags and escaped characters.
        /// For example, <code>record["_raw"]</code> field value returns this:
        /// </para>
        /// <example>
        /// <![CDATA[
        /// "http://localhost:8000/en-US/app/search/flashtimeline?q=search%20search%20index%3D_internal%20%7C%20head%2010&earliest=rt-1h&latest=rt"
        /// ]]>
        /// </example>
        /// <para>
        /// While <code>record.SegmentedRaw</code> returns this:</para>
        /// <example>
        /// <v xml:space="preserve" trunc="0">"http://localhost:8000/en-US/app/<sg h="1">search</sg>/flashtimeline?q=<sg h="1">search</sg>%20<sg h="1">search</sg>%20index%3D_internal%20%7C%20head%2010&amp;earliest=rt-1h&amp;latest=rt"</v>
        /// </example>
        /// </remarks>
        /// </summary>
        public string SegmentedRaw { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder("Record(");

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
    }
}
