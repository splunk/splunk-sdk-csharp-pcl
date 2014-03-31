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
// [ ] Contracts
// [ ] Documentation

namespace Splunk.Sdk
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class ResourceName : IComparable, IComparable<ResourceName>, IEquatable<ResourceName>, IReadOnlyList<string>
    {
        #region Constructors

        public ResourceName(ResourceName resourceName, params string[] parts)
            : this(resourceName.Concat(parts))
        { }

        public ResourceName(params string[] parts)
            : this(parts.AsEnumerable<string>())
        { }

        ResourceName(IEnumerable<string> parts)
        {
            this.parts = parts.ToArray();
        }

        #endregion

        #region Fields

        public static readonly ResourceName AppsLocal = new ResourceName("apps", "local");
        public static readonly ResourceName AuthLogin = new ResourceName("auth", "login");
        public static readonly ResourceName AuthorizationCapabilities = new ResourceName("authorization", "capabilities");
        public static readonly ResourceName Configs = new ResourceName("configs");
        public static readonly ResourceName DataIndexes = new ResourceName("data", "indexes");
        public static readonly ResourceName Properties = new ResourceName("properties");
        public static readonly ResourceName SavedSearches = new ResourceName("saved", "searches");
        public static readonly ResourceName SearchJobs = new ResourceName("search", "jobs");
        public static readonly ResourceName SearchJobsExport = new ResourceName("search", "jobs", "export");
        public static readonly ResourceName ServerInfo = new ResourceName("server", "info");
        
        #endregion

        #region Properties

        public string this[int index]
        {
	        get { return this.parts[index]; }
        }

        public int Count
        {
            get { return this.parts.Count; }
        }

        #endregion

        #region Methods

        public int CompareTo(object other)
        {
            return this.CompareTo(other as ResourceName);
        }

        public int CompareTo(ResourceName other)
        {
            if (other == null)
            {
                return 1;
            }

            if (object.ReferenceEquals(this, other))
            {
                return 0;
            }

            int diff = this.parts.Count - other.parts.Count;

            if (diff != 0)
            {
                return diff;
            }

            var pair = this.parts
                .Zip(other.parts, (p1, p2) => new { ThisPart = p1, OtherPart = p2 })
                .First(p => p.ThisPart != p.OtherPart);

            if (pair == null)
            {
                return 0;
            }

            return pair.ThisPart.CompareTo(pair.OtherPart);
        }

        public override bool Equals(object other)
        {
            return this.Equals(other as ResourceName);
        }

        public bool Equals(ResourceName other)
        {
            if (other == null)
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.parts.Count != other.parts.Count)
            {
                return false;
            }

            return this.parts.SequenceEqual(other.parts);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return this.parts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.parts.GetEnumerator();
        }

        public override int GetHashCode()
        {
            // TODO: Check this against the algorithm presented in Effective Java
            return this.parts.Aggregate(seed: 17, func: (value, part) => value * 23 + part.GetHashCode());
        }

        /// <summary>
        /// Converts the value of the current <see cref="Namespace"/> to its
        /// equivalent string representation.
        /// </summary>
        /// <returns>
        /// A string representation of the current <see cref="Namespace"/>
        /// </returns>
        public override string ToString()
        {
            return string.Join("/", from segment in this select segment);
        }

        /// <summary>
        /// Converts the value of the current <see cref="Namespace"/> object to
        /// its equivalent URI encoded string representation.
        /// </summary>
        /// <returns>
        /// A string representation of the current <see cref="Namespace"/>
        /// </returns>
        /// <remarks>
        /// The value is converted using <see cref="Uri.EscapeUriString"/>.
        /// </remarks>
        public string ToUriString()
        {
            return string.Join("/", from segment in this select Uri.EscapeDataString(segment));
        }

        #endregion

        #region Privates

        readonly IReadOnlyList<string> parts;

        #endregion
    }
}
