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
//// [O] Documentation
//// [ ] Diagnostics

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
    public class ArgumentSet : ISet<Argument>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentSet"/>
        /// class.
        /// </summary>
        public ArgumentSet(string argumentPrefix)
        {
            this.argumentPrefix = string.IsNullOrEmpty(argumentPrefix) ? null : argumentPrefix;
            this.set = new HashSet<Argument>();
        }

        public ArgumentSet(string argumentPrefix, params Argument[] arguments)
            : this(argumentPrefix, arguments.AsEnumerable())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentSet"/>
        /// class from a collection of <see cref="Argument"/> values.
        /// </summary>
        public ArgumentSet(string argumentPrefix, IEnumerable<Argument> arguments)
        {
            Contract.Requires<ArgumentNullException>(arguments != null);
            
            this.argumentPrefix = string.IsNullOrEmpty(argumentPrefix) ? null : argumentPrefix;
            this.set = new HashSet<Argument>(arguments);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string ArgumentPrefix
        {
            get { return this.argumentPrefix; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        { 
            get { return this.set.Count; } 
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        { 
            get { return false; } 
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            this.set.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Add(Argument item)
        {
            return this.set.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void ICollection<Argument>.Add(Argument item)
        {
            this.set.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(Argument item)
        {
            return this.set.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Argument[] array, int index)
        {
            this.set.CopyTo(array, index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void ExceptWith(IEnumerable<Argument> other)
        {
            this.set.ExceptWith(other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void IntersectWith(IEnumerable<Argument> other)
        {
            this.set.IntersectWith(other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsProperSubsetOf(IEnumerable<Argument> other)
        {
            return this.IsProperSubsetOf(other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsProperSupersetOf(IEnumerable<Argument> other)
        {
            return this.IsProperSupersetOf(other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsSubsetOf(IEnumerable<Argument> other)
        {
            return this.set.IsSubsetOf(other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsSupersetOf(IEnumerable<Argument> other)
        {
            return this.set.IsSupersetOf(other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Overlaps(IEnumerable<Argument> other)
        {
            return this.set.Overlaps(other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(Argument item)
        {
            return this.set.Remove(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool SetEquals(IEnumerable<Argument> other)
        {
            return this.SetEquals(other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
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
        public override string ToString()
        {
            return string.Join("; ", from arg in this select arg.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
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
