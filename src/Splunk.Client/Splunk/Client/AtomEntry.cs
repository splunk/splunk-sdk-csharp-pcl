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
////
//// [ ] Consider IValueConverter as an alternative to As<data-type> methods
////     in the Field class
////
////     Idea: A single set of value converters for all Splunk data types 
////     providing consistent conversion, manipulation, and presentation on 
////     all Portable Class Library platforms with consideration for 
////     CultureInfo.
////
////     Selling point: One of the handy things you can do with data binding 
////     in WPF and Silverlight is convert the data as you pull it from the 
////     data source. The interface used for doing this is IValueConverter.
////     It is part of the Portable Class Library profile and available on 
////     all current Windows platforms as well as Xamarin (?)
////
////     Note: 
////     Neither IValueConveter or TypeConverter, an alternative to 
////     IValueConverter, is part of the Portable Class Library profile because 
////     it is not implemented for Windows Store apps.
////
////     References:
////     1. [IValueConverter in WPF data binding](http://goo.gl/wHBov)
////     2. [IValueConverter Class](http://goo.gl/Dep0VT)
////     3. [ValueConversionAttribute Class](http://goo.gl/0aKer9)
////
//// [O] Improve error handling. Bad data should in an element should 
////     produce diagnostic field-oriented error messages via 
////     InvalidDataException.
////
//// [ ] Identify optional properties and sensible default values, if
////     they're missing.
////
//// [ ] Check AtomEntry properties against splunk-sdk-csharp-1.0.X
////
//// [X] Either drop or improve AtomEntry.NormalizePropertyName
////     Improved AtomEntry.NormalizePropertyName since we do not have
////     known serialization requirements; just deserialization.
////
//// [O] Contracts
////
//// [ ] Documentation
////
//// [ ] Use NameTable in AtomEntry and AtomFeed

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Provides an object representation of an individual entry in a Splunk 
    /// Atom Feed response.
    /// <para>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///     <a href="http://goo.gl/YVTE9l">REST API: Atom Feed responses</a>
    /// </description></item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class AtomEntry
    {
        #region Constructors

        public AtomEntry()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string Author
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public dynamic Content
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Uri Id
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyDictionary<string, Uri> Links
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Published
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Title
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Updated
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously reads XML data into the current <see cref="AtomEntry"/>.
        /// </summary>
        /// <param name="reader">
        /// The reader from which to read.
        /// </param>
        /// <returns></returns>
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

            var links = new Dictionary<string, Uri>();
            this.Links = links;

            await reader.ReadAsync();

            while (reader.NodeType == XmlNodeType.Element)
            {
                string name = reader.Name;

                switch (name)
                {
                    case "title":

                        this.Title = await reader.ReadElementContentAsync(StringConverter.Instance);
                        break;

                    case "id":

                        this.Id = await reader.ReadElementContentAsync(UriConverter.Instance);
                        break;

                    case "author":

                        await reader.ReadAsync();

                        if (!(reader.NodeType == XmlNodeType.Element && reader.Name == "name"))
                        {
                            throw new InvalidDataException(); // TODO: Diagnostics
                        }

                        this.Author = await reader.ReadElementContentAsync(StringConverter.Instance);

                        if (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "author"))
                        {
                            throw new InvalidDataException(); // TODO: Diagnostics
                        }

                        await reader.ReadAsync();
                        break;

                    case "published":

                        this.Published = await reader.ReadElementContentAsync(DateTimeConverter.Instance);
                        break;

                    case "updated":

                        this.Updated = await reader.ReadElementContentAsync(DateTimeConverter.Instance);
                        break;

                    case "link":

                        string href = reader.GetAttribute("href");

                        if (string.IsNullOrWhiteSpace(href))
                        {
                            throw new InvalidDataException();  // TODO: Diagnostics
                        }

                        string rel = reader.GetAttribute("rel");

                        if (string.IsNullOrWhiteSpace(rel))
                        {
                            throw new InvalidDataException();  // TODO: Diagnostics
                        }

                        links[rel] = UriConverter.Instance.Convert(href);
                        await reader.ReadAsync();
                        break;

                    case "content":

                        this.Content = await ParsePropertyValueAsync(reader);
                        break;

                    default: throw new InvalidDataException(); // TODO: Diagnostics
                }
            }

            if (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "entry"))
            {
                throw new InvalidDataException(); // TODO: Diagnostics
            }

            await reader.ReadAsync();
        }

        public override string ToString()
        {
            return string.Format("AtomEntry(Title={0}, Author={1}, Id={2}, Updated={3})", this.Title, this.Author, this.Id, this.Updated);
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

        static async Task<dynamic> ParseDictionaryAsync(XmlReader reader)
        {
            var value = (IDictionary<string, dynamic>)new ExpandoObject();

            if (!reader.IsEmptyElement)
            {
                await reader.ReadAsync();

                while (reader.NodeType == XmlNodeType.Element && reader.Name == "s:key")
                {
                    string name = reader.GetAttribute("name");

                    // TODO: Include a domain-specific name translation capability (?)

                    switch (name)
                    {
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
                        case "display.visualizations.charting.chart":
                            name += ".Type";
                            break;
                        case "update.checksum.type":
                            name = "update.checksum_type";
                            break;
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
                                throw new InvalidDataException(); // TODO: Diagnostics
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
                    propertyValue = await ParsePropertyValueAsync(reader);
                    dictionary.Add(propertyName, propertyValue);
                }

                if (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "s:dict"))
                {
                    throw new InvalidDataException();
                }
            }

            await reader.ReadAsync();
            return value;  // TODO: what's the type seen by dynamic?
        }

        static async Task<IReadOnlyList<dynamic>> ParseListAsync(XmlReader reader)
        {
            List<dynamic> value = new List<dynamic>();

            if (!reader.IsEmptyElement)
            {
                await reader.ReadAsync();

                while (reader.NodeType == XmlNodeType.Element && reader.Name == "s:item")
                {
                    value.Add(await ParsePropertyValueAsync(reader));
                }

                if (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "s:list"))
                {
                    throw new InvalidDataException();
                }
            }

            await reader.ReadAsync();
            return value;
        }

        static async Task<dynamic> ParsePropertyValueAsync(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                await reader.ReadAsync();
                return null;
            }

            string name = reader.Name;
            dynamic value;

            await reader.ReadAsync();

            switch (reader.NodeType)
            {
                default:

                    value = await reader.ReadContentAsStringAsync();
                    break;

                case XmlNodeType.Element:

                    // TODO: rewrite

                    switch (reader.Name)
                    {
                        case "s:dict":

                            value = await ParseDictionaryAsync(reader);
                            break;

                        case "s:list":

                            value = await ParseListAsync(reader);
                            break;

                        default: throw new InvalidDataException(string.Format("Unexpected element name: {0}", reader.Name));
                    }

                    break;

                case XmlNodeType.EndElement:

                    if (reader.Name != name)
                    {
                        throw new InvalidDataException(); // TODO: Diagnostics
                    }
                    value = null;
                    break;
            }

            if (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == name))
            {
                throw new InvalidDataException(); // TODO: Diagnostics
            }

            await reader.ReadAsync();
            return value;
        }

        #endregion
    }
}
