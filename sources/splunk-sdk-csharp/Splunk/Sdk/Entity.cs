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

    public class Entity<TEntity> where TEntity : Entity<TEntity>, new()
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public Entity()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="namespace"></param>
        /// <param name="collection"></param>
        /// <param name="title"></param>
        protected Entity(Context context, Namespace @namespace, ResourceName collection, string title)
        {
            Contract.Requires<ArgumentNullException>(@namespace != null, "namespace");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(title), "name");
            Contract.Requires<ArgumentException>(collection != null, "collection");
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires(@namespace.IsSpecific);

            this.Context = context;

            this.Namespace = @namespace;
            this.Collection = collection;
            this.Title = title;

            this.ResourceName = new ResourceName(this.Collection, this.Title);
        }

        #endregion

        #region Properties that are stable for the lifetime of an instance

        /// <summary>
        /// Gets the path to the collection containing the current <see cref=
        /// "Entity"/>.
        /// </summary>
        public ResourceName Collection
        { get; internal set; }

        /// <summary>
        /// Gets the <see cref="Context"/> instance for the current <see cref=
        /// "Entity"/>.
        /// </summary>
        public Context Context
        { get; internal set; }

        /// <summary>
        /// Gets the namespace containing the current <see cref="Entity"/>.
        /// </summary>
        public Namespace Namespace
        { get; private set; }

        /// <summary>
        /// Gets the resource name of the current <see cref="Entity"/>.
        /// </summary>
        /// <remarks>
        /// The resource name is the concatenation of <see cref=
        /// "Entity.Collection"/> and <see cref="Entity.Title"/>.
        /// </remarks>
        public ResourceName ResourceName
        { get; private set; }

        /// <summary>
        /// Gets the title of this <see cref="Entity"/>.
        /// </summary>
        public string Title
        { get; private set; }

        #endregion

        #region Properties backed by AtomEntry

        public string Author
        { 
            get { return this.AtomEntry == null ? null : this.AtomEntry.Author; } 
        }

        public Uri Id
        { 
            get { return this.AtomEntry == null ? null : this.AtomEntry.Id; } 
        }

        public IReadOnlyDictionary<string, Uri> Links
        { 
            get { return this.AtomEntry == null ? null : this.AtomEntry.Links; } 
        }

        public DateTime Updated
        { 
            get { return this.AtomEntry == null ? DateTime.MinValue : this.AtomEntry.Updated; } 
        }

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
            get
            {
                if (this.data == null)
                {
                    throw new InvalidOperationException(); // TODO: diagnostics
                }
                return this.data.Adapter;
            }
        }

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
            return this.AtomEntry == null ? null : this.AtomEntry.Title;
        }

        /// <summary>
        /// Refreshes the cached state of the current <see cref=
        /// "Entity<TEntity>"/>.
        /// </summary>
        public void Update()
        {
            this.UpdateAsync().Wait();
        }

        /// <summary>
        /// Refreshes the cached state of the current <see cref=
        /// "Entity<TEntity>"/>.
        /// </summary>
        public async Task UpdateAsync()
        {
            // TODO: Parmeterized retry logic

            RequestException requestException = null;

            // FJR: I assume the retry logic is for jobs, since nothing else requires this. I suggest moving it
            // into Job. Also, it's insufficient. If you're just trying to get some state, this will do it, but
            // as of Splunk 6, getting a 200 and content back does not imply you have all the fields. For pivot
            // support, they're now shoving fields in as they become ready, so you have to wait until the dispatchState
            // field of the Atom entry reaches a certain point.
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
                        this.data = new Data(entry);
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

        public override string ToString()
        {
            return string.Join("/", this.Context.ToString(), this.Namespace.ToString(), this.Collection.ToString(), this.Title);
        }

        #endregion

        #region Privates/internals

        AtomEntry AtomEntry
        { 
            get { return this.data == null ? null : this.data.Entry; } 
        }

        internal static TEntity CreateEntity(Context context, Namespace @namespace, ResourceName collection, AtomEntry atomEntry)
        {
            // TODO: Entity<TEntity> derivatives should provide their ResourceName property. 
            // CreateEntity should not require it (?)

            Contract.Requires<ArgumentNullException>(collection != null, "collection");
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(atomEntry != null, "entry");

            var entity = new TEntity();

            entity.data = new Data(atomEntry); // must be set before entity.Title
            entity.Title = entity.GetTitle();

            entity.Context = context;
            entity.Collection = collection;
            entity.ResourceName = new ResourceName(collection, entity.Title);

            dynamic content = atomEntry.Content;

            if ((content as ExpandoObject) == null)
            {
                entity.Namespace = @namespace;
            }
            else
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
                    entity.Namespace = new Namespace(acl.Owner, acl.App);
                }
                catch (ArgumentException e)
                {
                    throw new InvalidDataException("", e); // TODO: Diagnostics
                }
            }

            return entity;
        }

        #endregion

        #region Privates

        volatile Data data;

        #endregion

        #region Types

        class Data
        {
            public Data(AtomEntry entry)
            {
                if (entry.Content == null)
                {
                    this.adapter = ExpandoAdapter.Empty;
                }
                else
                {
                    this.adapter = entry.Content as ExpandoAdapter;
                    
                    if (this.adapter == null)
                    {
                        dynamic content = new ExpandoObject();
                        content.Value = entry.Content;
                        this.adapter = new ExpandoAdapter(content);
                    }
                }

                this.entry = entry;
            }

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
