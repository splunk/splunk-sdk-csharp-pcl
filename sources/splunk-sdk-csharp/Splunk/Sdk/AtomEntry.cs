﻿/*
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
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    class AtomEntry
    {
        #region Constructors

        public AtomEntry(XElement entry)
        {
            Contract.Requires<ArgumentNullException>(entry != null);
            XElement element;

            // Title

            element = entry.Element(AtomFeed.ElementName.Title);

            if (element == null)
            {
                throw new InvalidDataException(string.Format("Missing title element in atom feed entry {0}", entry.Name));
            }

            this.Title = element.Value;

            // Author

            element = entry.Element(AtomFeed.ElementName.Author);

            if (element == null)
            {
                throw new InvalidDataException(string.Format("Missing author element in atom feed entry {0}", entry.Name));
            }

            this.Author = element.Value;

            // Id

            element = entry.Element(AtomFeed.ElementName.Id);

            if (element == null)
            {
                throw new InvalidDataException(string.Format("Missing id element in atom feed entry {0}", entry.Name));
            }

            this.Id = new Uri(element.Value);

            // Published

            element = entry.Element(AtomFeed.ElementName.Updated);

            if (element == null)
            {
                throw new InvalidDataException(string.Format("Missing published element in atom feed entry {0}", entry.Name));
            }

            this.Updated = DateTime.Parse(element.Value);

            // Updated

            element = entry.Element(AtomFeed.ElementName.Updated);

            if (element == null)
            {
                throw new InvalidDataException(string.Format("Missing updated element in atom feed entry {0}", entry.Name));
            }

            this.Updated = DateTime.Parse(element.Value);

            // Content

            element = entry.Element(AtomFeed.ElementName.Content);

            if (element == null)
            {
                throw new InvalidDataException(string.Format("Missing content element in atom feed entry {0}", entry.Name));
            }
            this.Content = ParsePropertyValue(element);

            // Links

            var baseUri = new Uri(this.Id.ToString() + "/"); // must be terminated by a slash
            var links = new Dictionary<string, Uri>();

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

        static string NormalizePropertyName(string name)
        {
            return char.ToUpper(name[0]) + name.Substring(1);
        }

        static dynamic ParsePropertyValue(XElement content)
        {
            if (content.FirstNode == null)
            {
                return null; // no content is represented by a null value
            }

            if (content.FirstNode.NextNode != null)
            {
                throw new InvalidDataException(); // we expect a single value, not multiple values
            }

            if (content.FirstNode.NodeType == XmlNodeType.Element)
            {
                var element = (XElement)content.FirstNode;

                if (element.Name == AtomFeed.ElementName.Dict)
                {
                    return ParseDictionary(element);
                }
                if (element.Name == AtomFeed.ElementName.List)
                {
                    return ParseList(element);
                }
                throw new InvalidDataException(string.Format("Unrecognized element name: {0}", element.Name));
            }
            else if (content.FirstNode.NodeType == XmlNodeType.Text)
            {
                XText text = (XText)content.FirstNode;
                return text.Value;
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
