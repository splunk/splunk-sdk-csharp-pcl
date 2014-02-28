/*
 * Copyright 2013 Splunk, Inc.
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
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// The <see cref="SearchResults"/> class represents a streaming XML 
    /// reader for Splunk search results. When a stream from an export search
    /// is passed to this reader, it skips any preview events in the stream. 
    /// If you want to access the preview events, use the  
    /// <see cref="MultiSearchResultsXml"/> class.
    /// </summary>
    public class SearchResults : IDisposable, IEnumerable<Record>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResults"/> 
        /// class for the event stream. You should only attempt to parse an XML
        /// stream with the XML reader. 
        /// </summary>
        /// <param name="stream">The stream to parse.</param>
        public SearchResults(Stream stream)
            : this(stream, false)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResults"/> class.
        /// </summary>
        /// <param name="stream">The XML stream to parse.</param>
        /// <param name="isInMultiReader">
        /// Whether the reader is the underlying reader of a multi-reader.
        /// </param>
        internal SearchResults(Stream stream, bool isInMultiReader)
        {
            this.IsExportStream = false; // stream is ExportResultsStream; // TODO: figure out ExportResultsStream

            var setting = new XmlReaderSettings
            {
                Async = true, ConformanceLevel = ConformanceLevel.Fragment
            };

            this.XmlReader = XmlReader.Create(stream, setting);

            if (isInMultiReader)
            {
                return;
            }

            while (true)
            {
                // Stop if no more set is available
                if (!this.NextResultSet())
                {
                    break;
                }

                // No skipping of result sets if the stream is not from an export endpoint.
                if (!this.IsExportStream)
                {
                    break;
                }

                // Skipping ends at any file results.
                if (!this.IsPreview)
                {
                    break;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets all the field names that may appear in each result.
        /// </summary>
        /// <remarks>
        /// Be aware that any given result will contain a subset of these fields.
        /// </remarks>
        public IReadOnlyList<string> FieldNames
        { get; private set; }

        /// <summary>
        /// Gets a value indicating whether these <see cref="SearchResults"/> are 
        /// a preview of the results from an unfinished search job.
        /// </summary>
        /// <remarks>
        /// This property's default value is "false", which results in no 
        /// results set skipping or concatenation.
        /// </remarks>
        public bool IsPreview
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Releases unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Record> GetEnumerator()
        {
            while (true)
            {
                foreach (var record in this.ResultSet())
                {
                    yield return record;
                }

                // We don't concatenate across previews across sets, since each
                // set might be a snapshot at a given time or a summary result
                // with partial data from a reporting search (for example,
                // "count by host"). So if this is a preview, break to end the 
                // iteration.
                if (this.IsPreview)
                {
                    break;
                }

                // If we did not advance to next set, i.e. the end of stream is
                // reached, break to end the iteration.
                if (!this.NextResultSet())
                {
                    break;
                }

                // We have advanced to the next set. IsPreview is for that set.
                // It should not be a preview. Splunk should never return a preview
                // after final results which we might have concatenated together
                // across sets.
                Debug.Assert(!this.IsPreview, "Preview result set should never be after a final set.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Privates/internals

        bool disposed;

        private void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.XmlReader.Dispose();
                this.disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the stream is 
        /// from the export endpoint.
        /// </summary>
        internal bool IsExportStream
        { get; private set; }

        /// <summary>
        /// Returns an enumerator over a set of the events 
        /// in the event stream, and gets ready for the next set.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When using 'search/jobs/export endpoint', search results
        /// will be streamed back as they become available. It is possible
        /// for one or more previews to be received before the final one.
        /// The enumerator returned will be over a single preview or 
        /// the final results. Each time this method is called, 
        /// the next preview or the final results are enumerated if they are
        /// available; otherwise, an exception is thrown.
        /// </para>
        /// <para>
        /// After all events in the set is enumerated, the metadata of the 
        /// next set (if available) is read, with 
        /// <see cref="SearchResults.IsPreview"/> 
        /// and <see cref="SearchResults.Fields"/> being set accordingly.
        /// </para>
        /// </remarks>
        /// <returns>A enumerator.</returns>
        internal IEnumerable<Record> ResultSet()
        {
            while (true)
            {
                if (!this.XmlReader.ReadToNextSibling("result"))
                {
                    yield break;
                }

                var result = new Record();

                this.ReadEachDescendant("field", () =>
                {
                    var key = this.XmlReader["k"];

                    if (key == null)
                    {
                        throw new XmlException("'field' attribute 'k' not found");
                    }

                    var values = new List<string>();

                    var xmlDepthField = this.XmlReader.Depth;

                    while (this.XmlReader.Read())
                    {
                        if (this.XmlReader.Depth == xmlDepthField)
                        {
                            break;
                        }

                        Debug.Assert(XmlReader.Depth > xmlDepthField, "The loop should have exited earlier.");

                        if (this.XmlReader.IsStartElement("value"))
                        {
                            if (this.XmlReader.ReadToDescendant("text"))
                            {
                                values.Add(this.XmlReader.ReadElementContentAsString());
                            }
                        }
                        else if (this.XmlReader.IsStartElement("v"))
                        {
                            result.SegmentedRaw = this.XmlReader.ReadOuterXml();
                            string value = ReadElementContentAsString(result.SegmentedRaw);
                            values.Add(value);
                        }
                    }

                    result.Add(key, new Field(values.ToArray()));
                });

                yield return result;
            }
        }

        /// <summary>
        /// Advances to the next search results set.
        /// </summary>
        /// <returns><code>true</code> if there is a next search results set; 
        /// otherwise, if the current result set is the final result set, a 
        /// value of <code>false</code>.</returns>
        /// <remarks>
        /// This operation skips remaining records in the current search results
        /// set and reads metadata before reading the first results 
        /// <see cref="Record"/>.
        /// Below is an example of an input stream, with a single 'results'
        /// element. With a stream from an export point, there can be
        /// multiple ones.
        /// 
        /// <example><![CDATA[
        /// <?xml version='1.0' encoding='UTF-8'?>
        ///  <results preview='0'>
        ///    <meta>
        ///    <fieldOrder>
        ///        <field>series</field>
        ///        <field>sum(kb)</field>
        ///    </fieldOrder>
        ///    </meta>
        ///    <messages>
        ///    <msg type='DEBUG'>base lispy: [ AND ]</msg>
        ///    <msg type='DEBUG'>search context: user='admin', app='search', bs-pathname='/some/path'</msg>
        ///    </messages>
        ///    <result offset='0'>
        ///    <field k='series'>
        ///        <value>
        ///        <text>twitter</text>
        ///        </value>
        ///    </field>
        ///    <field k='sum(kb)'>
        ///        <value>
        ///        <text>14372242.758775</text>
        ///        </value>
        ///    </field>
        ///    </result>
        ///    <result offset='1'>
        ///    <field k='series'>
        ///        <value>
        ///        <text>splunkd</text>
        ///        </value>
        ///    </field>
        ///    <field k='sum(kb)'>
        ///        <value>
        ///        <text>267802.333926</text>
        ///        </value>
        ///    </field>
        ///    </result>
        /// </results>
        /// </remarks>]]>
        /// </example>
        internal bool NextResultSet()
        {
            if (this.XmlReader.ReadToFollowing("results"))
            {
                this.IsPreview = XmlConvert.ToBoolean(this.XmlReader["preview"]);
                this.ReadMetaElement();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the underlying reader of the XML stream.
        /// </summary>
        internal XmlReader XmlReader
        { get; private set; }

        /// <summary>
        /// Reads each descendant found and positions the reader on the end
        /// tag of the current node.
        /// </summary>
        /// <param name="name">Name of the descendant.</param>
        /// <param name="readAction">
        /// The action that reads each descendant found, and 
        /// positions the reader at the decendant's element depth
        /// (for instance, the end tag or the start tag).
        /// </param>
        private void ReadEachDescendant(string name, Action readAction)
        {
            if (this.XmlReader.ReadToDescendant(name))
            {
                readAction();

                while (this.XmlReader.ReadToNextSibling(name))
                {
                    readAction();
                }
            }
        }

        /// <summary>
        /// Extracts and concatenate text, excluding any markup.
        /// </summary>
        /// <param name="xml">The XML fragment with markup.</param>
        /// <returns>Extracted and concatenated text.</returns>
        private static string ReadElementContentAsString(string xml)
        {
            var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };

            using (var reader = XmlReader.Create(new StringReader(xml), settings))
            {
                var result = new StringBuilder();

                while (reader.Read())
                {
                    result.Append(reader.ReadElementContentAsString());
                }

                return result.ToString();
            }
        }

        /// <summary>
        /// Reads the <b>meta</b> element to populate the <see cref="SearchResults.FieldNames"/>
        /// property, and moves to its end tag.
        /// </summary>
        private void ReadMetaElement()
        {
            List<string> fields = new List<string>();

            if (this.XmlReader.ReadToDescendant("meta"))
            {
                if (this.XmlReader.ReadToDescendant("fieldOrder"))
                {
                    this.ReadEachDescendant("field", () => fields.Add(this.XmlReader.ReadElementContentAsString()));
                    this.XmlReader.Skip();
                }
                this.XmlReader.Skip();
            }

            this.FieldNames = fields;
        }

        #endregion
    }
}
