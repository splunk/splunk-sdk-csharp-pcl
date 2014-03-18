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
// [ ] Pick up standard properties from AtomEntry on Update, not just AtomEntry.Content
//     See [Splunk responses to REST operations](http://goo.gl/tyXDfs).
// [ ] Check for HTTP Status Code 204 (No Content) and empty atoms in 
//     Entity<TEntity>.UpdateAsync.
// [O] Contracts
//
// [ ] Documentation
//
// [ ] Remove Entity<TEntity>.Invalidate method
//     FJR: This gets called when we set the record value. Add a comment saying what it's
//     supposed to do when it's overridden.
//     DSN: I've adopted an alternative method for getting strongly-typed values. See, for
//     example, Job.DispatchState or ServerInfo.Guid.

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

    public class Entity<TEntity> : ExpandoAdapter where TEntity : Entity<TEntity>, new()
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

        #region Properties

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

        public DateTime Published
        {
            get { return this.AtomEntry == null ? DateTime.MinValue : this.AtomEntry.Published; }
        }

        public DateTime Updated
        {
            get { return this.AtomEntry == null ? DateTime.MinValue : this.AtomEntry.Updated; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the title of the current <see cref="Entity"/> from its atom entry.
        /// </summary>
        /// <returns>
        /// The title of the current <see cref="Entity"/>.
        /// </returns>
        /// <remarks>
        /// This method is overridden by the <see cref="Job"/> class. Its title
        /// comes from the <c>Sid</c> property, not the <c>Title</c> property of
        /// <see cref="Entity.Record"/>.
        /// </remarks>
        protected virtual string GetTitle()
        {
            return GetValue("Title", StringConverter.Instance);
        }

        /// <summary>
        /// Refreshes the cached state of the current <see cref="Entity"/>.
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

                    using (var response = await Response.CreateAsync(await this.Context.GetAsync(this.Namespace, this.ResourceName)))
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

                        if (reader.Name == "feed")
                        {
                            await response.XmlReader.ReadToFollowingAsync("entry");
                        }

                        if (reader.Name != "entry")
                        {
                            throw new InvalidDataException(); // TODO: Diagnostics
                        }

                        var atomEntry = new AtomEntry();
                        
                        await atomEntry.ReadXmlAsync(reader);
                        this.AtomEntry = atomEntry;
                        this.ExpandoObject = atomEntry.Content;
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
        { get; set; }

        internal static TEntity CreateEntity(Context context, ResourceName collection, AtomEntry atomEntry)
        {
            Contract.Requires<ArgumentNullException>(collection != null, "collection");
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(atomEntry != null, "entry");

            dynamic content = atomEntry.Content;

            var entity = new TEntity()
            {
                AtomEntry = atomEntry,
                Collection = collection,
                Context = context,
                ExpandoObject = content,
            };

            entity.Title = entity.GetTitle();
            entity.ResourceName = new ResourceName(collection, entity.Title);
            entity.Namespace = new Namespace(content.Eai.Acl.Owner, content.Eai.Acl.App);

            return entity;
        }

        #endregion
    }
}
