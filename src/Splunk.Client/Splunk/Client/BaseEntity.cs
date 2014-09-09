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
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a Splunk entity.
    /// </summary>
    /// <remarks>
    /// This is the base class for all Splunk entities.
    /// </remarks>
    /// <typeparam name="TResource">
    /// Type of the resource.
    /// </typeparam>
    /// <seealso cref="T:Splunk.Client.Endpoint"/>
    [ContractClass(typeof(BaseEntityContract<>))]
    public abstract class BaseEntity<TResource> : Endpoint where TResource : BaseResource, new()
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="BaseEntity&lt;TResource&gt;"/> instance.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// </param>
        /// <param name="name">
        /// An object identifying a Splunk resource within
        /// <paramref name= "service"/>.<see cref="Namespace"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> or <paramref name="name"/> are <c>null</c>.
        /// </exception>
        protected BaseEntity(Service service, ResourceName name)
            : base(service.Context, service.Namespace, name)
        {
            Contract.Requires<ArgumentNullException>(service != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity&lt;TResource&gt;"/>
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
        /// <paramref name="context"/>, <paramref name="ns"/>, or
        /// <paramref name= "resourceName"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ns"/> is not specific.
        /// </exception>
        protected BaseEntity(Context context, Namespace ns, ResourceName resourceName)
            : base(context, ns, resourceName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity&lt;TResource&gt;"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="collection">
        /// The <see cref="ResourceName"/> of an <see cref="EntityCollection&lt;TEntity,TResource&gt;"/>.
        /// </param>
        /// <param name="name">
        /// An object identifying a Splunk resource within the resource identfied by <paramref name="collection"/>.
        /// </param>
        protected BaseEntity(Context context, Namespace ns, ResourceName collection, string name)
            : base(context, ns, new ResourceName(collection, name))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity&lt;TResource&gt;"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk response atom feed.
        /// </param>
        protected BaseEntity(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the
        /// <see cref= "BaseEntity&lt;TResource&gt;"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not intended to
        /// be used directly from your code.
        /// </remarks>
        protected BaseEntity()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets an object representing the Splunk resource at the time it
        /// was last retrieved by the current entity.
        /// </summary>
        /// <value>
        /// An object representing the Splunk resource at the time it was last
        /// retrieved by the current entity.
        /// </value>
        protected TResource Snapshot
        {
            get { return this.snapshot; }
            set { this.snapshot = value; }
        }

        /// <summary>
        /// Gets the dynamic.
        /// </summary>
        /// <value>
        /// The dynamic.
        /// </value>
        public dynamic Dynamic
        {
            get { return this.Snapshot; }
        }
        
        /// <inheritdoc/>
        public Version GeneratorVersion
        {
            get { return this.Snapshot.GeneratorVersion; }
        }

        /// <inheritdoc/>
        public Uri Id
        {
            get { return this.Snapshot.Id; }
        }

        /// <inheritdoc/>
        public string Title
        {
            get { return this.Snapshot.GetValue("Title"); }
        }

        /// <inheritdoc/>
        public DateTime Updated
        {
            get { return this.Snapshot.Updated; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates an entity from a Splunk atom feed
        /// <see cref= "Response"/>.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Type of the entity.
        /// </typeparam>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="response">
        /// An object representing a Splunk atom feed response.
        /// </param>
        /// <returns>
        /// The <see cref="BaseEntity&lt;TResource&gt;"/> created.
        /// </returns>
        internal static async Task<TEntity> CreateAsync<TEntity>(Context context, Response response) 
            where TEntity : BaseEntity<TResource>, new()
        {
            var reader = response.XmlReader;
            TEntity resourceEndpoint;

            reader.Requires(await reader.MoveToDocumentElementAsync("feed", "entry").ConfigureAwait(false));

            if (reader.Name == "entry")
            {
                var entry = new AtomEntry();

                await entry.ReadXmlAsync(reader).ConfigureAwait(false);
                resourceEndpoint = new TEntity();
                resourceEndpoint.Initialize(context, entry, new Version(0, 0));
            }
            else
            {
                var feed = new AtomFeed();

                await feed.ReadXmlAsync(reader).ConfigureAwait(false);
                resourceEndpoint = new TEntity();
                resourceEndpoint.Initialize(context, feed);
            }

            return resourceEndpoint;
        }

        /// <summary>
        /// Updates the content of the current <see cref= "BaseEntity&lt;TResource&gt;"/>.
        /// </summary>
        /// <param name="entry">
        /// A Splunk <see cref="AtomEntry"/>.
        /// </param>
        /// <param name="generatorVersion">
        /// The version of the generator producing the <see cref="AtomFeed"/> feed
        /// containing <paramref name="entry"/>.
        /// </param>
        protected abstract void CreateSnapshot(AtomEntry entry, Version generatorVersion);

        /// <summary>
        /// Updates the <see cref="Snapshot"/> of the current
        /// <see cref= "BaseEntity&lt;TResource&gt;"/> in derived types.
        /// </summary>
        /// <remarks>
        /// Derived types must implement this method.
        /// </remarks>
        /// <param name="feed">
        /// A Splunk <see cref="AtomFeed"/>.
        /// </param>
        protected abstract void CreateSnapshot(AtomFeed feed);

        /// <summary>
        /// Updates the content of the current <see cref= "BaseEntity&lt;TResource&gt;"/> in derived types.
        /// </summary>
        /// <remarks>
        /// Derived types must implement this method.
        /// </remarks>
        /// <param name="resource">
        /// An object representing a Splunk resource.
        /// </param>
        protected virtual void CreateSnapshot(TResource resource)
        {
            Contract.Requires<ArgumentNullException>(resource != null);
            this.snapshot = resource;
        }

        /// <summary>
        /// Infrastructure. Initializes the current uninitialized
        /// <see cref= "BaseEntity&lt;TResource&gt;"/>. class.
        /// </summary>
        /// <remarks>
        /// This method may be called once to initialize a <see cref="BaseEntity&lt;TResource&gt;"/>
        /// instantiated by the default constructor. Override this method to provide
        /// special initialization code. Call this base method before initialization
        /// is complete.
        /// <note type="note">
        /// This method supports the Splunk client infrastructure and is not intended
        /// to be used directly from your code.
        /// </note>
        /// </remarks>
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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/>, <paramref name="entry"/>, or
        /// <paramref name="generatorVersion"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The current <see cref="BaseEntity&lt;TResource&gt;"/> is already initialized.
        /// </exception>
        protected internal void Initialize(Context context, AtomEntry entry, Version generatorVersion)
        {
            Contract.Requires<ArgumentNullException>(generatorVersion != null);
            Contract.Requires<ArgumentNullException>(context != null);
            Contract.Requires<ArgumentNullException>(entry != null);

            this.CreateSnapshot(entry, generatorVersion);
            this.Initialize(context, this.Snapshot.Id);
        }

        /// <summary>
        /// Infrastructure. Initializes the current uninitialized
        /// <see cref= "BaseEntity&lt;TResource&gt;"/>. class.
        /// </summary>
        /// <remarks>
        /// This method may be called once to initialize a <see cref="BaseEntity&lt;TResource&gt;"/>
        /// instantiated by the default constructor. Override this method to provide
        /// special initialization code. Call this base method before initialization
        /// is complete.
        /// <note type="note">
        /// This method supports the Splunk client infrastructure and is not intended
        /// to be used directly from your code.
        /// </note>
        /// </remarks>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk response atom feed.
        /// </param>
        protected internal void Initialize(Context context, AtomFeed feed)
        {
            Contract.Requires<ArgumentNullException>(context != null);
            Contract.Requires<ArgumentNullException>(feed != null);

            this.CreateSnapshot(feed);
            this.Initialize(context, this.Snapshot.Id);
        }

        /// <summary>
        /// Infrastructure. Initializes the current uninitialized
        /// <see cref= "BaseEntity&lt;TResource&gt;"/>. class.
        /// </summary>
        /// <remarks>
        /// This method may be called once to initialize a <see cref="BaseEntity&lt;TResource&gt;"/>
        /// instantiated by the default constructor. Override this method to provide
        /// special initialization code. Call this base method before initialization
        /// is complete.
        /// <note type="note">
        /// This method supports the Splunk client infrastructure and is not intended
        /// to be used directly from your code.
        /// </note>
        /// </remarks>
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
        /// The current <see cref="BaseEntity&lt;TResource&gt;"/> is already initialized.
        /// </exception>
        protected internal void Initialize(Context context, TResource resource)
        {
            Contract.Requires<ArgumentNullException>(resource != null);
            Contract.Requires<ArgumentNullException>(context != null);

            this.CreateSnapshot(resource);
            this.Initialize(context, this.Snapshot.Id);
        }

        /// <summary>
        /// Asynchronously updates the content of the current
        /// <see cref="BaseEntity&lt;TResource&gt;"/>.
        /// </summary>
        /// <param name="response">
        /// A Splunk atom feed <see cref="Response"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        protected internal virtual async Task<bool> ReconstructSnapshotAsync(Response response)
        {
            Contract.Requires<ArgumentNullException>(response != null);
            var feed = new AtomFeed();

            await feed.ReadXmlAsync(response.XmlReader).ConfigureAwait(false);
            this.CreateSnapshot(feed);

            return true;
        }

        #endregion

        #region Privates/internals

        static readonly TResource MissingResource = CreateMissingResource();
        volatile TResource snapshot = MissingResource;

        static TResource CreateMissingResource()
        {
            var resource = new TResource();
            resource.Initialize(new ExpandoObject());
            return resource;
        }

        #endregion
    }

    /// <summary>
    /// A base entity contract.
    /// </summary>
    /// <typeparam name="TResource">
    /// Type of the resource.
    /// </typeparam>
    /// <seealso cref="T:Splunk.Client.BaseEntity{TResource}"/>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = 
        "Contract classes should be contained in the same C# document as the class they reprsent.")
    ]
    [ContractClassFor(typeof(BaseEntity<>))]
    public abstract class BaseEntityContract<TResource> : BaseEntity<TResource> where TResource : BaseResource, new()
    {
        /// <summary>
        /// Creates a snapshot.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        /// <param name="generatorVersion">
        /// The generator version.
        /// </param>
        protected override void CreateSnapshot(AtomEntry entry, Version generatorVersion)
        {
            Contract.Requires<ArgumentNullException>(entry != null);
            Contract.Requires<ArgumentNullException>(generatorVersion != null);
        }

        /// <summary>
        /// Creates a snapshot.
        /// </summary>
        /// <param name="feed">
        /// An object representing a Splunk atom feed response.
        /// </param>
        protected override void CreateSnapshot(AtomFeed feed)
        {
            Contract.Requires<ArgumentNullException>(feed != null);
        }
    }
}
