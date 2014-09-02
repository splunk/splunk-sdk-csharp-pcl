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
//// [ ] Performance: NameTable could make in AtomEntry.ReadXmlAsync and 
////     AtomFeed.ReadXmlAsync significantly faster.
//// [ ] Synchronization: AtomFeed.ReadXmlAsync and AtomEntry.ReadXmlAsync can
////     be called more than once. (In practice these methods are never called
////     move than once.)

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Provides an object representation of an individual entry in a Splunk Atom
    /// Feed response.
    /// <para>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/TDthxd">REST API Reference Manual: Accessing
    ///   Splunk resources</a>.
    /// </description></item>
    /// <item><description>
    ///   <a href="http://goo.gl/YVTE9l">REST API Reference Manual: Atom Feed
    ///   responses</a>.
    /// </description></item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class AtomEntry
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomEntry"/> class.
        /// </summary>
        public AtomEntry()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the owner of the resource represented by the current
        /// <see cref="AtomEntry"/>, as defined in the access control list.
        /// </summary>
        /// <remarks>
        /// This value can be <c>"system"</c>, <c>"nobody"</c> or some specific user
        /// name. Refer to <a href="http://goo.gl/iTpzO0">Access control lists for
        /// Splunk objects</a> in the section on
        /// <a href= "http://goo.gl/TDthxd">Accessing Splunk resources</a>.
        /// </remarks>
        /// <value>
        /// Owner of the resource represented by the current <see cref= "AtomEntry"/>.
        /// </value>
        public string Author
        { get; private set; }

        /// <summary>
        /// Gets a dynamic object representing the content of the resource
        /// represented by the current <see cref="AtomEntry"/>.
        /// </summary>
        /// <remarks>
        /// Splunk typically returns content as dictionaries with key/value pairs
        /// that list properties of the entry. However, content can be returned as a
        /// list of values or as plain text.
        /// </remarks>
        /// <value>
        /// A dynamic object representing the content of the resource represented by
        /// the current <see cref="AtomEntry"/>.
        /// </value>
        public dynamic Content
        { get; private set; }

        /// <summary>
        /// Gets the Splunk management URI for accessing the resource represented by
        /// the current <see cref="AtomEntry"/>.
        /// </summary>
        /// <value>
        /// The Splunk management URI for accessing the resource represented by the
        /// current <see cref="AtomEntry"/>.
        /// </value>
        public Uri Id
        { get; private set; }

        /// <summary>
        /// Gets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        public IReadOnlyDictionary<string, Uri> Links
        { get; private set; }

        /// <summary>
        /// Gets the Date/Time of the published.
        /// </summary>
        /// <value>
        /// The published.
        /// </value>
        public DateTime Published
        { get; private set; }

        /// <summary>
        /// Gets the human readable name for the current <see cref="AtomEntry"/>.
        /// </summary>
        /// <remarks>
        /// This value varies depending on the endpoint used to access the current
        /// <see cref="AtomEntry"/>.
        /// </remarks>
        /// <value>
        /// The human readable name for the current <see cref="AtomEntry"/>.
        /// </value>
        public string Title
        { get; private set; }

        /// <summary>
        /// Gets the date and time the current <see cref="AtomEntry"/> was last
        /// updated in Splunk.
        /// </summary>
        /// <value>
        /// The date and time the current <see cref="AtomEntry"/> was last updated in
        /// Splunk.
        /// </value>
        public DateTime Updated
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously reads XML data into the current <see cref="AtomEntry"/>.
        /// </summary>
        /// <exception cref="InvalidDataException">
        /// Thrown when an Invalid Data error condition occurs.
        /// </exception>
        /// <param name="reader">
        /// The reader from which to read.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public async Task ReadXmlAsync(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");

            this.Author = null;
            this.Content = null;
            this.Id = null;
            this.Links = null;
            this.Published = DateTime.MinValue;
            this.Title = null;
            this.Updated = DateTime.MinValue;

            reader.Requires(await reader.MoveToDocumentElementAsync("entry").ConfigureAwait(false));

            Dictionary<string, Uri> links = null;
            await reader.ReadAsync().ConfigureAwait(false);

            while (reader.NodeType == XmlNodeType.Element)
            {
                string name = reader.Name;

                switch (name)
                {
                    case "title":

                        this.Title = await reader.ReadElementContentAsync(StringConverter.Instance).ConfigureAwait(false);
                        break;

                    case "id":

                        this.Id = await reader.ReadElementContentAsync(UriConverter.Instance).ConfigureAwait(false);
                        break;

                    case "author":

                        await reader.ReadAsync().ConfigureAwait(false);
                        reader.EnsureMarkup(XmlNodeType.Element, "name");
                        this.Author = await reader.ReadElementContentAsync(StringConverter.Instance).ConfigureAwait(false);
                        reader.EnsureMarkup(XmlNodeType.EndElement, "author");
                        await reader.ReadAsync().ConfigureAwait(false);
                        break;

                    case "published":

                        this.Published = await reader.ReadElementContentAsync(DateTimeConverter.Instance).ConfigureAwait(false);
                        break;

                    case "updated":

                        this.Updated = await reader.ReadElementContentAsync(DateTimeConverter.Instance).ConfigureAwait(false);
                        break;

                    case "link":

                        if (links == null)
                        {
                            links = new Dictionary<string, Uri>();
                        }

                        var href = reader.GetRequiredAttribute("href");
                        var rel = reader.GetRequiredAttribute("rel");
                        links[rel] = UriConverter.Instance.Convert(href);
                        await reader.ReadAsync().ConfigureAwait(false);
                        break;

                    case "content":

                        this.Content = await ParsePropertyValueAsync(reader, 0).ConfigureAwait(false);
                        break;

                    default: throw new InvalidDataException(); // TODO: Diagnostics : unexpected start tag
                }
            }

            reader.EnsureMarkup(XmlNodeType.EndElement, "entry");
            await reader.ReadAsync().ConfigureAwait(false);

            if (links != null)
            {
                this.Links = new ReadOnlyDictionary<string, Uri>(links);
            }
        }

        /// <summary>
        /// Gets a string representation for the current <see cref="AtomEntry"/>.
        /// </summary>
        /// <returns>
        /// A string representation of the current <see cref="AtomEntry"/>.
        /// </returns>
        /// <seealso cref="M:System.Object.ToString()"/>
        public override string ToString()
        {
            var text= string.Format(CultureInfo.CurrentCulture, "AtomEntry(Title={0}, Author={1}, Id={2}, Published={3}, Updated={4})", 
                this.Title, this.Author, this.Id, this.Published, this.Updated);
            return text;
        }

        #endregion

        #region Privates

        static string NormalizePropertyName(string name)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name));
            var builder = new StringBuilder(name.Length);
            int index = 0;

            // Leading underscores distinguish some names

            while (name[index] == '_')
            {
                builder.Append('_');

                if (++index >= name.Length)
                {
                    return builder.ToString();
                }
            }

            for (; ; )
            {
                // We squeeze out dashes, dots, and all but the leading underscores

                while (name[index] == '_' || name[index] == '.' || name[index] == '-')
                {
                    if (++index >= name.Length)
                    {
                        return builder.ToString();
                    }
                }

                // We capitalize the first character following [_.-]

                builder.Append(char.ToUpper(name[index]));

                if (++index >= name.Length)
                {
                    return builder.ToString();
                }

                // We don't alter the case of subsequent characters following [_.-]

                while (!(name[index] == '_' || name[index] == '.' || name[index] == '-'))
                {
                    builder.Append(name[index]);

                    if (++index >= name.Length)
                    {
                        return builder.ToString();
                    }
                }
            }
        }

        static async Task<dynamic> ParseDictionaryAsync(XmlReader reader, int level)
        {
            var value = (IDictionary<string, dynamic>)new ExpandoObject();

            if (!reader.IsEmptyElement)
            {
                await reader.ReadAsync().ConfigureAwait(false);

                while (reader.NodeType == XmlNodeType.Element && reader.Name == "s:key")
                {
                    string name = reader.GetAttribute("name");

                    // TODO: Include a domain-specific name translation capability (?)

                    if (level == 0)
                    {
                        switch (name)
                        {
                            case "action.email.subject.alert":
                                name = "action.email.subject_alert";
                                break;
                            case "action.email.subject.report":
                                name = "action.email.subject_report";
                                break;
                            case "action.email":
                            case "action.populate_lookup":
                            case "action.rss":
                            case "action.script":
                            case "action.summary_index":
                            case "alert.suppress":
                            case "auto_summarize":
                                name += ".IsEnabled";
                                break;
                            case "alert_comparator":
                                name = "alert.comparator";
                                break;
                            case "alert_condition":
                                name = "alert.condition";
                                break;
                            case "alert_threshold":
                                name = "alert.threshold";
                                break;
                            case "alert_type":
                                name = "alert.type";
                                break;
                            case "coldPath.maxDataSizeMB":
                                name = "coldPath_maxDataSizeMB";
                                break;
                            case "display.visualizations.charting.chart":
                                name += ".Type";
                                break;
                            case "homePath.maxDataSizeMB":
                                name = "homePath_maxDataSizeMB";
                                break;
                            case "update.checksum.type":
                                name = "update.checksum_type";
                                break;
                        }
                    }

                    string[] names = name.Split(':', '.');
                    var dictionary = value;
                    string propertyName;
                    dynamic propertyValue;

                    for (int i = 0; i < names.Length - 1; i++)
                    {
                        propertyName = NormalizePropertyName(names[i]);

                        if (dictionary.TryGetValue(propertyName, out propertyValue))
                        {
                            if (!(propertyValue is ExpandoObject))
                            {
                                throw new InvalidDataException(); // TODO: Diagnostics : conversion error
                            }
                        }
                        else
                        {
                            propertyValue = new ExpandoObject();
                            dictionary.Add(propertyName, propertyValue);
                        }

                        dictionary = (IDictionary<string, object>)propertyValue;
                    }

                    propertyName = NormalizePropertyName(names[names.Length - 1]);
                    propertyValue = await ParsePropertyValueAsync(reader, level + 1).ConfigureAwait(false);
                    dictionary.Add(propertyName, propertyValue);
                }

                reader.EnsureMarkup(XmlNodeType.EndElement, "s:dict");
            }

            await reader.ReadAsync().ConfigureAwait(false);
            return value;  // TODO: what's the type seen by dynamic?
        }

        static async Task<ReadOnlyCollection<dynamic>> ParseListAsync(XmlReader reader, int level)
        {
            List<dynamic> value = new List<dynamic>();

            if (!reader.IsEmptyElement)
            {
                await reader.ReadAsync().ConfigureAwait(false);

                while (reader.NodeType == XmlNodeType.Element && reader.Name == "s:item")
                {
                    value.Add(await ParsePropertyValueAsync(reader, level + 1).ConfigureAwait(false));
                }
                
                reader.EnsureMarkup(XmlNodeType.EndElement, "s:list");
            }

            await reader.ReadAsync().ConfigureAwait(false);
            return new ReadOnlyCollection<dynamic>(value);
        }

        static async Task<dynamic> ParsePropertyValueAsync(XmlReader reader, int level)
        {
            if (reader.IsEmptyElement)
            {
                await reader.ReadAsync().ConfigureAwait(false);
                return null;
            }

            string name = reader.Name;
            dynamic value;

            await reader.ReadAsync().ConfigureAwait(false);

            switch (reader.NodeType)
            {
                default:

                    value = await reader.ReadContentAsStringAsync().ConfigureAwait(false);
                    break;

                case XmlNodeType.Element:

                    // TODO: rewrite

                    switch (reader.Name)
                    {
                        case "s:dict":

                            value = await ParseDictionaryAsync(reader, level).ConfigureAwait(false);
                            break;

                        case "s:list":

                            value = await ParseListAsync(reader, level).ConfigureAwait(false);
                            break;

                        default: throw new InvalidDataException(); // TODO: Diagnostics : unexpected start tag
                    }

                    break;

                case XmlNodeType.EndElement:

                    reader.EnsureMarkup(XmlNodeType.EndElement, name);
                    value = null;
                    break;
            }

            reader.EnsureMarkup(XmlNodeType.EndElement, name);
            await reader.ReadAsync().ConfigureAwait(false);

            return value;
        }

        #endregion
    }
}
