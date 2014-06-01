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
//// [X] ResourceEndpoint is an Endpoint that aggregates Resource

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a base class that represents a Splunk resource as an object.
    /// </summary>
    public class ResourceCollection<TResource> : BaseResource where TResource : BaseResource, new()
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
        /// Initializes a new instance of the <see cref="ResourceCollection"/> class.
        /// </summary>
        /// <param name="other">
        /// Another resource.
        /// </param>
        protected internal ResourceCollection(ResourceCollection<TResource> other)
            : base(other)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "ResourceCollection"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. 
        /// </remarks>
        public ResourceCollection()
        { }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        protected internal static readonly ResourceCollection<TResource> Missing =
            new ResourceCollection<TResource>(new ExpandoObject());

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        protected internal IReadOnlyCollection<TResource> Entries
        {
            get { return this.resources; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Infrastructure. Initializes the current uninitialized <see cref=
        /// "ResourceCollection"/>.
        /// </summary>
        /// <param name="feed">
        /// An object representing a Splunk atom feed response.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="feed"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The current <see cref="ResourceCollection"/> is already initialized.
        /// </exception>
        /// <remarks>
        /// This method may be called once to intialize a <see cref="ResourceCollection"/>
        /// instantiated by the default constructor. Override this method to 
        /// provide special initialization code. Call this base method before 
        /// initialization is complete. 
        /// <note type="note">
        /// This method supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code.
        /// </note>
        /// </remarks>
        protected internal override void Initialize(AtomFeed feed)
        {
            Contract.Requires<ArgumentNullException>(feed != null);
            //this.EnsureUninitialized();

            dynamic expando = new ExpandoObject();

            expando.GeneratorVersion = feed.GeneratorVersion;
            expando.Id = feed.Id;
            expando.Title = feed.Title;
            expando.Updated = feed.Updated;

            expando.Author = feed.Author;

            if (feed.Links != null)
            {
                expando.Links = feed.Links;
            }

            if (feed.Messages != null)
            {
                expando.Messages = feed.Messages;
            }

            if (!feed.Pagination.Equals(Pagination.None))
            {
                expando.Pagination = feed.Pagination;
            }

            var resources = new List<TResource>();

            if (feed.Entries != null)
            {
                foreach (var entry in feed.Entries)
                {
                    var resource = new TResource();
                    resource.Initialize(entry, feed.GeneratorVersion);
                    resources.Add(resource);
                }
            }

            this.resources = new ReadOnlyCollection<TResource>(resources);
            this.Object = expando;
            
            //this.MarkInitialized();
        }

        /// <summary>
        /// Gets a string identifying the current <see cref="ResourceCollection"/>.
        /// </summary>
        /// <returns>
        /// A string representing the identity of the current <see cref=
        /// "ResourceCollection"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Id.ToString();
        }

        #endregion

        #region Privates/internals

        IReadOnlyList<TResource> resources;

        #endregion
    }
}
