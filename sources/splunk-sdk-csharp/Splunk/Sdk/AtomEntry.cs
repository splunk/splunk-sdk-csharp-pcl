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

// TODO:
//
// [ ] Consider IValueConverter as an alternative to As<data-type> methods
//     in this class as well as in the Field class
//
//     Idea: A single set of value converters for all Splunk data types 
//     providing consistent conversion, manipulation, and presentation on 
//     all Portable Class Library platforms with consideration for 
//     CultureInfo.
//
//     Selling point: One of the handy things you can do with data binding 
//     in WPF and Silverlight is convert the data as you pull it from the 
//     data source. The interface used for doing this is IValueConverter.
//     It is part of the Portable Class Library profile and available on 
//     all current Windows platforms as well as Xamarin (?)
//
//     Note: TypeConverter, an alternative to IValueConverter, is not
//     part of the Portable Class Library profile because it is not 
//     implemented for Windows Store apps.
//
//     References:
//     1. [IValueConverter in WPF data binding](http://goo.gl/wHBov)
//     2. [IValueConverter Class](http://goo.gl/Dep0VT)
//     3. [ValueConversionAttribute Class](http://goo.gl/0aKer9)
//
// [O] Improve error handling. Bad data should in an element should 
//     produce diagnostic field-oriented error messages via 
//     InvalidDataException.
//
// [ ] Identify optional properties and sensible default values, if
//     they're missing.
//
// [ ] Check AtomEntry properties against splunk-sdk-csharp-1.0.X
//
// [X] Either drop or improve AtomEntry.NormalizePropertyName
//     Improved AtomEntry.NormalizePropertyName since we do not have
//     known serialization requirements; just deserialization.
//
// [O] Contracts
//
// [ ] Documentation

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Linq;

    class AtomEntry
    {
        #region Constructors

        public AtomEntry(XElement entry)
        {
            Contract.Requires<ArgumentNullException>(entry != null);

            this.Title = GetElement(entry, AtomFeed.ElementName.Title, AsString);
            this.Author = GetElement(entry, AtomFeed.ElementName.Author, AsString);
            this.Id = GetElement(entry, AtomFeed.ElementName.Id, AsAbsoluteUri);
            this.Published = GetElement(entry, AtomFeed.ElementName.Published, AsDateTime);
            this.Updated = GetElement(entry, AtomFeed.ElementName.Updated, AsDateTime);
            this.Content = GetElement(entry, AtomFeed.ElementName.Content, AsExpandoObject);

            // Links

            var links = new Dictionary<string, Uri>();
            string id = this.Id.AbsoluteUri;
            Uri baseUri = id.EndsWith("/") ? this.Id : new Uri(id + "/");

            foreach (var link in entry.Elements(AtomFeed.ElementName.Link))
            {
                string rel = link.Attribute("rel").Value;
                Uri uri = new Uri(baseUri, rel);
                links[rel] = uri;
            }

            this.Links = links;
        }

        #endregion

        #region Properties

        public string Author
        { get; private set; }

        public Uri Id
        { get; private set; }

        public IReadOnlyDictionary<string, Uri> Links
        { get; private set; }

        public DateTime Published
        { get; private set; }

        public DateTime Updated
        { get; private set; }

        public IReadOnlyList<Message> Messages
        { get; private set; }

        public string Title
        { get; private set; }

        public dynamic Content
        { get; private set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return string.Format("AtomEntry(Title={0}, Author={1}, Id={2}, Published={3}, Updated={3})", this.Title, this.Author, this.Id, this.Published, this.Updated);
        }

        #endregion

        #region Privates

        static Regex propertyNamePattern = new Regex(@"[_.-]+(.?)");

        static Uri AsAbsoluteUri(XElement element)
        {
            Uri result;

            if (!Uri.TryCreate(element.Value, UriKind.Absolute, out result))
            {
                throw new InvalidDataException(); // TODO: Diagnostics
            }
            return result;
        }

        static DateTime AsDateTime(XElement element)
        {
            DateTime result;

            if (!DateTime.TryParse(element.Value, out result))
            {
                throw new InvalidDataException(); // TODO: Diagnostics
            }
            return result;
        }

        static ExpandoObject AsExpandoObject(XElement element)
        {
            return ParsePropertyValue(element);
        }

        static string AsString(XElement element)
        {
            return element.Value;
        }

        static T GetElement<T>(XElement entry, XName name, Func<XElement, T> convert)
        {
            XElement element = entry.Element(name);

            if (element == null)
            {
                throw new InvalidDataException(string.Format("Missing {0} element in atom feed entry {1}", name, entry.Name));
            }

            return convert(element);
        }

        static string NormalizePropertyName(string name)
        {
            var builder = new StringBuilder(name);

            builder[0] = char.ToUpper(builder[0]);
            name = propertyNamePattern.Replace(builder.ToString(), (match) => match.Groups[1].Value.ToUpper());

            return name;
        }

        static dynamic ParsePropertyValue(XElement content)
        {
            if (content.FirstNode == null)
            {
                return null; // no content is represented by a null value
            }

            if (content.FirstNode.NextNode != null)
            {
                throw new InvalidDataException(); // TODO: Diagnostics: We expect a single value, not multiple values
            }

            var node = content.FirstNode;

            switch (node.NodeType)
            {
                case XmlNodeType.Element:
                    
                    var element = (XElement)node;

                    if (element.Name == AtomFeed.ElementName.Dict)
                    {
                        return ParseDictionary(element);
                    }
                    if (element.Name == AtomFeed.ElementName.List)
                    {
                        return ParseList(element);
                    }
                    throw new InvalidDataException(string.Format("Unrecognized element name: {0}", element.Name));

                case XmlNodeType.Text:
                    
                    XText text = (XText)node;
                    return text.Value;

                case XmlNodeType.CDATA:

                    XCData cdata = (XCData)node;
                    return cdata.Value;
            }

            throw new InvalidDataException(string.Format("Unexpected node type: {0}", content.FirstNode.NodeType));
        }

        static ExpandoObject ParseDictionary(XElement content)
        {
            // TODO: Remove property name translation because it decreases serialization fidelity

            ExpandoObject value = new ExpandoObject();
            var valueDictionary = (IDictionary<string, object>)value;

            foreach (var element in content.Elements())
            {
                if (element.Name != AtomFeed.ElementName.Key)
                {
                    throw new InvalidDataException();
                }

                string[] name = element.Attribute("name").Value.Split(':');

                if (name.Length > 2)
                {
                    throw new InvalidDataException();
                }

                var dictionary = valueDictionary;
                string propertyName = NormalizePropertyName(name[0]);

                if (name.Length == 2)
                {
                    dynamic propertyValue;

                    if (dictionary.TryGetValue(propertyName, out propertyValue))
                    {
                        if (!(propertyValue is ExpandoObject))
                        {
                            throw new InvalidDataException();
                        }
                    }
                    else
                    {
                        propertyValue = new ExpandoObject();
                        dictionary.Add(propertyName, propertyValue);
                    }

                    dictionary = (IDictionary<string, object>)propertyValue;
                    propertyName = NormalizePropertyName(name[1]);
                }

                dictionary.Add(propertyName, ParsePropertyValue(element));
            }

            return value;
        }

        static List<object> ParseList(XElement content)
        {
            List<object> value = new List<object>();

            foreach (var element in content.Elements())
            {
                if (element.Name != AtomFeed.ElementName.Item)
                {
                    throw new InvalidDataException();
                }
                value.Add(ParsePropertyValue(element));
            }

            return value;
        }

        #endregion
    }
}
