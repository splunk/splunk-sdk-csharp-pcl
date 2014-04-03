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
    using System.Diagnostics.Contracts;

    public class Argument : IComparable, IComparable<Argument>, IEquatable<Argument>
    {
        public Argument(string name, string value)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            this.Name = name;
            this.Value = value;
        }

        public string Name
        { get; private set; }

        public string Value
        { get; private set; }

        public int CompareTo(object o)
        {
            return this.CompareTo(o as Argument);
        }

        public int CompareTo(Argument other)
        {
            if (other == null)
            {
                return 1;
            }

            if (object.ReferenceEquals(this, other))
            {
                return 0;
            }

            int result = this.Name.CompareTo(other.Name);
            return result != 0 ? result : this.Value.CompareTo(other.Value);
        }

        public override bool Equals(object o)
        {
            return this.Equals(o as Argument);
        }

        public bool Equals(Argument other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Name == other.Name && this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            // TODO: Check this against the algorithm presented in Effective Java
            int hash = 17;

            hash = hash * 23 + this.Name.GetHashCode();
            hash = hash * 23 + this.Value.GetHashCode();

            return hash;
        }

        public override string ToString()
        {
            return string.Join("=", this.Name, this.Value);
        }
    }
}
