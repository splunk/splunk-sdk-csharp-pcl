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
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    
    /// <summary>
    /// Provides an object representation of a Splunk Atom feed.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    class AtomFeed<TEntity> where TEntity: Entity<TEntity>, new()
    {
        // TODO: Refactor into AtomFeed and AtomEntry. Refactor EntityCollection<TEntity> to match a structure similar to this:
        //
        //  entityCollection: EntityCollection<TEntity>
        //      feed: IReadOnlyList<AtomEntry>
        //      *entity: TEntity
        //  entity: Entity
        //      entry: AtomEntry <- collection.feed[i]
        //      properties: * <- entity.entry.*

        #region Constructors

        public AtomFeed(Context context, ResourceName collection, XDocument document)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(document != null, "document");
            Contract.Requires<ArgumentNullException>(collection != null, "collection");

            if (document.Root.Name != ElementName.Feed)
            {
                throw new InvalidDataException();
            }

            var entries = document.Root.Elements(ElementName.Entry);

            if (entries == null)
            {
                throw new InvalidDataException();
            }

            this.Entities = new List<TEntity>(from entry in entries select CreateEntity(context, collection, entry));
        }

        #endregion

        #region Properties

        public IReadOnlyList<TEntity> Entities
        { get; private set; }

        #endregion

        #region Methods

        public static TEntity CreateEntity(Context context, ResourceName collection, XElement entry)
        {
            XElement content = entry.Element(ElementName.Content);

            if (content == null)
            {
                throw new InvalidDataException(string.Format("Missing content element in atom feed entry {0}", entry.Name));
            }

            dynamic record = CreatePropertyValue(content);
            var dictionary = (IDictionary<string, object>)record;

            var entity = new TEntity()
            {
                Namespace = new Namespace(record.Eai.Acl.Owner, record.Eai.Acl.App),
                Collection = collection,
                Name = (string)dictionary[dictionary.ContainsKey("Title") ? "Title" : "Sid"], // TODO: Rework Entity so that it pulls its name from record (e.g., create a pair of entity constructors; one that accepts a name and one that accepts a record from which the name is drawn. the base Entity constructor would draw title. derives Entities would draw the name they need)
                Record = record,
                Context = context
            };

            return entity;
        }

        #endregion

        #region Privates

        static ExpandoObject CreatePropertyDict(XElement content)
        {
            // TODO: Remove property name translation because it decreases serialization fidelity

            ExpandoObject value = new ExpandoObject();
            var valueDictionary = (IDictionary<string, object>)value;

            foreach (var element in content.Elements())
            {
                if (element.Name != ElementName.Key)
                {
                    throw new InvalidDataException();
                }

                string[] name = element.Attribute("name").Value.Split(':');

                if (name.Length > 2)
                {
                    throw new InvalidDataException();
                }

                var dictionary = valueDictionary;
                string propertyName = CreatePropertyName(name[0]);

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
                    propertyName = CreatePropertyName(name[1]);
                }

                dictionary.Add(propertyName, CreatePropertyValue(element));
            }

            return value;
        }

        static List<object> CreatePropertyList(XElement content)
        {
            List<object> value = new List<object>();

            foreach (var element in content.Elements())
            {
                if (element.Name != ElementName.Item)
                {
                    throw new InvalidDataException();
                }
                value.Add(CreatePropertyValue(element));
            }

            return value;
        }

        static string CreatePropertyName(string name)
        {
            return char.ToUpper(name[0]) + name.Substring(1);
        }

        static dynamic CreatePropertyValue(XElement content)
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

                if (element.Name == ElementName.Dict)
                {
                    return CreatePropertyDict(element);
                }
                if (element.Name == ElementName.List)
                {
                    return CreatePropertyList(element);
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

        #endregion

        #region Types

        static class ElementName 
        {
            public static readonly XName Content = "{http://www.w3.org/2005/Atom}content";
            public static readonly XName Entry = "{http://www.w3.org/2005/Atom}entry";
            public static readonly XName Feed = "{http://www.w3.org/2005/Atom}feed";

            public static readonly XName Dict = "{http://dev.splunk.com/ns/rest}dict";
            public static readonly XName Item = "{http://dev.splunk.com/ns/rest}item";
            public static readonly XName Key = "{http://dev.splunk.com/ns/rest}key";
            public static readonly XName List = "{http://dev.splunk.com/ns/rest}list";
        };

        #endregion
    }
}
