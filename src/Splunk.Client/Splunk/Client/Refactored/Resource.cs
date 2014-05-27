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

namespace Splunk.Client.Refactored
{
    using Splunk.Client;
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
    public class Resource : ExpandoAdapter
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        /// <param name="entry">
        /// An object representing a Splunk atom entry response.
        /// </param>
        /// <param name="generatorVersion">
        /// The version of the generator producing the <see cref="AtomFeed"/>
        /// feed containing <paramref name="entry"/>.
        /// </param>
        protected internal Resource(AtomEntry entry, Version generatorVersion)
        {
            this.Initialize(entry, generatorVersion);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        /// <param name="feed">
        /// An object representing a Splunk atom feed response.
        /// </param>
        protected internal Resource(AtomFeed feed)
        {
            this.Initialize(feed);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        /// <param name="other">
        /// Another resource.
        /// </param>
        protected internal Resource(Resource other)
        {
            this.Initialize(other);
        }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "Resource"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. 
        /// </remarks>
        public Resource()
        { }

        #endregion

        #region Properties

        public Version GeneratorVersion
        {
            get { return this.GetValue("GeneratorVersion"); }
        }

        public Uri Id
        {
            get { return this.GetValue("Id"); }
        }

        public string Title
        {
            get { return this.GetValue("Title"); }
        }

        public DateTime Updated
        {
            get { return this.GetValue("Updated"); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates a <see cref="Resource"/> from a Splunk atom
        /// feed <see cref="Response"/>.
        /// </summary>
        /// <typeparam name="TResource">
        /// The type of <see cref="Resource"/> to be created.
        /// </typeparam>
        /// <param name="response">
        /// An object representing a Splunk atom feed response.
        /// </param>
        /// <returns>
        /// The <see cref="Resource"/> created.
        /// </returns>
        internal static async Task<TResource> CreateAsync<TResource>(Response response) where TResource : Resource, new()
        {
            var feed = new AtomFeed();

            await feed.ReadXmlAsync(response.XmlReader);
            var resource = new TResource();
            resource.Initialize(feed);

            return resource;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Resource"/> refers to 
        /// the same resource as the current one.
        /// </summary>
        /// <param name="other">
        /// The <see cref="Resource"/> to compare with the current one.
        /// </param>
        /// <returns>
        /// A value of <c>true</c> if the two instances represent the same
        /// <see cref="Resource"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            return this.Equals(other as Resource);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Resource"/> refers to 
        /// the same resource as the current one.
        /// </summary>
        /// <param name="other">
        /// The <see cref="Resource"/> to compare with the current one.
        /// </param>
        /// <returns>
        /// A value of <c>true</c> if the two instances represent the same
        /// <see cref="Resource"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Resource other)
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
        /// Returns the hash code for the current <see cref="Resource"/>.
        /// </summary>
        /// <returns>
        /// Hash code for the current <see cref="Resource"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        /// <summary>
        /// Infrastructure. Initializes the current uninitialized <see cref=
        /// "Resource"/>.
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
        /// The current <see cref="Resource"/> is already initialized.
        /// </exception>
        /// <remarks>
        /// This method may be called once to intialize a <see cref="Resource"/>
        /// instantiated by the default constructor. Override this method to 
        /// provide special initialization code. Call this base method before 
        /// initialization is complete. 
        /// <note type="note">
        /// This method supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code.
        /// </note>
        /// </remarks>
        protected internal void Initialize(AtomEntry entry, Version generatorVersion)
        {
            Contract.Requires<ArgumentNullException>(entry != null);
            Contract.Requires<ArgumentNullException>(generatorVersion != null);
            this.EnsureUninitialized();

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

            this.Expando = expando;
            this.MarkInitialized();
        }

        /// <summary>
        /// Infrastructure. Initializes the current uninitialized <see cref=
        /// "Resource"/>.
        /// </summary>
        /// <param name="feed">
        /// An object representing a Splunk atom feed response.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="feed"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The current <see cref="Resource"/> is already initialized.
        /// </exception>
        /// <remarks>
        /// This method may be called once to intialize a <see cref="Resource"/>
        /// instantiated by the default constructor. Override this method to 
        /// provide special initialization code. Call this base method before 
        /// initialization is complete. 
        /// <note type="note">
        /// This method supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code.
        /// </note>
        /// </remarks>
        protected internal void Initialize(AtomFeed feed)
        {
            Contract.Requires<ArgumentNullException>(feed != null);
            this.EnsureUninitialized();

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

            var resources = new List<Resource>();

            foreach (var entry in feed.Entries)
            {
                var resource = new Resource(entry, feed.GeneratorVersion);
                resources.Add(resource);
            }

            expando.Resources = new ReadOnlyCollection<Resource>(resources);
            this.Expando = expando;
            this.MarkInitialized();
        }

        /// <summary>
        /// Infrastructure. Initializes the current uninitialized <see cref=
        /// "Resource"/>.
        /// </summary>
        /// <param name="feed">
        /// An object representing a Splunk atom feed response.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="feed"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The current <see cref="Resource"/> is already initialized.
        /// </exception>
        /// <remarks>
        /// This method may be called once to intialize a <see cref="Resource"/>
        /// instantiated by the default constructor. Override this method to 
        /// provide special initialization code. Call this base method before 
        /// initialization is complete. 
        /// <note type="note">
        /// This method supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code.
        /// </note>
        /// </remarks>
        protected internal void Initialize(Resource other)
        {
            Contract.Requires<ArgumentNullException>(other != null);
            
            this.EnsureUninitialized();
            this.Expando = other.Expando;
            this.MarkInitialized();
        }

        /// <summary>
        /// Gets a string identifying the current <see cref="Resource"/>.
        /// </summary>
        /// <returns>
        /// A string representing the identity of the current <see cref=
        /// "Resource"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Id.ToString();
        }

        #endregion

        #region Privates/internals

        protected internal static readonly Resource Missing = new Resource();
        bool initialized;

        void EnsureUninitialized()
        {
            if (this.initialized)
            {
                throw new InvalidOperationException("Resource was intialized; Initialize operation may not execute again.");
            }
        }

        void MarkInitialized()
        {
            initialized = true;
        }

        #endregion
    }
}
