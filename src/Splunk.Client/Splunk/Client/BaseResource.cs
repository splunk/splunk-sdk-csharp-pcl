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
    public abstract class BaseResource : ExpandoAdapter
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseResource"/> class.
        /// </summary>
        /// <param name="entry">
        /// An object representing a Splunk atom entry response.
        /// </param>
        /// <param name="generatorVersion">
        /// The version of the generator producing the <see cref="AtomFeed"/>
        /// feed containing <paramref name="entry"/>.
        /// </param>
        protected internal BaseResource(AtomEntry entry, Version generatorVersion)
        {
            this.Initialize(entry, generatorVersion);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseResource"/> class.
        /// </summary>
        /// <param name="feed">
        /// An object representing a Splunk atom feed response.
        /// </param>
        protected internal BaseResource(AtomFeed feed)
        {
            this.Initialize(feed);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseResource"/> class.
        /// </summary>
        /// <param name="expandObject">
        /// An object containing the dynamic members of the newly created
        /// <see cref="BaseResource"/>.
        /// </param>
        protected BaseResource(ExpandoObject expandObject)
            : base(expandObject)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "BaseResource"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. 
        /// </remarks>
        public BaseResource()
        { }

        #endregion

        #region Properties
        
        /// <summary>
        /// 
        /// </summary>
        public Version GeneratorVersion
        {
            get { return this.GetValue("GeneratorVersion"); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Uri Id
        {
            get { return this.GetValue("Id"); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            get { return this.GetValue("Title"); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Updated
        {
            get { return this.GetValue("Updated"); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected ExpandoAdapter Content
        {
            get { return this.GetValue("Content", ExpandoAdapter.Converter.Instance) ?? ExpandoAdapter.Empty; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal IReadOnlyList<BaseResource> Resources
        {
            get { return this.GetValue("Resources") ?? NoResources; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates a <see cref="BaseResource"/> from a Splunk atom
        /// feed <see cref="Response"/>.
        /// </summary>
        /// <typeparam name="TResource">
        /// The type of <see cref="BaseResource"/> to be created.
        /// </typeparam>
        /// <param name="response">
        /// An object representing a Splunk atom feed response.
        /// </param>
        /// <returns>
        /// The <see cref="BaseResource"/> created.
        /// </returns>
        internal static async Task<TResource> CreateAsync<TResource>(Response response) where TResource : BaseResource, new()
        {
            var feed = new AtomFeed();

            await feed.ReadXmlAsync(response.XmlReader);
            var resource = new TResource();
            resource.Initialize(feed);

            return resource;
        }

        /// <summary>
        /// Determines whether the specified <see cref="BaseResource"/> refers to 
        /// the same resource as the current one.
        /// </summary>
        /// <param name="other">
        /// The <see cref="BaseResource"/> to compare with the current one.
        /// </param>
        /// <returns>
        /// A value of <c>true</c> if the two instances represent the same
        /// <see cref="BaseResource"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            return this.Equals(other as BaseResource);
        }

        /// <summary>
        /// Determines whether the specified <see cref="BaseResource"/> refers to 
        /// the same resource as the current one.
        /// </summary>
        /// <param name="other">
        /// The <see cref="BaseResource"/> to compare with the current one.
        /// </param>
        /// <returns>
        /// A value of <c>true</c> if the two instances represent the same
        /// <see cref="BaseResource"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(BaseResource other)
        {
            if (other == null)
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            bool result = this.Id.Equals(other.Id);
            return result;
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="BaseResource"/>.
        /// </summary>
        /// <returns>
        /// Hash code for the current <see cref="BaseResource"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        /// <summary>
        /// Infrastructure. Initializes the current uninitialized <see cref=
        /// "BaseResource"/>.
        /// class.
        /// </summary>
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
        /// The current <see cref="BaseResource"/> is already initialized.
        /// </exception>
        /// <remarks>
        /// This method may be called once to intialize a <see cref="BaseResource"/>
        /// instantiated by the default constructor. Override this method to 
        /// provide special initialization code. Call this base method before 
        /// initialization is complete. 
        /// <note type="note">
        /// This method supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code.
        /// </note>
        /// </remarks>
        protected internal abstract void Initialize(AtomEntry entry, Version generatorVersion);

        /// <summary>
        /// Infrastructure. Initializes the current uninitialized <see cref=
        /// "BaseResource"/>.
        /// </summary>
        /// <param name="feed">
        /// An object representing a Splunk atom feed response.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="feed"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The current <see cref="BaseResource"/> is already initialized.
        /// </exception>
        /// <remarks>
        /// This method may be called once to intialize a <see cref="BaseResource"/>
        /// instantiated by the default constructor. Override this method to 
        /// provide special initialization code. Call this base method before 
        /// initialization is complete. 
        /// <note type="note">
        /// This method supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code.
        /// </note>
        /// </remarks>
        protected internal abstract void Initialize(AtomFeed feed);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="resource"></param>
        /// <param name="entry"></param>
        /// <param name="generatorVersion"></param>
        /// <returns></returns>
        protected internal static TResource Initialize<TResource>(TResource resource, AtomEntry entry,
            Version generatorVersion)
            where TResource : BaseResource, new()
        {
            Contract.Requires<ArgumentNullException>(entry != null);
            Contract.Requires<ArgumentNullException>(generatorVersion != null);

            resource.EnsureUninitialized();

            dynamic expando = new ExpandoObject();

            if (entry.Content != null)
            {
                var content = entry.Content as ExpandoObject;

                if (content == null)
                {
                    expando.Value = entry.Content;
                }
                else
                {
                    expando.Content = entry.Content;
                }
            }

            expando.GeneratorVersion = generatorVersion;
            expando.Id = entry.Id;
            expando.Title = entry.Title;
            expando.Updated = entry.Updated;

            if (entry.Author != null)
            {
                expando.Author = entry.Author;
            }

            if (entry.Links.Count > 0)
            {
                expando.Links = entry.Links;
            }

            if (entry.Published != DateTime.MinValue)
            {
                expando.Published = entry.Published;
            }

            resource.Object = expando;
            resource.MarkInitialized();

            return resource;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="resource"></param>
        /// <param name="entry"></param>
        /// <param name="generatorVersion"></param>
        /// <returns></returns>
        protected internal static TCollection Initialize<TCollection, TResource>(TCollection collection, AtomFeed feed)
            where TCollection : BaseResource, new()
            where TResource : BaseResource, new()
        {
            Contract.Requires<ArgumentNullException>(feed != null);
            collection.EnsureUninitialized();

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

            if (feed.Entries != null)
            {
                var resources = new List<TResource>();

                foreach (var entry in feed.Entries)
                {
                    var resource = new TResource();

                    resource.Initialize(entry, feed.GeneratorVersion);
                    resources.Add(resource);
                }

                expando.Resources = new ReadOnlyCollection<TResource>(resources);
            }

            collection.Object = expando;
            collection.MarkInitialized();
            
            return collection;
        }

        /// <summary>
        /// Gets a string identifying the current <see cref="BaseResource"/>.
        /// </summary>
        /// <returns>
        /// A string representing the identity of the current <see cref=
        /// "BaseResource"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Id.ToString();
        }

        #endregion

        #region Privates/internals

        static readonly IReadOnlyList<BaseResource> NoResources = new ReadOnlyCollection<Resource>(new Resource[0]);

        bool initialized;

        void EnsureUninitialized()
        {
            if (this.initialized)
            {
                throw new InvalidOperationException("Resource was intialized; Initialize operation may not execute again.");
            }
        }

        /// <summary>
        /// Infrastructure. Initializes the current uninitialized resource.
        /// </summary>
        /// <param name="feed">
        /// An object representing a Splunk atom feed response.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="feed"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The current <see cref="BaseResource"/> is already initialized.
        /// </exception>
        /// <remarks>
        /// This method may be called once to intialize a <see cref="BaseResource"/>
        /// instantiated by the default constructor. Override this method to 
        /// provide special initialization code. Call this base method before 
        /// initialization is complete. 
        /// <note type="note">
        /// This method supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code.
        /// </note>
        /// </remarks>
        internal void Initialize(ExpandoObject @object)
        {
            Contract.Requires<ArgumentNullException>(@object != null);

            this.EnsureUninitialized();
            this.Object = @object;
            this.MarkInitialized();
        }

        void MarkInitialized()
        {
            initialized = true;
        }

        #endregion
    }
}
