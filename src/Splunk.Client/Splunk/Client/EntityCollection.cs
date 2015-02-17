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
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a base class for representing a collection of Splunk resources.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/TDthxd">Accessing Splunk resources</a>,
    ///   especially "Other actions for Splunk REST API endpoints".
    /// </description></item>
    /// <item><description>
    ///   <a href="http://goo.gl/oc65Bo">REST API Reference</a>.
    /// </description></item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TEntity">
    /// Type of the entity.
    /// </typeparam>
    /// <typeparam name="TResource">
    /// Type of the resource.
    /// </typeparam>
    /// <seealso cref="T:Splunk.Client.BaseEntity{Splunk.Client.ResourceCollection}"/>
    /// <seealso cref="T:Splunk.Client.IEntityCollection{TEntity,TResource}"/>
    public class EntityCollection<TEntity, TResource> : BaseEntity<ResourceCollection>, IEntityCollection<TEntity, TResource> 
        where TEntity : BaseEntity<TResource>, new()
        where TResource : BaseResource, new()
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="EntityCollection&lt;TEntity,TResource&gt;"/> instance.
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
        protected internal EntityCollection(Service service, ResourceName name)
            : base(service, name)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollection&lt;TEntity,TResource&gt;"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="name">
        /// An object identifying a Splunk entity collection within
        /// <paramref name="ns"/>.
        /// </param>
        protected internal EntityCollection(Context context, Namespace ns, ResourceName name)
            : base(context, ns, name)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollection&lt;TEntity,TResource&gt;"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="entry">
        /// A entry in a Splunk atom feed response.
        /// </param>
        /// <param name="generatorVersion">
        /// The generator version.
        /// </param>
        protected internal EntityCollection(Context context, AtomEntry entry, Version generatorVersion)
        {
            // We cannot use the base constructor because it will use base.CreateEntity, not this.CreateEntity
            this.Initialize(context, entry, generatorVersion);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollection&lt;TEntity,TResource&gt;"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk atom feed response.
        /// </param>
        protected internal EntityCollection(Context context, AtomFeed feed)
        {
            // We cannot use the base constructor because it will use base.CreateResource, not this.CreateResource
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref= "EntityCollection&lt;TEntity,TResource&gt;"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not intended to
        /// be used directly from your code.
        /// </remarks>
        public EntityCollection()
        { }

        #endregion

        #region Properties

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
            get { return this.Create((TResource)this.Snapshot.Resources[index]); }
        }

        /// <summary>
        /// Gets the number of entries in the current
        /// <see cref="EntityCollection&lt;TEntity,TResource&gt;"/>.
        /// </summary>
        /// <value>
        /// The number of entries in the current
        /// <see cref="EntityCollection&lt;TEntity,TResource&gt;"/>.
        /// </value>
        public int Count
        {
            get { return this.Snapshot.Resources.Count; }
        }

        #endregion

        #region Methods

        #region Operational interface

        /// <summary>
        /// Asynchronously creates a new Splunk entity.
        /// </summary>
        /// <param name="arguments">
        /// Arguments to the Splunk REST API for creating the desired entity.
        /// </param>
        /// <returns>
        /// An object representing the entity created.
        /// </returns>
        public virtual async Task<TEntity> CreateAsync(params Argument[] arguments)
        {
            return await this.CreateAsync(arguments.AsEnumerable()).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously creates a new Splunk entity.
        /// </summary>
        /// <param name="arguments">
        /// Arguments to the Splunk REST API for creating the desired entity.
        /// </param>
        /// <returns>
        /// An object representing the entity created.
        /// </returns>
        public virtual async Task<TEntity> CreateAsync(IEnumerable<Argument> arguments)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, arguments).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created).ConfigureAwait(false);
                return await BaseEntity<TResource>.CreateAsync<TEntity>(this.Context, response).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously retrieves a <see cref="Entity&lt;TResource&gt;"/> in the current
        /// <see cref="EntityCollection&lt;TEntity,TResource&gt;"/> by name.
        /// </summary>
        /// <param name="name">
        /// Name of the entity to retrieve.
        /// </param>
        /// <returns>
        /// The entity retrieved.
        /// </returns>
        public virtual async Task<TEntity> GetAsync(string name)
        {
            var resourceName = new ResourceName(this.ResourceName, name);

            using (Response response = await this.Context.GetAsync(this.Namespace, resourceName).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
                return await BaseEntity<TResource>.CreateAsync<TEntity>(this.Context, response).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously retrieves a fresh copy of the full list of entities in the
        /// current <see cref="EntityCollection&lt;TEntity,TResource&gt;"/>.
        /// </summary>
        /// <remarks>
        /// Following completion of the operation the list of entites in the current
        /// <see cref="EntityCollection&lt;TEntity,TREsource&gt;"/> will contain all changes
        /// since the list was last retrieved.
        /// </remarks>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public virtual async Task GetAllAsync()
        {
            using (var response = await this.Context.GetAsync(this.Namespace, this.ResourceName, GetAll).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
                await this.ReconstructSnapshotAsync(response).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously retrieves an entity in the current collection by name.
        /// </summary>
        /// <param name="name">
        /// The name of the entity to retrieve.
        /// </param>
        /// <returns>
        /// An object representing entity <paramref name="name"/> or <c>null</c>,
        /// if no such entity exists.
        /// </returns>
        public virtual async Task<TEntity> GetOrNullAsync(string name)
        {
            var resourceName = new ResourceName(this.ResourceName, name);

            using (Response response = await this.Context.GetAsync(this.Namespace, resourceName).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK, HttpStatusCode.NotFound).ConfigureAwait(false);
                TEntity resourceEndpoint = null;

                if (response.Message.StatusCode == HttpStatusCode.OK)
                {
                    resourceEndpoint = await BaseEntity<TResource>.CreateAsync<TEntity>(this.Context, response).ConfigureAwait(false);
                }

                return resourceEndpoint;
            }
        }

        /// <summary>
        /// Asynchronously retrieves select entities from the list of entites in the
        /// current <see cref="EntityCollection&lt;TEntity,TResource&gt;"/>.
        /// </summary>
        /// <remarks>
        /// Following completion of the operation the list of entities in the current
        /// <see cref="EntityCollection&lt;TEntity,TResource&gt;"/> will contain all changes
        /// since the select entites were last retrieved.
        /// </remarks>
        /// <param name="arguments">
        /// A variable-length parameters list containing arguments.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public virtual async Task GetSliceAsync(params Argument[] arguments)
        {
            await this.GetSliceAsync(arguments.AsEnumerable()).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves select entities from the list of entites in the
        /// current <see cref="EntityCollection&lt;TEntity,TResource&gt;"/>.
        /// </summary>
        /// <remarks>
        /// Following completion of the operation the list of entities in the current
        /// <see cref="EntityCollection&lt;TEntity,TResource&gt;"/> will contain all changes
        /// since the select entites were last retrieved.
        /// </remarks>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public virtual async Task GetSliceAsync(IEnumerable<Argument> arguments)
        {
            using (Response response = await this.Context.GetAsync(this.Namespace, this.ResourceName).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
                await this.ReconstructSnapshotAsync(response).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously forces the Splunk server to reload data for the current
        /// <see cref="EntityCollection&lt;TEntity,TResource&gt;"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public async Task ReloadAsync()
        {
            var reload = new ResourceName(this.ResourceName, "_reload");

            using (Response response = await this.Context.GetAsync(this.Namespace, reload).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
            }
        }

        #endregion

        #region IReadOnlyList<TEntity> methods

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator that iterates through the current <see cref=
        /// "EntityCollection&lt;TEntity,TResource&gt;"/>.
        /// </summary>
        /// <returns>
        /// An object for iterating through the current
        /// <see cref= "EntityCollection&lt;TEntity,TResource&gt;"/>.
        /// </returns>
        public IEnumerator<TEntity> GetEnumerator()
        {
            return this.Snapshot.Resources.Select(resource => {
                return Create((TResource)resource);
            }).GetEnumerator();
        }

        #endregion

        #region Infrastructure methods

        /// <inheritdoc/>
        protected override void CreateSnapshot(AtomEntry entry, Version generatorVersion)
        {
            this.Snapshot = new ResourceCollection();
            this.Snapshot.Initialize(entry, generatorVersion);
        }

        /// <inheritdoc/>
        protected override void CreateSnapshot(AtomFeed feed)
        {
            this.Snapshot = new ResourceCollection();
            this.Snapshot.Initialize<TResource>(feed);
        }

        /// <inheritdoc/>
        protected override void CreateSnapshot(ResourceCollection resource)
        {
            this.Snapshot = resource;
        }

        #endregion

        #endregion

        #region Privates/internals

        internal static readonly ReadOnlyCollection<Message> NoMessages = new ReadOnlyCollection<Message>(new List<Message>());
        static readonly Argument[] GetAll = new Argument[] { new Argument("count", 0) };

        TEntity Create(TResource resource)
        {
            var entity = new TEntity();
            
            entity.Initialize(this.Context, resource);
            return entity;
        }

        #endregion
    }
}
