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
//// [ ] Check for HTTP Status Code 204 (No Content) and empty atoms in 
////     Entity<TEntity>.UpdateAsync.
////
//// [O] Contracts
////
//// [O] Documentation
////
//// [X] Pick up standard properties from AtomEntry on Update, not just AtomEntry.Content
////     See [Splunk responses to REST operations](http://goo.gl/tyXDfs).
////
//// [X] Remove Entity<TEntity>.Invalidate method
////     FJR: This gets called when we set the record value. Add a comment saying what it's
////     supposed to do when it's overridden.
////     DSN: I've adopted an alternative method for getting strongly-typed values. See, for
////     example, Job.DispatchState or ServerInfo.Guid.

namespace Splunk.Client
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

    /// <summary>
    /// Provides a base class for representing a Splunk entity.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The entity type inheriting from this class.
    /// </typeparam>
    public class Entity<TEntity> : Resource<TEntity> where TEntity : Entity<TEntity>, new()
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity<TEntity>"/> 
        /// class as specified by <see cref="context"/>, <see cref="namespace"/>
        /// and "<see cref="resourceName"/>.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="namespace">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="resourceName">
        /// An object identifying a Splunk resource within <see cref="namespace"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <see cref="context"/>, <see cref="namespace"/>, or <see cref=
        /// "resourceName"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="namespace"/> is not specific.
        /// </exception>
        protected Entity(Context context, Namespace @namespace, ResourceName resourceName)
            : base(context, @namespace, resourceName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity<TEntity>"/> 
        /// class as specified by <see cref="context"/>, <see cref="namespace"/>,
        /// and resource <see cref="collection"/> and <see cref="title"/>.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="namespace">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="collection">
        /// An object identifying a Splunk resource collection within <see 
        /// cref="namespace"/>.
        /// </param>
        /// <param name="entity">
        /// The name of a resource within <see cref="collection"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <see cref="entity"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <see cref="context"/>, <see cref="namespace"/>, or <see cref=
        /// "collection"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="namespace"/> is not specific.
        /// </exception>
        protected Entity(Context context, Namespace @namespace, ResourceName collection, string entity)
            : this(context, @namespace, new ResourceName(collection, entity))
        { }

        public Entity()
        { }

        #endregion

        #region Properties backed by AtomEntry

        public string Author
        { 
            get { return this.Snapshot.Author; }
        }

        public Uri Id
        { 
            get { return this.Snapshot.Id; } 
        }

        public IReadOnlyDictionary<string, Uri> Links
        { 
            get { return this.Snapshot.Links; } 
        }

        public DateTime Published
        {
            get { return this.Snapshot.Published; }
        }

        public DateTime Updated
        { 
            get { return this.Snapshot.Updated; }
        }

        #region Protected properties

        /// <summary>
        /// 
        /// </summary>
        protected EntitySnapshot Snapshot
        {
            get { return this.snapshot; }
            set { this.snapshot = value; }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Refreshes the cached state of the current <see cref="Entity<TEntity>"/>.
        /// </summary>
        public virtual async Task GetAsync()
        {
            using (var response = await this.Context.GetAsync(this.Namespace, this.ResourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                await this.UpdateSnapshotAsync(response);
            }
        }

        protected dynamic GetValue(string name)
        {
            return this.Snapshot.Adapter.GetValue(name);
        }

        protected TValue GetValue<TValue>(string name, ValueConverter<TValue> valueConverter)
        {
            return this.Snapshot.Adapter.GetValue(name, valueConverter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="namespace">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="collection">
        /// </param>
        /// <param name="entry">
        /// </param>
        protected internal override void Initialize(Context context, AtomEntry entry)
        {
            this.snapshot = new EntitySnapshot(entry);
            base.Initialize(context, entry);
        }

        protected async Task UpdateSnapshotAsync(Response response)
        {
            var feed = new AtomFeed();
            await feed.ReadXmlAsync(response.XmlReader);

            if (feed.Entries.Count != 1)
            {
                throw new InvalidDataException();  // TODO: Diagnostics
            }

            this.Snapshot = new EntitySnapshot(feed.Entries[0]);
        }

        #endregion

        #region Privates

        volatile EntitySnapshot snapshot = EntitySnapshot.Missing;

        #endregion
    }
}
