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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a Splunk entity.
    /// </summary>
    /// <remarks>
    /// This is the base class for all Splunk entities.
    /// </remarks>
    public abstract class BaseEntity : Endpoint
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="ResourceEndpoint"/> instance.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// <param name="name">
        /// An object identifying a Splunk resource within <paramref name=
        /// "service"/>.<see cref="Namespace"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> or <paramref name="name"/> are <c>null</c>.
        protected internal BaseEntity(Service service, ResourceName name)
            : base(service.Context, service.Namespace, name)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity"/> 
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
        public BaseEntity(Context context, Namespace ns, ResourceName resourceName)
            : base(context, ns, resourceName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="collection">
        /// The <see cref="ResourceName"/> of an <see cref="EntityCollection&lt;TEntity&gt;"/>.
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
        public BaseEntity(Context context, Namespace ns, ResourceName collection, string name)
            : base(context, ns, new ResourceName(collection, name))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk response atom feed.
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
        public BaseEntity(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "BaseEntity"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        public BaseEntity()
        { }

        #endregion

        #region Properties

        public dynamic ResourceCache
        {
            get { return this.snapshot; }
        }

        /// <inheritdoc cref="ResourceCache.GeneratorVersion"/>
        public Version GeneratorVersion
        {
            get { return this.Snapshot.GeneratorVersion; }
        }

        /// <inheritdoc cref="ResourceCache.Id"/>
        public Uri Id
        {
            get { return this.Snapshot.Id; }
        }

        /// <inheritdoc cref="ResourceCache.Title"/>
        public string Title
        {
            get { return this.Snapshot.GetValue("Title"); }
        }

        /// <inheritdoc cref="ResourceCache.Updated"/>
        public DateTime Updated
        {
            get { return this.Snapshot.Updated; }
        }

        /// <summary>
        /// Gets an object representing the Splunk resource at the time it was
        /// last retrieved by the current <see cref="BaseEntity"/>.
        /// </summary>
        protected BaseResource Snapshot
        {
            get { return this.snapshot; }
            set { this.snapshot = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates a <see cref="BaseEntity"/> from a
        /// Splunk atom feed <see cref="Response"/>.
        /// </summary>
        /// <typeparam name="TResourceEndpoint">
        /// The type of <see cref="BaseEntity"/> to be created.
        /// </typeparam>
        /// <param name="response">
        /// An object representing a Splunk atom feed response.
        /// </param>
        /// <returns>
        /// The <see cref="BaseEntity"/> created.
        /// </returns>
        internal static async Task<TResourceEndpoint> CreateAsync<TResourceEndpoint>(Context context, Response response) 
            where TResourceEndpoint : BaseEntity, new()
        {
            var reader = response.XmlReader;
            TResourceEndpoint resourceEndpoint;

            reader.Requires(await reader.MoveToDocumentElementAsync("feed", "entry"));

            if (reader.Name == "entry")
            {
                var entry = new AtomEntry();

                await entry.ReadXmlAsync(reader);
                resourceEndpoint = new TResourceEndpoint();
                resourceEndpoint.Initialize(context, entry, new Version(0, 0));
            }
            else
            {
                var feed = new AtomFeed();

                await feed.ReadXmlAsync(reader);
                resourceEndpoint = new TResourceEndpoint();
                resourceEndpoint.Initialize(context, feed);
            }

            return resourceEndpoint;
        }

        /// <summary>
        /// Infrastructure. Initializes the current uninitialized <see cref=
        /// "BaseEntity"/>.
        /// class.
        /// </summary>
        /// <param name="context">
        /// /// An object representing a Splunk server session.
        /// </param>
        /// <param name="entry">
        /// An object representing a Splunk atom entry response.
        /// </param>
        /// <param name="generatorVersion">
        /// The version of the generator producing the <see cref="AtomFeed"/>
        /// feed containing <paramref name="entry"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/>, <paramref name="entry"/>, or <paramref 
        /// name="generatorVersion"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The current <see cref="BaseEntity"/> is already initialized.
        /// </exception>
        /// <remarks>
        /// This method may be called once to intialize a <see cref="BaseEntity"/>
        /// instantiated by the default constructor. Override this method to 
        /// provide special initialization code. Call this base method before 
        /// initialization is complete. 
        /// <note type="note">
        /// This method supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code.
        /// </note>
        /// </remarks>
        protected internal void Initialize(Context context, AtomEntry entry, Version generatorVersion)
        {
            Contract.Requires<ArgumentNullException>(generatorVersion != null);
            Contract.Requires<ArgumentNullException>(context != null);
            Contract.Requires<ArgumentNullException>(entry != null);

            this.ReconstructSnapshot(entry, generatorVersion);
            this.Initialize(context, this.Snapshot.Id);
        }

        /// <summary>
        /// Infrastructure. Initializes the current uninitialized <see cref=
        /// "BaseEntity"/>.
        /// class.
        /// </summary>
        /// <param name="context">
        /// /// An object representing a Splunk server session.
        /// </param>
        /// <param name="entry">
        /// An object representing a Splunk atom entry response.
        /// </param>
        /// <param name="generatorVersion">
        /// The version of the generator producing the <see cref="AtomFeed"/>
        /// feed containing <paramref name="entry"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/>, <paramref name="entry"/>, or <paramref 
        /// name="generatorVersion"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The current <see cref="BaseEntity"/> is already initialized.
        /// </exception>
        /// <remarks>
        /// This method may be called once to intialize a <see cref="BaseEntity"/>
        /// instantiated by the default constructor. Override this method to 
        /// provide special initialization code. Call this base method before 
        /// initialization is complete. 
        /// <note type="note">
        /// This method supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code.
        /// </note>
        /// </remarks>
        protected internal void Initialize(Context context, AtomFeed feed)
        {
            Contract.Requires<ArgumentNullException>(context != null);
            Contract.Requires<ArgumentNullException>(feed != null);

            this.ReconstructSnapshot(feed);
            this.Initialize(context, this.Snapshot.Id);
        }

        /// <summary>
        /// Infrastructure. Initializes the current uninitialized <see cref=
        /// "BaseEntity"/>.
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="resource">
        /// An object representing a Splunk resource.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="resource"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The current <see cref="BaseEntity"/> is already initialized.
        /// </exception>
        /// <remarks>
        /// This method may be called once to intialize a <see cref="BaseEntity"/>
        /// instantiated by the default constructor. Override this method to 
        /// provide special initialization code. Call this base method before 
        /// initialization is complete. 
        /// <note type="note">
        /// This method supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code.
        /// </note>
        /// </remarks>
        protected internal void Initialize(Context context, BaseResource resource)
        {
            Contract.Requires<ArgumentNullException>(resource != null);
            Contract.Requires<ArgumentNullException>(context != null);

            this.ReconstructSnapshot(resource);
            this.Initialize(context, this.Snapshot.Id);
        }

        /// <summary>
	    /// Updates the <see cref="Content"/> of the current <see cref=
        /// "BaseEntity"/>.
	    /// </summary>
	    /// <param name="entry">
	    /// A Splunk <see cref="AtomEntry"/>.
	    /// </param>
        protected void ReconstructSnapshot(AtomEntry entry, Version generatorVersion)
        {
            Contract.Requires<ArgumentNullException>(generatorVersion != null);
            Contract.Requires<ArgumentNullException>(entry != null);
            this.snapshot = new BaseResource(entry, generatorVersion);
        }

        /// <summary>
        /// Updates the <see cref="Snapshot"/> of the current <see cref=
        /// "BaseEntity"/> in derived types.
        /// </summary>
        /// <param name="feed">
        /// A Splunk <see cref="AtomFeed"/>.
        /// </param>
        /// <remarks>
        /// Derived types must implement this method.
        /// </remarks>
        protected abstract void ReconstructSnapshot(AtomFeed feed);

        /// <summary>
        /// Updates the <see cref="Content"/> of the current <see cref=
        /// "BaseEntity"/> in derived types.
        /// </summary>
        /// <param name="resource">
        /// An object representing a Splunk resource.
        /// </param>
        /// <remarks>
        /// Derived types must implement this method.
        /// </remarks>
        protected abstract void ReconstructSnapshot(BaseResource resource);

        /// <summary>
        /// Asynchronously updates the <see cref="Content"/> of the current <see
        /// cref="BaseEntity"/>
        /// </summary>
        /// <param name="response">
        /// A Splunk atom feed <see cref="Response"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        protected internal async Task<bool> ReconstructSnapshotAsync(Response response)
        {
            Contract.Requires<ArgumentNullException>(response != null);
            var feed = new AtomFeed();

            await feed.ReadXmlAsync(response.XmlReader);
            this.ReconstructSnapshot(feed);

            return true;
        }

        #endregion

        #region Privates/internals

        volatile BaseResource snapshot = Splunk.Client.BaseResource.Missing;

        #endregion
    }
}
