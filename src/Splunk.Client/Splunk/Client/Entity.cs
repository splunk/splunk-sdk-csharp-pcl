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

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
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
    /// <seealso cref="T:Splunk.Client.BaseEntity{TResource}"/>
    /// <seealso cref="T:Splunk.Client.IEntity"/>
    public class Entity<TResource> : BaseEntity<TResource>, IEntity where TResource : BaseResource, new()
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity&lt;TResource&gt;"/> class.
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
        protected internal Entity(Service service, ResourceName name)
            : base(service.Context, service.Namespace, name)
        {
            Contract.Requires<ArgumentNullException>(service != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity&lt;TResource&gt;"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk response atom feed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="feed"/> are
        /// <c>null</c>.
        /// </exception>
        protected internal Entity(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity&lt;TResource&gt;"/> class.
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
        protected internal Entity(Context context, Namespace ns, ResourceName resourceName)
            : base(context, ns, resourceName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity&lt;TResource&gt;"/> class.
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
        /// The name of an entity within <paramref name="collection"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/>, <paramref name="ns"/>, or <paramref
        /// name="collection"/>, or <paramref name="name"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ns"/> is not specific.
        /// </exception>
        protected internal Entity(Context context, Namespace ns, ResourceName collection, string name)
            : this(context, ns, new ResourceName(collection, name))
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref="Entity&lt;TResource&gt;"/>
        /// class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not intended to
        /// be used directly from your code.
        /// </remarks>
        public Entity()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public virtual ExpandoAdapter Content
        {
            get { return this.GetValue("Content", ExpandoAdapter.Converter.Instance) ?? ExpandoAdapter.Empty; }
        }

        #endregion

        #region Methods

        #region Operational interface

        /// <inheritdoc/>
        public virtual async Task GetAsync()
        {
            using (var response = await this.Context.GetAsync(this.Namespace, this.ResourceName).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
                await this.ReconstructSnapshotAsync(response).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public virtual async Task<bool> SendAsync(HttpMethod method, string action, params Argument[] arguments)
        {
            var resourceName = new ResourceName(this.ResourceName, action);

            using (var response = await this.Context.SendAsync(method, this.Namespace, resourceName, arguments).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
                var reader = response.XmlReader;

                await reader.MoveToDocumentElementAsync("feed", "entry", "response").ConfigureAwait(false);

                if (reader.Name == "response")
                {
                    return false;
                }

                return await this.ReconstructSnapshotAsync(response).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public virtual async Task RemoveAsync()
        {
            using (var response = await this.Context.DeleteAsync(this.Namespace, this.ResourceName).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public virtual async Task<bool> UpdateAsync(params Argument[] arguments)
        {
            return await this.UpdateAsync(arguments.AsEnumerable()).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<bool> UpdateAsync(IEnumerable<Argument> arguments)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, arguments).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
                var reader = response.XmlReader;

                await reader.MoveToDocumentElementAsync("feed", "entry", "response").ConfigureAwait(false);

                if (reader.Name == "response")
                {
                    return false;
                }

                return await this.ReconstructSnapshotAsync(response).ConfigureAwait(false);
            }
        }

        #endregion

        #region Infrastructure methods

        /// <summary>
        /// Gets a converted property value from the current snapshot of the
        /// current <see cref="Entity&lt;TResource&gt;"/>.
        /// </summary>
        /// <remarks>
        /// Use this method to create static properties from the dynamic properties
        /// exposed by the current snapshot.
        /// </remarks>
        /// <typeparam name="TValue">
        /// Type of the value.
        /// </typeparam>
        /// <param name="name">
        /// The name of a property.
        /// </param>
        /// <param name="valueConverter">
        /// A value converter for converting property <paramref name="name"/>.
        /// </param>
        /// <returns>
        /// The converted value or
        /// <paramref name="valueConverter"/><c>.DefaultValue</c>, if
        /// <paramref name="name"/> does not exist.
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// The conversion failed.
        /// </exception>
        protected TValue GetValue<TValue>(string name, ValueConverter<TValue> valueConverter)
        {
            return this.Snapshot.GetValue(name, valueConverter);
        }

        /// <inheritdoc/>
        protected override void CreateSnapshot(AtomEntry entry, Version generatorVersion)
        {
            this.Snapshot = new TResource();
            this.Snapshot.Initialize(entry, generatorVersion);
        }

        /// <inheritdoc/>
        protected override void CreateSnapshot(AtomFeed feed)
        {
            if (feed.Entries == null || feed.Entries.Count == 0)
            {
                return;
            }

            int count = feed.Entries.Count;

            if (count > 1)
            {
                var text = string.Format(CultureInfo.CurrentCulture, "Atom feed response contains {0} entries.", count);
                throw new InvalidDataException(text);
            }

            this.CreateSnapshot(feed.Entries[0], feed.GeneratorVersion);
        }

        #endregion

        #endregion
    }
}
