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
        /// Initializes a new instance of the <see cref="Entity&lt;TEntity&gt;"/> 
        /// class as specified by <paramref name="context"/>, <paramref name="ns"/>
        /// and "<paramref name="resourceName"/>.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="resourceName">
        /// An object identifying a Splunk resource within <paramref name="ns"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/>, <paramref name="ns"/>, or <paramref name=
        /// "resourceName"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ns"/> is not specific.
        /// </exception>
        protected Entity(Context context, Namespace ns, ResourceName resourceName)
            : base(context, ns, resourceName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity&lt;TEntity&gt;"/> 
        /// class as specified by <paramref name="context"/>, <paramref name="ns"/>,
        /// <paramref name="collection"/>, and <paramref name="entity"/>.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="collection">
        /// The <see cref="ResourceName"/> of an <see cref="EntityCollection&lt;TCollection, TEntity&gt;"/>.
        /// </param>
        /// <param name="entity">
        /// The name of an entity within <paramref name="collection"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="entity"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/>, <paramref name="ns"/>, or <paramref 
        /// name="collection"/>, or <paramref name="entity"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ns"/> is not specific.
        /// </exception>
        protected Entity(Context context, Namespace ns, ResourceName collection, string entity)
            : this(context, ns, new ResourceName(collection, entity))
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "Resource&lt;TResource&gt;"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        public Entity()
        { }

        #endregion

        #region Properties backed by AtomEntry

        /// <summary>
        /// Gets the author of the current <see cref="Entity&lt;TEntity&gt;"/>.
        /// </summary>
        /// <remarks>
        /// <c>"Splunk"</c> is the author of all <see cref="Entity&lt;TEntity&gt;"/> 
        /// and <see cref="EntityCollection&lt;TCollection, TEntity&gt;"/>
        /// instances.
        /// </remarks>
        public string Author
        { 
            get { return this.Snapshot.Author; }
        }

        /// <summary>
        /// Gets the Splunk management URI for accessing the current <see cref=
        /// "Entity&lt;TEntity&gt;"/>.
        /// </summary>
        public Uri Id
        { 
            get { return this.Snapshot.Id; } 
        }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyDictionary<string, Uri> Links
        { 
            get { return this.Snapshot.Links; } 
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Published
        {
            get { return this.Snapshot.Published; }
        }

        /// <summary>
        /// Gets the date that <see cref="Id"/> was implemented in Splunk.
        /// </summary>
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
        /// Asynchronously retrieves a fresh copy of the current <see cref=
        /// "Entity&lt;TEntity&gt;"/> that contains all changes to it since it
        /// was last retrieved.
        /// </summary>
        /// <returns></returns>
        public virtual async Task GetAsync()
        {
            using (var response = await this.Context.GetAsync(this.Namespace, this.ResourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                await this.UpdateSnapshotAsync(response);
            }
        }

        /// <summary>
        /// Gets a property value from the <see cref="ExpandoAdapter"/>
        /// underlying the current <see cref="Entity&lt;TEntity&gt;"/>.
        /// </summary>
        /// <param name="name">
        /// Property name.
        /// </param>
        /// <returns>
        /// Property value or <c>null</c>, if property does not exist.
        /// </returns>
        protected dynamic GetValue(string name)
        {
            return this.Snapshot.Adapter.GetValue(name);
        }

        /// <summary>
        /// Gets a converted property value from the <see cref="ExpandoAdapter"/>
        /// underlying the current <see cref="Entity&lt;TEntity&gt;"/>.
        /// </summary>
        /// <param name="name">
        /// Property name.
        /// </param>
        /// <param name="valueConverter">
        /// A value converter for converting the property identified by
        /// <paramref name="name"/>.
        /// </param>
        /// <returns>
        /// Property value or <c>null</c>, if property does not exist.
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// The conversion failed.
        /// </exception>
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
        /// <param name="entry">
        /// 
        /// </param>
        protected internal override void Initialize(Context context, AtomEntry entry)
        {
            this.snapshot = new EntitySnapshot(entry);
            base.Initialize(context, entry);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response">
        /// 
        /// </param>
        /// <returns></returns>
        protected virtual async Task UpdateSnapshotAsync(Response response)
        {
            var feed = new AtomFeed();
            await feed.ReadXmlAsync(response.XmlReader);

            if (feed.Entries.Count != 1)
            {
                throw new InvalidDataException(); // TODO: Diagnostics
            }

            this.Snapshot = new EntitySnapshot(feed.Entries[0]);
        }

        #endregion

        #region Privates

        volatile EntitySnapshot snapshot = EntitySnapshot.Missing;

        #endregion
    }
}
