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
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Provides custom arguments.
    /// </summary>
    /// <seealso cref="T:System.Collections.Generic.ISet{Splunk.Client.Argument}"/>
    public class ArgumentSet : ISet<Argument>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentSet"/>
        /// class.
        /// </summary>
        /// <param name="argumentPrefix">
        /// The argument prefix.
        /// </param>
        public ArgumentSet(string argumentPrefix)
        {
            this.argumentPrefix = string.IsNullOrEmpty(argumentPrefix) ? null : argumentPrefix;
            this.set = new HashSet<Argument>();
        }

        /// <summary>
        /// Initializes a new instance of the Splunk.Client.ArgumentSet class.
        /// </summary>
        /// <param name="argumentPrefix">
        /// The argument prefix.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        public ArgumentSet(string argumentPrefix, params Argument[] arguments)
            : this(argumentPrefix, arguments.AsEnumerable())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentSet"/>
        /// class from a collection of <see cref="Argument"/> values.
        /// </summary>
        /// <param name="argumentPrefix">
        /// The argument prefix.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        public ArgumentSet(string argumentPrefix, IEnumerable<Argument> arguments)
        {
            Contract.Requires<ArgumentNullException>(arguments != null);
            
            this.argumentPrefix = string.IsNullOrEmpty(argumentPrefix) ? null : argumentPrefix;
            this.set = new HashSet<Argument>(arguments);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the argument prefix.
        /// </summary>
        /// <value>
        /// The argument prefix.
        /// </value>
        public string ArgumentPrefix
        {
            get { return this.argumentPrefix; }
        }

        /// <summary>
        /// Gets the number of. 
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count
        { 
            get { return this.set.Count; } 
        }

        /// <summary>
        /// Gets a value indicating whether this object is read only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is read only, <c>false</c> if not.
        /// </value>
        public bool IsReadOnly
        { 
            get { return false; } 
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clears this object to its blank/initial state.
        /// </summary>
        public void Clear()
        {
            this.set.Clear();
        }

        /// <summary>
        /// Adds item.
        /// </summary>
        /// <param name="item">
        /// 
        /// </param>
        /// <returns>
        /// <c>true</c> if it succeeds, <c>false</c> if it fails.
        /// </returns>
        public bool Add(Argument item)
        {
            return this.set.Add(item);
        }

        void ICollection<Argument>.Add(Argument item)
        {
            this.set.Add(item);
        }

        /// <summary>
        /// Query if this object contains the given item.
        /// </summary>
        /// <param name="item">
        /// 
        /// </param>
        /// <returns>
        /// <c>true</c> if the object is in this collection, <c>false</c> if not.
        /// </returns>
        public bool Contains(Argument item)
        {
            return this.set.Contains(item);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">
        /// 
        /// </param>
        /// <param name="index">
        /// 
        /// </param>
        public void CopyTo(Argument[] array, int index)
        {
            this.set.CopyTo(array, index);
        }

        /// <summary>
        /// Except with.
        /// </summary>
        /// <param name="other">
        /// 
        /// </param>
        public void ExceptWith(IEnumerable<Argument> other)
        {
            this.set.ExceptWith(other);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        public IEnumerator<Argument> GetEnumerator()
        {
            if (this.argumentPrefix == null)
            {
                foreach (var item in this.set)
                {
                    yield return item;
                }
            }
            else
            {
                foreach (var item in this.set)
                {
                    yield return new Argument(this.argumentPrefix + item.Name, item.Value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Intersect with.
        /// </summary>
        /// <param name="other">
        /// 
        /// </param>
        public void IntersectWith(IEnumerable<Argument> other)
        {
            this.set.IntersectWith(other);
        }

        /// <summary>
        /// Is proper subset of the given other.
        /// </summary>
        /// <param name="other">
        /// 
        /// </param>
        /// <returns>
        /// <c>true</c> if proper subset of, <c>false</c> if not.
        /// </returns>
        public bool IsProperSubsetOf(IEnumerable<Argument> other)
        {
            return this.IsProperSubsetOf(other);
        }

        /// <summary>
        /// Is proper superset of the given other.
        /// </summary>
        /// <param name="other">
        /// 
        /// </param>
        /// <returns>
        /// <c>true</c> if proper subset of, <c>false</c> if not.
        /// </returns>
        public bool IsProperSupersetOf(IEnumerable<Argument> other)
        {
            return this.IsProperSupersetOf(other);
        }

        /// <summary>
        /// Is subset of the given other.
        /// </summary>
        /// <param name="other">
        /// 
        /// </param>
        /// <returns>
        /// <c>true</c> if subset of, <c>false</c> if not.
        /// </returns>
        public bool IsSubsetOf(IEnumerable<Argument> other)
        {
            return this.set.IsSubsetOf(other);
        }

        /// <summary>
        /// Is superset of the given other.
        /// </summary>
        /// <param name="other">
        /// 
        /// </param>
        /// <returns>
        /// <c>true</c> if superset of, <c>false</c> if not.
        /// </returns>
        public bool IsSupersetOf(IEnumerable<Argument> other)
        {
            return this.set.IsSupersetOf(other);
        }

        /// <summary>
        /// Query if this object overlaps the given other.
        /// </summary>
        /// <param name="other">
        /// 
        /// </param>
        /// <returns>
        /// <c>true</c> if it succeeds, <c>false</c> if it fails.
        /// </returns>
        public bool Overlaps(IEnumerable<Argument> other)
        {
            return this.set.Overlaps(other);
        }

        /// <summary>
        /// Removes the given item.
        /// </summary>
        /// <param name="item">
        /// 
        /// </param>
        /// <returns>
        /// <c>true</c> if it succeeds, <c>false</c> if it fails.
        /// </returns>
        public bool Remove(Argument item)
        {
            return this.set.Remove(item);
        }

        /// <summary>
        /// Sets the equals.
        /// </summary>
        /// <param name="other">
        /// 
        /// </param>
        /// <returns>
        /// <c>true</c> if it succeeds, <c>false</c> if it fails.
        /// </returns>
        public bool SetEquals(IEnumerable<Argument> other)
        {
            return this.SetEquals(other);
        }

        /// <summary>
        /// Symmetric except with.
        /// </summary>
        /// <param name="other">
        /// 
        /// </param>
        public void SymmetricExceptWith(IEnumerable<Argument> other)
        {
            this.set.SymmetricExceptWith(other);
        }

        /// <summary>
        /// Gets a string representation for the current <see cref="ArgumentSet"/>.
        /// </summary>
        /// <returns>
        /// A string representation of the current <see cref="ArgumentSet"/>.
        /// </returns>
        /// <seealso cref="M:System.Object.ToString()"/>
        public override string ToString()
        {
            return string.Join("; ", from arg in this select arg.ToString());
        }

        /// <summary>
        /// Union with.
        /// </summary>
        /// <param name="other">
        /// 
        /// </param>
        public void UnionWith(IEnumerable<Argument> other)
        {
            this.set.UnionWith(other);
        }

        #endregion

        #region Privates

        readonly string argumentPrefix;
        readonly HashSet<Argument> set;

        #endregion
    }
}
