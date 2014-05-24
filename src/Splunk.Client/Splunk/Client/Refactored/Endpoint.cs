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

namespace Splunk.Client.Refactored
{
    using Splunk.Client;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a base class that represents a Splunk endpoint as an object.
    /// </summary>
    public class Endpoint : IComparable, IComparable<Endpoint>, IEquatable<Endpoint>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Endpoint"/> class.
        /// </summary>
        /// <param name="service">
        /// An object representing a Splunk service endpoint.
        /// <param name="name">
        /// An object identifying a Splunk resource within <paramref name=
        /// "service"/>.<see cref="Namespace"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> or <paramref name="name"/> are <c>null</c>.
        protected internal Endpoint(Service service, ResourceName name)
            : this(service.Context, service.Namespace, name)
        {
            Contract.Requires<ArgumentNullException>(service != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Endpoint"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="name">
        /// An object identifying a Splunk resource within <paramref name="ns"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/>, <paramref name="ns"/>, or <paramref name=
        /// "name"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ns"/> is not specific.
        /// </exception>
        protected internal Endpoint(Context context, Namespace ns, ResourceName name)
        {
            Contract.Requires<ArgumentException>(name != null);
            Contract.Requires<ArgumentNullException>(ns != null);
            Contract.Requires<ArgumentNullException>(context != null);
            Contract.Requires<ArgumentOutOfRangeException>(ns.IsSpecific);

            this.context = context;
            this.ns = ns;
            this.resourceName = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="Context"/> instance for the current <see cref=
        /// "Endpoint"/>.
        /// </summary>
        public Context Context
        {
            get { return this.context; }
        }

        /// <summary>
        /// Gets the title of the current <see cref="Endpoint"/> which is the 
        /// final part of <see cref="ResourceName"/>.
        /// </summary>
        public string Name
        {
            get { return this.ResourceName.Title; }
        }

        /// <summary>
        /// Gets the namespace containing the current <see cref="Endpoint"/>.
        /// </summary>
        public Namespace Namespace
        {
            get { return this.ns; }
        }

        /// <summary>
        /// Gets the name of the current <see cref="Endpoint"/>.
        /// </summary>
        public ResourceName ResourceName
        {
            get { return this.resourceName; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Compares the specified object with the current <see cref=
        /// "Endpoint"/> instance and indicates whether the identity of the 
        /// current instance precedes, follows, or appears in the same position
        /// in the sort order as the specified object.
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
            return this.CompareTo(other as Endpoint);
        }

        /// <summary>
        /// Compares the specified <see cref="Endpoint"/> with the current 
        /// instance and indicates whether the identity of the current instance
        /// precedes, follows, or appears in the same position
        /// in the sort order as the specified instance.
        /// </summary>
        /// <param name="other">
        /// An instance to compare or <c>null</c>.
        /// </param>
        /// <returns>
        /// A signed number indicating the relative values of the current 
        /// instance and <paramref name="other"/>.
        /// </returns>
        public int CompareTo(Endpoint other)
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

        /// <summary>
        /// Determines whether the specified <see cref="Endpoint"/> refers to 
        /// the same resource as the current one.
        /// </summary>
        /// <param name="other">
        /// The <see cref="Endpoint"/> to compare with the current one.
        /// </param>
        /// <returns>
        /// A value of <c>true</c> if the two instances represent the same
        /// <see cref="Endpoint"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            return this.Equals(other as Endpoint);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Endpoint"/> refers to 
        /// the same resource as the current one.
        /// </summary>
        /// <param name="other">
        /// The <see cref="Endpoint"/> to compare with the current one.
        /// </param>
        /// <returns>
        /// A value of <c>true</c> if the two instances represent the same
        /// <see cref="Endpoint"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Endpoint other)
        {
            if (other == null)
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            bool result = this.ResourceName.Equals(other.ResourceName);
            return result;
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="Endpoint"/>.
        /// </summary>
        /// <returns>
        /// Hash code for the current <see cref="Endpoint"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.ResourceName.GetHashCode();
        }

        /// <summary>
        /// Gets a string identifying the current <see cref="Endpoint"/>.
        /// </summary>
        /// <returns>
        /// A string representing the identity of the current <see cref=
        /// "Endpoint"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Join("/", this.Context.ToString(), this.Namespace.ToString(), this.ResourceName.ToString());
        }

        #endregion

        #region Privates

        Context context;
        Namespace ns;
        ResourceName resourceName;

        #endregion
    }
}
