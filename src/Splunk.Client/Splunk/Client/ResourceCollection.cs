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
    using System.Dynamic;

    /// <summary>
    /// Provides a base class that represents a Splunk resource as an object.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.BaseResource"/>
    public class ResourceCollection : BaseResource
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceCollection"/> class.
        /// </summary>
        /// <param name="entry">
        /// An object representing a Splunk atom entry response.
        /// </param>
        /// <param name="generatorVersion">
        /// The version of the generator producing the <see cref="AtomFeed"/>
        /// feed containing <paramref name="entry"/>.
        /// </param>
        protected internal ResourceCollection(AtomEntry entry, Version generatorVersion)
            : base(entry, generatorVersion)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceCollection"/> class.
        /// </summary>
        /// <param name="feed">
        /// An object representing a Splunk atom feed response.
        /// </param>
        protected internal ResourceCollection(AtomFeed feed)
        {
            this.Initialize(feed);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceCollection"/> class.
        /// </summary>
        /// <param name="expandObject">
        /// An object containing the dynamic members of the newly created
        /// <see cref="ResourceCollection"/>.
        /// </param>
        protected ResourceCollection(ExpandoObject expandObject)
            : base(expandObject)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the
        /// <see cref= "ResourceCollection"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not intended to
        /// be used directly from your code.
        /// </remarks>
        public ResourceCollection()
        { }

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected internal override void Initialize(AtomEntry entry, Version generatorVersion)
        {
            BaseResource.Initialize<ResourceCollection>(this, entry, generatorVersion);
        }

        /// <summary>
        /// Infrastructure. Initializes the current uninitialized
        /// <see cref= "ResourceCollection"/>.
        /// </summary>
        /// <remarks>
        /// This method may be called once to initialize a
        /// <see cref="ResourceCollection"/>
        /// instantiated by the default constructor. Override this method to provide
        /// special initialization code. Call this base method before initialization
        /// is complete.
        /// <note type="note">
        /// This method supports the Splunk client infrastructure and is not intended
        /// to be used directly from your code.
        /// </note>
        /// </remarks>
        /// <param name="feed">
        /// An object representing a Splunk atom feed response.
        /// </param>
        /// <seealso cref="M:Splunk.Client.BaseResource.Initialize(AtomFeed)"/>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="feed"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The current <see cref="ResourceCollection"/> is already initialized.
        /// </exception>
        protected internal override void Initialize(AtomFeed feed)
        {
            BaseResource.Initialize<ResourceCollection, Resource>(this, feed);
        }

        /// <summary>
        /// Infrastructure. Initializes the current uninitialized
        /// <see cref= "ResourceCollection"/>.
        /// </summary>
        /// <remarks>
        /// This method may be called once to initialize a
        /// <see cref="ResourceCollection"/>
        /// instantiated by the default constructor. Override this method to provide
        /// special initialization code. Call this base method before initialization
        /// is complete.
        /// <note type="note">
        /// This method supports the Splunk client infrastructure and is not intended
        /// to be used directly from your code.
        /// </note>
        /// </remarks>
        /// <typeparam name="TResource">
        /// Type of the resource.
        /// </typeparam>
        /// <param name="feed">
        /// An object representing a Splunk atom feed response.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="feed"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The current <see cref="ResourceCollection"/> is already initialized.
        /// </exception>
        protected internal virtual void Initialize<TResource>(AtomFeed feed) where TResource : BaseResource, new()
        {
            BaseResource.Initialize<ResourceCollection, TResource>(this, feed);
        }

        /// <summary>
        /// Gets a string identifying the current <see cref="ResourceCollection"/>.
        /// </summary>
        /// <returns>
        /// A string representing the identity of the current
        /// <see cref= "ResourceCollection"/>.
        /// </returns>
        /// <seealso cref="M:Splunk.Client.BaseResource.ToString()"/>
        public override string ToString()
        {
            return this.Id.ToString();
        }

        #endregion
    }
}
