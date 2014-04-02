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

// TODO:
// [ ] Check for HTTP Status Code 204 (No Content) and empty atoms in 
//     Resource<TResource>.UpdateAsync.
//
// [O] Contracts
//
// [O] Documentation
//
// [X] Pick up standard properties from AtomEntry on Update, not just AtomEntry.Content
//     See [Splunk responses to REST operations](http://goo.gl/tyXDfs).
//
// [X] Remove Resource<TResource>.Invalidate method
//     FJR: This gets called when we set the record value. Add a comment saying what it's
//     supposed to do when it's overridden.
//     DSN: I've adopted an alternative method for getting strongly-typed values. See, for
//     example, Job.DispatchState or ServerInfo.Guid.

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Threading.Tasks;

    public abstract class Resource<TResource> where TResource : Resource<TResource>, new()
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <param name="namespace">
        /// </param>
        /// <param name="collection">
        /// </param>
        /// <param name="title">
        /// </param>
        protected Resource(Context context, Namespace @namespace, ResourceName resourceName)
        {
            Contract.Requires<ArgumentException>(resourceName != null, "resourceName");
            Contract.Requires<ArgumentNullException>(@namespace != null, "namespace");
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires(@namespace.IsSpecific);

            this.Context = context;
            this.Namespace = @namespace;
            this.ResourceName = resourceName;

            this.initialized = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public Resource()
        { }

        #endregion

        #region Properties that are stable for the lifetime of an instance

        /// <summary>
        /// Gets the <see cref="Context"/> instance for the current <see cref=
        /// "Resource"/>.
        /// </summary>
        public Context Context
        { get; internal set; }

        /// <summary>
        /// Gets the namespace containing the current <see cref="Resource"/>.
        /// </summary>
        public Namespace Namespace
        { get; private set; }

        /// <summary>
        /// Gets the resource name of the current <see cref="Resource"/>.
        /// </summary>
        /// <remarks>
        /// The resource name is the concatenation of <see cref=
        /// "Resource.Collection"/> and <see cref="Resource.Title"/>.
        /// </remarks>
        public ResourceName ResourceName
        { get; private set; }

        /// <summary>
        /// Gets the title of this <see cref="Resource"/>.
        /// </summary>
        public string Title
        {
            get { return this.ResourceName.Title; }
        }

        #endregion

        #region Properties backed by AtomEntry

        public abstract string Author
        { get; }

        public abstract Uri Id
        { get; }

        public abstract IReadOnlyDictionary<string, Uri> Links
        { get; }

        public abstract DateTime Updated
        { get; }

        #endregion

        #region Methods

        public virtual void Initialize(Context context, Namespace @namespace, ResourceName resourceName, object entry)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(@namespace != null, "namespace");
            Contract.Requires<ArgumentNullException>(resourceName != null, "resourceName");

            if (this.initialized)
            {
                throw new InvalidOperationException(); // TODO: diagnostics
            }

            this.Context = context;
            this.Namespace = @namespace;
            this.ResourceName = resourceName;

            this.initialized = true;
        }

        protected static async Task EnsureStatusCodeAsync(Response response, HttpStatusCode expected)
        {
            if (response.Message.StatusCode != expected)
            {
                throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
            }
        }

        /// <summary>
        /// Refreshes the cached state of the current <see cref=
        /// "Resource<TResource>"/>.
        /// </summary>
        public void Get()
        {
            this.GetAsync().Wait();
        }

        /// <summary>
        /// Refreshes the cached state of the current <see cref=
        /// "Resource<TResource>"/>.
        /// </summary>
        #pragma warning disable 1998 // This async method lacks 'await' operators and will run synchronously.
        public virtual async Task GetAsync()
        { throw new NotImplementedException(); }

        /// <summary>
        /// Gets a string identifying the current <see cref="Resource<TResource>"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join("/", this.Context.ToString(), this.Namespace.ToString(), this.ResourceName.ToString());
        }

        #endregion

        #region Privates

        bool initialized;

        #endregion
    }
}
