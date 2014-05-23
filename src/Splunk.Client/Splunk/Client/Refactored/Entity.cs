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

namespace Splunk.Client.Refactored
{
    using Splunk.Client;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Provides an object representation of a Splunk entity.
    /// </summary>
    /// <remarks>
    /// This is the base class for all Splunk entities.
    /// </remarks>
    public class Entity : Resource
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="Entity"/> instance.
        /// </summary>
        /// <param name="service">
        /// An object representing a Splunk service endpoint.
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
        /// Initializes a new instance of the <see cref="Entity"/> 
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
        public Entity(Context context, Namespace ns, ResourceName resourceName)
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
        public Entity(Context context, Namespace ns, ResourceName collection, string name)
            : this(context, ns, new ResourceName(collection, name))
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
        public Entity(Context context, AtomFeed feed)
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

            this.Initialize(context, feed.Entries[0], feed.GeneratorVersion);
        }

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

        #region Methods

        /// <summary>
        /// Asynchronously retrieves a fresh copy of the current <see cref=
        /// "Entity"/> that contains all changes to it since it was last 
        /// retrieved.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public virtual async Task GetAsync()
        {
            using (var response = await this.Context.GetAsync(this.Namespace, this.ResourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                await this.UpdateSnapshotAsync(response);
            }
        }

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
            return this.CurrentSnapshot.GetValue(name, valueConverter);
        }

        /// <summary>
        /// Asynchronously removes the current <see cref="Entity"/> from Splunk.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public virtual async Task RemoveAsync()
        {
            using (var response = await this.Context.DeleteAsync(this.Namespace, this.ResourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Asynchronously updates the attributes of the current <see cref=
        /// "Entity"/> on Splunk.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the <see cref="CurrentSnapshot"/> was also updated.
        /// </returns>
        /// <remarks>
        /// Splunk usually returns an updated snapshot on completion of the
        /// operation. When it does the <see cref="CurrentSnapshot"/> will
        /// also be updated.
        /// </remarks>
        public async Task<bool> UpdateAsync(params Argument[] arguments)
        {
            return await this.UpdateAsync(arguments.AsEnumerable());
        }

        /// <summary>
        /// Asynchronously updates the attributes of the current <see cref=
        /// "Entity"/> on Splunk.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the <see cref="CurrentSnapshot"/> was also updated.
        /// </returns>
        /// <remarks>
        /// Splunk usually returns an updated snapshot on completion of the
        /// operation. When it does the <see cref="CurrentSnapshot"/> will
        /// be updated and returns <c>true</c>; otherwise, this method returns
        /// <c>false</c>.
        /// </remarks>
        public async Task<bool> UpdateAsync(IEnumerable<Argument> arguments)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                return await this.UpdateSnapshotAsync(response);
            }
        }

        /// <summary>
        /// Asynchronously updates the <see cref="CurrentSnapshot"/> for the
        /// current <see cref="Resource"/>
        /// </summary>
        /// <param name="response">
        /// A Splunk atom feed <see cref="Response"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        protected override async Task<bool> UpdateSnapshotAsync(Response response)
        {
            var feed = new AtomFeed();

            await feed.ReadXmlAsync(response.XmlReader);
            int count = feed.Entries.Count;

            if (count == 0)
            {
                return false;
            }

            if (count > 1)
            {
                throw new InvalidDataException(string.Format("Atom feed response contains {0} entries.", count)); // TODO: improve diagnostics
            }

            this.UpdateSnapshotAsync(feed.Entries[0], feed.GeneratorVersion);
            return true;
        }

        #endregion
    }
}
