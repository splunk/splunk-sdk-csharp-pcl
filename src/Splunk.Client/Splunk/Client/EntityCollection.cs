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
//// [ ] Reload method as per <a href="http://goo.gl/TDthxd">Accessing Splunk
////     resource</a>, Other actions for Splunk REST API endpoints.
//// [ ] Remove EntityCollection.args and put optional arguments on the GetAsync
////     method (?) args does NOT belong on the constructor. One difficulty:
////     not all collections take arguments. Examples: ConfigurationCollection
////     and IndexCollection.
//// [O] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a base class for representing a collection of Splunk entities.
    /// </summary>
    /// <typeparam name="TCollection">
    /// The entity collection type inheriting from this class.
    /// </typeparam>
    /// <typeparam name="TEntity">
    /// The type of the entity in <typeparamref name="TCollection"/>.
    /// </typeparam>
    public abstract class EntityCollection<TCollection, TEntity> : Resource<TCollection>, IReadOnlyList<TEntity> 
        where TCollection : EntityCollection<TCollection, TEntity>, new() 
        where TEntity : Resource<TEntity>, new()
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref=
        /// "EntityCollection&lt;TCollection, TEntity&gt;"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="resource">
        /// 
        /// </param>
        /// <param name="args">
        /// 
        /// </param>
        internal EntityCollection(Context context, Namespace ns, ResourceName resource, IEnumerable<Argument> 
            args = null)
            : base(context, ns, resource)
        {
            this.args = args;
        }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "EntityCollection&lt;TCollection, TEntity&gt;"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. 
        /// </remarks>
        public EntityCollection()
        { }

        #endregion

        #region Properties

        #region AtomFeed properties

        /// <summary>
        /// Gets the author of the current <see cref="EntityCollection&lt;
        /// TCollection, TEntity&gt;"/>.
        /// </summary>
        /// <remarks>
        /// <c>"Splunk"</c> is the author of all <see cref="Entity&lt;TEntity&gt;"/> 
        /// and <see cref="EntityCollection&lt;TCollection, TEntity&gt;"/>
        /// instances.
        /// </remarks>
        public string Author
        {
            get { return this.data.Author; }
        }

        /// <summary>
        /// Gets the version of the Atom Feed generator that produced the
        /// current <see cref="EntityCollection&lt;TCollection, TEntity&gt;"/>.
        /// </summary>
        public Version GeneratorVersion
        {
            get { return this.data.GeneratorVersion; }
        }

        /// <summary>
        /// Gets the Splunk management URI for accessing the current <see cref=
        /// "EntityCollection&lt;TCollection, TEntity&gt;"/>.
        /// </summary>
        public Uri Id
        {
            get { return this.data.Id; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyDictionary<string, Uri> Links
        {
            get { return this.data.Links; }
        }

        /// <summary>
        /// Gets the list of info, warning, or error messages associated with 
        /// the operation that produced the current <see cref="EntityCollection
        /// &lt;TCollection, TEntity&gt;"/>.
        /// </summary>
        /// <remarks>
        /// Not all operations produce messages.
        /// </remarks>
        public IReadOnlyList<Message> Messages
        {
            get { return this.data.Messages; }
        }

        /// <summary>
        /// Gets the pagination attributes for the current <see cref=
        /// "EntityCollection&lt;TCollection, TEntity&gt;"/>.
        /// </summary>
        public Pagination Pagination
        {
            get { return this.data.Pagination; }
        }

        /// <summary>
        /// Gets the human readable name of the current <see cref="EntityCollection
        /// &lt;TCollection, TEntity&gt;"/>.
        /// </summary>
        /// <remarks>
        /// This value is derived from the last segment of <see cref="Id"/>;
        /// </remarks>
        public string Title
        {
            get { return this.data.Title; }
        }

        /// <summary>
        /// Gets the date that <see cref="Id"/> was implemented in Splunk.
        /// </summary>
        public DateTime Updated
        {
            get { return this.data.Updated; }
        }

        #endregion

        #region IReadOnlyList<TEntity> properties

        /// <summary>
        /// Gets the entity at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the entity to get.
        /// </param>
        /// <returns>
        /// An object representing the entity at <paramref name="index"/>.
        /// </returns>
        public TEntity this[int index]
        {
            get { return this.data.Entities[index]; }
        }

        /// <summary>
        /// Gets the number of entities in the current <see cref=
        /// "EntityCollection&lt;TCollection, TEntity&gt;"/>.
        /// </summary>
        public int Count
        {
            get { return this.data.Entities.Count; }
        }

        #endregion

        #endregion

        #region Methods

        #region Request-related methods

        /// <summary>
        /// Infrastructure. Initializes the current <see cref="EntityCollection
        /// &lt;TCollection, TEntity&gt;"/>.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="entry">
        /// An atom entry containing metadata, plus the content for the current
        /// <see cref="EntityCollection&lt;TCollection, TEntity&gt;"/>.
        /// </param>
        /// <remarks>
        /// Override this method to provide special initialization code. Call
        /// the base implementation before initialization is complete. This
        /// method supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        protected internal override void Initialize(Context context, AtomEntry entry)
        {
            this.data = new DataCache(entry);
            base.Initialize(context, entry);
        }

        /// <summary>
        /// Asynchronously retrieves a fresh copy of the entities in the 
        /// current <see cref="EntityCollection&lt;TCollection, TEntity&gt;"/> 
        /// that contains all changes to it since it was last retrieved.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public virtual async Task GetAsync()
        {
            using (Response response = await this.Context.GetAsync(this.Namespace, this.ResourceName, this.args))
            {
                await response.EnsureStatusCodeAsync(System.Net.HttpStatusCode.OK);
                var feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader);

                this.data = new DataCache(this.Context, feed);
            }
        }

        #endregion

        #region IReadOnlyList<TEntity> methods

        /// <summary>
        /// Gets an enumerator that iterates through the current <see cref=
        /// "EntityCollection&lt;TCollection, TEntity&gt;"/>.
        /// </summary>
        /// <returns>
        /// An object for iterating through the current <see cref=
        /// "EntityCollection&lt;TCollection, TEntity&gt;"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator that iterates through the current <see cref=
        /// "EntityCollection&lt;TCollection, TEntity&gt;"/>.
        /// </summary>
        /// <returns>
        /// An object for iterating through the current <see cref=
        /// "EntityCollection&lt;TCollection, TEntity&gt;"/>.
        /// </returns>
        public IEnumerator<TEntity> GetEnumerator()
        {
            if (this.data == null)
            {
                throw new InvalidOperationException();
            }
            return this.data.Entities.GetEnumerator();
        }

        #endregion

        #endregion

        #region Privates

        volatile DataCache data = DataCache.Missing;
        readonly IEnumerable<Argument> args;

        #endregion

        #region Types

        class DataCache
        {
            #region Constructors

            public DataCache(AtomEntry entry)
            {
                this.author = entry.Author;
                this.id = entry.Id;
                this.generatorVersion = null; // TODO: figure out a way to inherit the enclosing feed's generator version or is it in each entry too?
                this.links = entry.Links;
                this.messages = new List<Message>(); // TODO: does an entry contain messages?
                this.pagination = Pagination.Empty;
                this.title = entry.Title;
                this.updated = entry.Updated;

                this.entities = new List<TEntity>();
            }

            public DataCache(Context context, AtomFeed feed)
            {
                this.author = feed.Author;
                this.id = feed.Id;
                this.generatorVersion = feed.GeneratorVersion;
                this.links = feed.Links;
                this.messages = feed.Messages;
                this.pagination = feed.Pagination;
                this.title = feed.Title;
                this.updated = feed.Updated;

                var entities = new List<TEntity>(feed.Entries.Count);

                foreach (var entry in feed.Entries)
                {
                    TEntity entity = new TEntity();

                    entity.Initialize(context, entry);
                    entities.Add(entity);
                }

                this.entities = entities;
            }

            DataCache()
            {
                this.author = null;
                this.id = null;
                this.generatorVersion = null;
                this.links = new Dictionary<string, Uri>();
                this.messages = new List<Message>();
                this.pagination = Pagination.Empty;
                this.title = null;
                this.updated = DateTime.MinValue;

                this.entities = new List<TEntity>();
            }

            #endregion

            #region Fields

            public static readonly DataCache Missing = new DataCache();

            #endregion

            #region Properties

            public string Author
            {
                get { return this.author; }
            }

            public Uri Id
            {
                get { return this.id; }
            }

            public IReadOnlyList<TEntity> Entities
            { 
                get { return this.entities; } 
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

            public Pagination Pagination
            {
                get { return this.pagination; }
            }

            public string Title
            {
                get { return this.title; }
            }

            public DateTime Updated
            {
                get { return this.updated; }
            }

            #endregion

            #region Privates

            readonly IReadOnlyList<TEntity> entities;
            readonly string author;
            readonly Uri id;
            readonly Version generatorVersion;
            readonly IReadOnlyDictionary<string, Uri> links;
            readonly IReadOnlyList<Message> messages;
            readonly Pagination pagination;
            readonly string title;
            readonly DateTime updated;

            #endregion
        }

        #endregion
    }
}
