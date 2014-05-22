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
//// [X] Documentation

namespace Splunk.Client.Refactored
{
    using Splunk.Client;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Provides a base class that represents a Splunk resource as an object.
    /// </summary>
    public class Resource : DynamicObject, IComparable, IComparable<Resource>, IEquatable<Resource>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="Resource"/> instance.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="name">
        /// An object identifying a Splunk resource within <paramref name="ns"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/>, <paramref name="ns"/>, or <paramref name=
        /// "name"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ns"/> is not specific.
        /// </exception>
        internal protected Resource(Context context, Namespace ns, ResourceName name)
        {
            Contract.Requires<ArgumentException>(name != null, "resourceName");
            Contract.Requires<ArgumentNullException>(ns != null, "namespace");
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentOutOfRangeException>(ns.IsSpecific);

            this.context = context;
            this.ns = ns;
            this.resourceName = name;
            this.initialized = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// An object representing a Splunk atom feed response.
        /// </param>
        protected internal Resource(Context context, AtomFeed feed)
        {
            Contract.Requires<ArgumentNullException>(context != null);
            Contract.Requires<ArgumentNullException>(feed != null);

            this.SetIdentity(context, feed.Id);
            this.snapshot = new Snapshot(context, feed);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="entry">
        /// An object representing a Splunk atom entry response.
        /// </param>
        /// <param name="generatorVersion">
        /// The version of the generator producing the <see cref="AtomFeed"/>
        /// feed containing <paramref name="entry"/>.
        /// </param>
        protected internal Resource(Context context, AtomEntry entry, Version generatorVersion = null)
        {
            Contract.Requires<ArgumentNullException>(context != null);
            Contract.Requires<ArgumentNullException>(entry != null);

            this.SetIdentity(context, entry.Id);
            this.snapshot = new Snapshot(context, entry, generatorVersion);
        }

        #endregion

        #region Properties stable for the lifetime of an instance

        /// <summary>
        /// Gets the <see cref="Context"/> instance for the current <see cref=
        /// "Resource"/>.
        /// </summary>
        public Context Context
        {
            get { return this.context; }
        }

        /// <summary>
        /// Gets the title of the current <see cref="Resource"/> which is the 
        /// final part of <see cref="ResourceName"/>.
        /// </summary>
        public string Name
        {
            get { return this.ResourceName.Title; }
        }

        /// <summary>
        /// Gets the namespace containing the current <see cref="Resource"/>.
        /// </summary>
        public Namespace Namespace
        {
            get { return this.ns; }
        }

        /// <summary>
        /// Gets the name of the current <see cref="Resource"/>.
        /// </summary>
        public ResourceName ResourceName
        {
            get { return this.resourceName; }
        }

        #endregion

        #region Properties backed by snapshot

        /// <summary>
        /// Gets the author of the current <see cref="Resource"/>.
        /// </summary>
        public string Author
        {
            get { return this.snapshot.Author; }
        }

        /// <summary>
        /// Gets the Splunk management URI for accessing the current <see cref=
        /// "Resource"/>.
        /// </summary>
        public Uri Id
        {
            get { return this.snapshot.Id; }
        }

        /// <summary>
        /// Gets the collection of service addresses supported by the current
        /// <see cref="Resource"/>.
        /// </summary>
        public IReadOnlyDictionary<string, Uri> Links
        {
            get { return this.snapshot.Links; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Published
        {
            get { return this.snapshot.Published; }
        }

        /// <summary>
        /// The collection of Splunk resources contained by the current <see 
        /// cref="Resource"/>.
        /// </summary>
        public IReadOnlyList<Resource> Resources
        {
            get { return this.snapshot.Resources; }
        }

        /// <summary>
        /// Gets the date that the interface to this resource type was introduced
        /// or updated by Splunk.
        /// </summary>
        public DateTime Updated
        {
            get { return this.snapshot.Updated; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Compares the specified object with the current <see cref=
        /// "Resource"/> instance and indicates whether the identity of the 
        /// current instance precedes, follows, or appears in the same position
        /// in the sort order as the specified object.
        /// </summary>
        /// <param name="other">
        /// An object to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and
        /// value.
        /// </returns>
        public int CompareTo(object other)
        {
            return this.CompareTo(other as Resource);
        }

        /// <summary>
        /// Compares the specified <see cref="Resource"/> with the current 
        /// instance and indicates whether the identity of the current instance
        /// precedes, follows, or appears in the same position
        /// in the sort order as the specified instance.
        /// </summary>
        /// <param name="other">
        /// An instance to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// A signed number indicating the relative values of the current 
        /// instance and <paramref name="other"/>.
        /// </returns>
        public int CompareTo(Resource other)
        {
            if (other == null)
            {
                return 1;
            }

            if (object.ReferenceEquals(this, other))
            {
                return 0;
            }

            return this.ResourceName.CompareTo(other.ResourceName);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Resource"/> refers to 
        /// the same resource as the current one.
        /// </summary>
        /// <param name="other">
        /// The <see cref="Resource"/> to compare with the current one.
        /// </param>
        /// <returns>
        /// A value of <c>true</c> if the two instances represent the same
        /// <see cref="Resource"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            return this.Equals(other as Resource);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Resource"/> refers to 
        /// the same resource as the current one.
        /// </summary>
        /// <param name="other">
        /// The <see cref="Resource"/> to compare with the current one.
        /// </param>
        /// <returns>
        /// A value of <c>true</c> if the two instances represent the same
        /// <see cref="Resource"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Resource other)
        {
            if (other == null)
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            bool result = this.ResourceName.Equals(other.ResourceName);
            return result;
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="Resource"/>.
        /// </summary>
        /// <returns>
        /// Hash code for the current <see cref="Resource"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return ((IDictionary<string, object>)(this.snapshot.Adapter.ExpandoObject)).Keys;
        }

        /// <summary>
        /// Gets a string identifying the current <see cref="Resource"/>.
        /// </summary>
        /// <returns>
        /// A string representing the identity of the current <see cref=
        /// "Resource"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Join("/", this.Context.ToString(), this.Namespace.ToString(), this.ResourceName.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder">
        /// 
        /// </param>
        /// <param name="result">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this.snapshot.Adapter.GetValue(binder.Name);
            return result != null;
        }

        #endregion

        #region Privates

        bool initialized;

        Context context;
        Namespace ns;
        ResourceName resourceName;
        Snapshot snapshot = Snapshot.Missing;

        void SetIdentity(Context context, Uri id)
        {
            if (this.initialized)
            {
                throw new InvalidOperationException("Resource was intialized; Initialize operation may not execute again");
            }

            // Compute namespace and resource name from entry.Id

            var path = id.AbsolutePath.Split('/');

            if (path.Length < 3)
            {
                throw new InvalidDataException(); // TODO: Diagnostics : conversion error
            }

            for (int i = 0; i < path.Length; i++)
            {
                path[i] = Uri.UnescapeDataString(path[i]);
            }

            Namespace ns;
            ResourceName name;

            switch (path[1])
            {
                case "services":

                    ns = Namespace.Default;
                    name = new ResourceName(new ArraySegment<string>(path, 2, path.Length - 2));
                    break;

                case "servicesNS":

                    if (path.Length < 5)
                    {
                        throw new InvalidDataException(); // TODO: Diagnostics : conversion error
                    }

                    ns = new Namespace(user: path[2], app: path[3]);
                    name = new ResourceName(new ArraySegment<string>(path, 4, path.Length - 4));
                    break;

                default: throw new InvalidDataException(); // TODO: Diagnostics : conversion error
            }

            this.context = context;
            this.ns = ns;
            this.resourceName = name;

            this.initialized = true;
        }

        #endregion

        #region Types

        /// <summary>
        /// Represents information about a Splunk resource at a point in time.
        /// </summary>
        class Snapshot
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Snapshot"/> class.
            /// </summary>
            /// <param name="entry">
            /// An object representing a Splunk resource at a point in time.
            /// </param>
            public Snapshot(Context context, AtomEntry entry, Version generatorVersion = null)
            {
                Contract.Requires<ArgumentNullException>(entry != null);

                if (entry.Content == null)
                {
                    this.adapter = ExpandoAdapter.Empty;
                }
                else
                {
                    dynamic content = entry.Content as ExpandoObject;

                    if (content == null)
                    {
                        content = new ExpandoObject();
                        content.Value = entry.Content;
                    }

                    this.adapter = new ExpandoAdapter(content);
                }

                this.author = entry.Author;
                this.id = entry.Id;
                this.generatorVersion = generatorVersion;
                this.links = entry.Links;
                this.messages = new Message[0];
                this.published = entry.Published;
                this.name = entry.Title;
                this.updated = entry.Updated;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Snapshot"/> class.
            /// </summary>
            /// <param name="entry">
            /// An object representing a Splunk resource at a point in time.
            /// </param>
            public Snapshot(Context context, AtomFeed feed)
            {
                Contract.Requires<ArgumentNullException>(feed != null);

                this.adapter = ExpandoAdapter.Empty;
                this.author = feed.Author;
                this.id = feed.Id;
                this.generatorVersion = feed.GeneratorVersion;
                this.links = feed.Links;
                this.messages = new Message[0];
                this.published = feed.Updated;
                this.name = feed.Title;
                this.updated = feed.Updated;

                var list = new List<Resource>();
                
                foreach (var entry in feed.Entries)
                {
                    var resource = new Resource(context, entry, feed.GeneratorVersion);
                    list.Add(resource);
                }

                this.resources = list;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Snapshot"/> class.
            /// </summary>
            /// <param name="other">
            /// </param>
            /// <param name="content">
            /// </param>
            public Snapshot(Snapshot other, dynamic content)
            {
                Contract.Requires<ArgumentNullException>(other != null);

                if (content == null)
                {
                    this.adapter = ExpandoAdapter.Empty;
                }
                else if (content is ExpandoObject)
                {
                    this.adapter = new ExpandoAdapter(content);
                }
                else
                {
                    dynamic expandoObject = new ExpandoObject();
                    expandoObject.Value = content;
                    this.adapter = new ExpandoAdapter(expandoObject);
                }

                this.author = other.Author;
                this.id = other.Id;
                this.generatorVersion = other.GeneratorVersion;
                this.resources = other.Resources;
                this.links = other.Links;
                this.messages = other.Messages;
                this.published = other.Published;
                this.resources = other.Resources;
                this.name = other.Name;
                this.updated = other.Updated;
            }

            Snapshot()
            {
                this.adapter = ExpandoAdapter.Empty;
                this.author = null;
                this.id = null;
                this.generatorVersion = null;
                this.links = null;
                this.messages = new Message[0];
                this.published = DateTime.MinValue;
                this.resources = new Resource[0];
                this.name = null;
                this.updated = DateTime.MinValue;
            }

            #endregion

            #region Fields

            public static readonly Snapshot Missing = new Snapshot();

            #endregion

            #region Properties

            public ExpandoAdapter Adapter
            {
                get { return this.adapter; }
            }

            public string Author
            {
                get { return this.author; }
            }

            public IReadOnlyList<Resource> Resources
            {
                get { return this.resources; }
            }
            
            public Uri Id
            {
                get { return this.id; }
            }

            public Version GeneratorVersion
            {
                get { return this.generatorVersion; }
            }

            public IReadOnlyDictionary<string, Uri> Links
            {
                get { return this.links; }
            }

            public IReadOnlyList<Message> Messages
            {
                get { return this.messages; }
            }

            public string Name
            {
                get { return this.name; }
            }

            public DateTime Published
            {
                get { return this.published; }
            }

            public DateTime Updated
            {
                get { return this.updated; }
            }

            #endregion

            #region Privates/internals

            readonly ExpandoAdapter adapter;
            readonly string author;
            readonly Uri id;
            readonly IReadOnlyList<Resource> resources;
            readonly Version generatorVersion;
            readonly IReadOnlyDictionary<string, Uri> links;
            readonly IReadOnlyList<Message> messages;
            readonly string name;
            readonly DateTime published;
            readonly DateTime updated;

            #endregion
        }

        #endregion
    }
}
