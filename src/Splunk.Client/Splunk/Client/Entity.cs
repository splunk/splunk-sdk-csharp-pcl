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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a Splunk entity.
    /// </summary>
    /// <remarks>
    /// This is the base class for all Splunk entities.
    /// </remarks>
    public class Entity<TResource> : BaseEntity<TResource>, IEntity where TResource : BaseResource, new()
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// <param name="name">
        /// An object identifying a Splunk resource within <paramref name=
        /// "service"/>.<see cref="Namespace"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> or <paramref name="name"/> are <c>null</c>.
        protected internal Entity(Service service, ResourceName name)
            : base(service.Context, service.Namespace, name)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
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
        protected internal Entity(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> 
        /// class.
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
        protected internal Entity(Context context, Namespace ns, ResourceName resourceName)
            : base(context, ns, resourceName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
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
        protected internal Entity(Context context, Namespace ns, ResourceName collection, string name)
            : this(context, ns, new ResourceName(collection, name))
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref="Entity"/> 
        /// class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        public Entity()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        protected ExpandoAdapter Content
        {
            get { return this.GetValue("Content", ExpandoAdapter.Converter.Instance) ?? ExpandoAdapter.Empty; }
        }

        #endregion

        #region Methods

        #region Operational interface

        /// <inheritdoc/>
        public virtual async Task GetAsync()
        {
            using (var response = await this.Context.GetAsync(this.Namespace, this.ResourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                await this.ReconstructSnapshotAsync(response);
            }
        }

        /// <inheritdoc/>
        public virtual async Task RemoveAsync()
        {
            using (var response = await this.Context.DeleteAsync(this.Namespace, this.ResourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        /// <inheritdoc/>
        public virtual async Task<bool> UpdateAsync(params Argument[] arguments)
        {
            return await this.UpdateAsync(arguments.AsEnumerable());
        }

        /// <inheritdoc/>
        public virtual async Task<bool> UpdateAsync(IEnumerable<Argument> arguments)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, arguments))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                var reader = response.XmlReader;

                await reader.MoveToDocumentElementAsync("feed", "response");

                if (reader.Name == "response")
                {
                    return false;
                }

                return await this.ReconstructSnapshotAsync(response);
            }
        }

        #endregion

        #region Infrastructure methods

        /// <summary>
        /// Gets a converted property value from the <see cref="CurrentSnapshot"/>
        /// of the current <see cref="Entity"/>.
        /// </summary>
        /// <param name="name">
        /// The name of a property.
        /// </param>
        /// <param name="valueConverter">
        /// A value converter for converting property <paramref name="name"/>.
        /// </param>
        /// <returns>
        /// The converted value or <paramref name="valueConverter"/><c>.DefaultValue</c>,
        /// if <paramref name="name"/> does not exist.
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// The conversion failed.
        /// </exception>
        /// <remarks>
        /// Use this method to create static properties from the dynamic 
        /// properties exposed by the <see cref="CurrentSnapshot"/>.
        /// </remarks>
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
            int count = feed.Entries.Count;

            if (count == 0)
            {
                return;
            }

            if (count > 1)
            {
                throw new InvalidDataException(string.Format("Atom feed response contains {0} entries.", count)); // TODO: improve diagnostics
            }

            this.CreateSnapshot(feed.Entries[0], feed.GeneratorVersion);
        }

        #endregion

        #endregion
    }
}
