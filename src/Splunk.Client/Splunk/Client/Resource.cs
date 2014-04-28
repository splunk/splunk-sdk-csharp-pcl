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

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResource"></typeparam>
    public abstract class Resource<TResource> : IComparable, IComparable<Resource<TResource>>, 
        IEquatable<Resource<TResource>> where TResource : Resource<TResource>, new()
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="namespace">
        /// An object identifying a Splunk service namespace.
        /// </param>
        /// <param name="resourceName">
        /// An object identifying a Splunk resource within <see cref="namespace"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <see cref="context"/>, <see cref="namespace"/>, or <see cref=
        /// "resourceName"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="namespace"/> is not specific.
        /// </exception>
        protected Resource(Context context, Namespace @namespace, ResourceName resourceName)
        {
            Contract.Requires<ArgumentException>(resourceName != null, "resourceName");
            Contract.Requires<ArgumentNullException>(@namespace != null, "namespace");
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentOutOfRangeException>(@namespace.IsSpecific);

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

        #region Properties (stable for the lifetime of an instance)

        /// <summary>
        /// Gets the <see cref="Context"/> instance for the current <see cref=
        /// "Resource&lt;TResource&gt;"/>.
        /// </summary>
        public Context Context
        { get; internal set; }

        /// <summary>
        /// Gets the name of the current <see cref="Resource&lt;TResource&gt;"/>.
        /// </summary>
        public string Name
        {
            get { return this.ResourceName.Title; }
        }

        /// <summary>
        /// Gets the namespace containing the current <see cref=
        /// "Resource&lt;TResource&gt;"/>.
        /// </summary>
        public Namespace Namespace
        { get; private set; }

        /// <summary>
        /// Gets the resource name of the current <see cref=
        /// "Resource&lt;TResource&gt;"/>.
        /// </summary>
        public ResourceName ResourceName
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Compares the specified object with the current <see cref=
        /// "Resource&lt;TResource&gt;"/> instance and indicates whether the 
        /// identity of the current instance precedes, follows, or appears in 
        /// the same position in the sort order as the specified object.
        /// </summary>
        /// <param name="other">
        /// An object to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and
        /// value.
        /// </returns>
        public int CompareTo(object other)
        {
            return this.CompareTo(other as Resource<TResource>);
        }

        /// <summary>
        /// Compares the specified <see cref="Resource&lt;TResource&gt;"/> with 
        /// the current instance and indicates whether the identity of the 
        /// current instance precedes, follows, or appears in the same position
        /// in the sort order as the specified instance.
        /// </summary>
        /// <param name="other">
        /// An instance to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// A signed number indicating the relative values of the current 
        /// instance and <see cref="other"/>.
        /// </returns>
        public int CompareTo(Resource<TResource> other)
        {
            if (other == null)
            {
                return 1;
            }

            if (object.ReferenceEquals(this, other))
            {
                return 0;
            }

            return this.ResourceName.CompareTo(other.ResourceName);
        }

        protected internal virtual void Initialize(Context context, AtomEntry entry)
        {
            Contract.Requires<ArgumentNullException>(context != null);
            Contract.Requires<ArgumentNullException>(entry != null);

            if (this.initialized)
            {
                throw new InvalidOperationException(); // TODO: diagnostics
            }

            // Compute namespace and resource name from entry.Id

            var path = entry.Id.AbsolutePath.Split('/');

            if (path.Length < 3)
            {
                throw new InvalidDataException(); // TODO: Diagnostics
            }

            for (int i = 0; i < path.Length; i++)
            {
                path[i] = Uri.UnescapeDataString(path[i]);
            }

            Namespace @namespace;
            ResourceName resourceName;

            switch (path[1])
            {
                case "services":

                    @namespace = Namespace.Default;
                    resourceName = new ResourceName(new ArraySegment<string>(path, 2, path.Length - 2));
                    break;

                case "servicesNS":

                    if (path.Length < 5)
                    {
                        throw new InvalidDataException(); // TODO: Diagnostics
                    }

                    @namespace = new Namespace(user: path[2], app: path[3]);
                    resourceName = new ResourceName(new ArraySegment<string>(path, 4, path.Length - 4));
                    break;

                default: throw new InvalidDataException(); // TODO: Diagnostics
            }

            this.Context = context;
            this.Namespace = @namespace;
            this.ResourceName = resourceName;

            this.initialized = true;
        }

        /// <summary>
        /// Determines whether the specified <see cref=
        /// "Resource&lt;TResource&gt;"/> refers to the same resource as the
        /// current one.
        /// </summary>
        /// <param name="other">
        /// The <see cref="Resource&lt;TResource&gt;"/> to compare with the current
        /// instance.
        /// </param>
        /// <returns>
        /// A value of <c>true</c> if the two instances represent the same
        /// <see cref="Resource&lt;TResource&gt;"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            return this.Equals(other as Resource<TResource>);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Resource&lt;TResource&gt;"/>
        /// refers to the same resource as the current one.
        /// </summary>
        /// <param name="other">
        /// The <see cref="Resource&lt;TResource&gt;"/> to compare with the current
        /// instance.
        /// </param>
        /// <returns>
        /// A value of <c>true</c> if the two instances represent the same
        /// <see cref="Resource&lt;TResource&gt;"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Resource<TResource> other)
        {
            if (other == null)
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            bool result = this.ResourceName == other.ResourceName;
            return result;
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="Resource&lt;TResource&gt;"/>.
        /// </summary>
        /// <returns>
        /// Hash code for the current <see cref="Resource&lt;TResource&gt;"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Gets a string identifying the current <see cref=
        /// "Resource&lt;TResource&gt;"/>.
        /// </summary>
        /// <returns>
        /// A string representing the identity of the current <see cref=
        /// "Resource&lt;TResource&gt;"/>.
        /// </returns>
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
