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
// [ ] Documentation
// [ ] Diagnostics

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    public sealed class SavedSearchTemplateArgs : ISet<Argument>
    {
        #region Constructors

        public SavedSearchTemplateArgs()
        {
            this.set = new HashSet<Argument>();
        }

        public SavedSearchTemplateArgs(IEnumerable<Argument> collection)
        {
            Contract.Requires<ArgumentNullException>(collection != null, "collection");
            this.set = new HashSet<Argument>(collection);
        }

        #endregion

        #region Properties

        public int Count
        { get { return this.set.Count; } }

        public bool IsReadOnly
        { get { return false; } }

        #endregion

        #region Methods

        public void Clear()
        {
            this.set.Clear();
        }

        public IEnumerator<Argument> GetEnumerator()
        {
            foreach (var item in this.set)
            {
                yield return new Argument("args." + item.Name, item.Value);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
 
        public override string ToString()
        {
            return string.Join("; ", from arg in this select arg.ToString());
        }

        #endregion

        #region privates

        HashSet<Argument> set;

        #endregion

        public bool Add(Argument item)
        {
            return this.set.Add(item);
        }

        public void ExceptWith(IEnumerable<Argument> other)
        {
            this.set.ExceptWith(other);
        }

        public void IntersectWith(IEnumerable<Argument> other)
        {
            this.set.IntersectWith(other);
        }

        public bool IsProperSubsetOf(IEnumerable<Argument> other)
        {
            return this.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<Argument> other)
        {
            return this.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<Argument> other)
        {
            return this.set.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<Argument> other)
        {
            return this.set.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<Argument> other)
        {
            return this.set.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<Argument> other)
        {
            return this.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<Argument> other)
        {
            this.set.SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<Argument> other)
        {
            this.set.UnionWith(other);
        }

        void ICollection<Argument>.Add(Argument item)
        {
            this.set.Add(item);
        }

        public bool Contains(Argument item)
        {
            return this.set.Contains(item);
        }

        public void CopyTo(Argument[] array, int index)
        {
            this.set.CopyTo(array, index);
        }

        public bool Remove(Argument item)
        {
            return this.set.Remove(item);
        }

        IEnumerator<Argument> IEnumerable<Argument>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
