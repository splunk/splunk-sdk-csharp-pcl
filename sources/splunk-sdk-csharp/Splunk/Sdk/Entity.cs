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
// [ ] Check for HTTP Status Code 204 (No Content) and empty atoms in 
//     Entity<TEntity>.UpdateAsync.
//
// [O] Contracts
//
// [O] Documentation
//
// [X] Pick up standard properties from AtomEntry on Update, not just AtomEntry.Content
//     See [Splunk responses to REST operations](http://goo.gl/tyXDfs).
//
// [X] Remove Entity<TEntity>.Invalidate method
//     FJR: This gets called when we set the record value. Add a comment saying what it's
//     supposed to do when it's overridden.
//     DSN: I've adopted an alternative method for getting strongly-typed values. See, for
//     example, Job.DispatchState or ServerInfo.Guid.

namespace Splunk.Sdk
{
    using Microsoft.CSharp.RuntimeBinder;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using System.Xml;

    public class Entity<TEntity> : Resource<TEntity> where TEntity : Entity<TEntity>, new()
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <param name="namespace">
        /// </param>
        /// <param name="collection">
        /// </param>
        /// <param name="title">
        /// </param>
        protected Entity(Context context, Namespace @namespace, ResourceName collection, string title)
            : base(context, @namespace, new ResourceName(collection, title))
        {
            Contract.Requires<ArgumentNullException>(@namespace != null, "namespace");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(title), "name");
            Contract.Requires<ArgumentException>(collection != null, "collection");
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires(@namespace.IsSpecific);
        }

        /// <summary>
        /// 
        /// </summary>
        public Entity()
        {
            this.data = DataObject.Missing;
        }

        #endregion

        #region Properties that are stable for the lifetime of an instance

        /// <summary>
        /// Gets the path to the collection containing the current <see cref=
        /// "Entity"/>.
        /// </summary>
        public ResourceName Collection
        { get; internal set; }

        #endregion

        #region Properties backed by AtomEntry

        public override string Author
        { 
            get { return this.data.Entry == null ? null : this.data.Entry.Author; }
        }

        public override Uri Id
        { 
            get { return this.data.Entry == null ? null : this.data.Entry.Id; } 
        }

        public override IReadOnlyDictionary<string, Uri> Links
        { 
            get { return this.data.Entry == null ? null : this.data.Entry.Links; } 
        }

        public override DateTime Updated
        { 
            get { return this.data.Entry == null ? DateTime.MinValue : this.data.Entry.Updated; } 
        }

        #region Protected properties

        /// <summary>
        /// Gets the <see cref="ExpandoAdapter"/> representing the content of
        /// the current <see cref="Entity<TEntity>"/>.
        /// </summary>
        /// <remarks>
        /// Use this property to map content elements to strongly typed 
        /// properties.
        /// </remarks>
        protected ExpandoAdapter Content
        {
            get { return this.data.Adapter; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected DataObject Data
        {
            get { return this.data; }
            set { this.data = value; }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Gets the title of the current <see cref="Entity<TEntity>"/>.
        /// </summary>
        /// <returns>
        /// The title of the current <see cref="Entity<TEntity>"/>.
        /// </returns>
        /// <remarks>
        /// This method is overridden by the <see cref="Job"/> class. Its title
        /// comes from the <c>Sid</c> property, not the <c>Title</c> property of
        /// <see cref="Entity<TEntity>.Record"/>.
        /// </remarks>
        protected virtual string GetTitle()
        {
            return this.data.Entry == null ? null : this.data.Entry.Title;
        }

        /// <summary>
        /// Refreshes the cached state of the current <see cref=
        /// "Entity<TEntity>"/>.
        /// </summary>
        public override async Task GetAsync()
        {
            // TODO: This retry logic is for jobs. Parmeterize it and move it into the Job class

            // FJR: I assume the retry logic is for jobs, since nothing else requires this. I suggest moving it
            // into Job. Also, it's insufficient. If you're just trying to get some state, this will do it, but
            // as of Splunk 6, getting a 200 and content back does not imply you have all the fields. For pivot
            // support, they're now shoving fields in as they become ready, so you have to wait until the dispatchState
            // field of the Atom entry reaches a certain point.

            RequestException requestException = null;

            for (int i = 3; i > 0; --i)
            {
                try
                {
                    // Guarantee: unique result because entities have specific namespaces

                    using (var response = await this.Context.GetAsync(this.Namespace, this.ResourceName))
                    {
                        if (response.Message.StatusCode == HttpStatusCode.NoContent)
                        {
                            throw new RequestException(response.Message, new Message(MessageType.Warning, string.Format("Resource '{0}/{1}' is not ready.", this.Namespace, this.ResourceName)));
                        }

                        if (!response.Message.IsSuccessStatusCode)
                        {
                            throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                        }

                        var reader = response.XmlReader;
                        await reader.ReadAsync();

                        if (reader.NodeType == XmlNodeType.XmlDeclaration)
                        {
                            await response.XmlReader.ReadAsync();
                        }

                        if (reader.NodeType != XmlNodeType.Element)
                        {
                            throw new InvalidDataException(); // TODO: Diagnostics
                        }

                        AtomEntry entry;

                        if (reader.Name == "feed")
                        {
                            AtomFeed feed = new AtomFeed();

                            await feed.ReadXmlAsync(reader);
                            int count = feed.Entries.Count;

                            foreach (var feedEntry in feed.Entries)
                            {
                                string id = feedEntry.Title;
                                id.Trim();
                            }

                            if (feed.Entries.Count != 1)
                            {
                                throw new InvalidDataException(); // TODO: Diagnostics
                            }

                            entry = feed.Entries[0];
                        }
                        else
                        {
                            entry = new AtomEntry();
                            await entry.ReadXmlAsync(reader);
                        }

                        // TODO: Check entry type (?)
                        this.data = new DataObject(entry);
                    }

                    return;
                }
                catch (RequestException e)
                {
                    if (e.StatusCode != System.Net.HttpStatusCode.NoContent)
                    {
                        throw;
                    }
                    requestException = e;
                }
                await Task.Delay(500);
            }

            throw requestException;
        }

        #endregion

        #region Privates/internals

        public override void Initialize(Context context, Namespace @namespace, ResourceName collection, object entry)
        {
            AtomEntry atomEntry = entry as AtomEntry;

            if (atomEntry == null)
            {
                throw new ArgumentException("Expected non-null entry of type AtomEntry.");
            }

            this.data = new DataObject(atomEntry); // must be set before entity.Title
            dynamic content = atomEntry.Content;

            if ((content as ExpandoObject) != null)
            {
                dynamic acl;

                try
                {
                    acl = content.Eai.Acl;
                }
                catch (RuntimeBinderException e)
                {
                    throw new InvalidDataException("", e); // TODO: Diagnostics
                }

                try
                {
                    @namespace = new Namespace(acl.Owner, acl.App);
                }
                catch (ArgumentException e)
                {
                    throw new InvalidDataException("", e); // TODO: Diagnostics
                }
            }

            var resourceName = new ResourceName(collection, this.GetTitle());
            base.Initialize(context, @namespace, resourceName, null /* unused */);
        }

        #endregion

        #region Privates

        volatile DataObject data;

        #endregion

        #region Types

        protected sealed class DataObject
        {
            public DataObject(AtomEntry entry)
            {
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

                this.entry = entry;
            }

            DataObject()
            {
                this.adapter = ExpandoAdapter.Empty;
            }

            public static readonly DataObject Missing = new DataObject();

            public ExpandoAdapter Adapter
            { 
                get { return this.adapter; } 
            }

            public AtomEntry Entry
            { 
                get { return this.entry; } 
            }

            readonly ExpandoAdapter adapter;
            readonly AtomEntry entry;
        }

        #endregion
    }
}
