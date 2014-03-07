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
// [ ] Diagnostics

namespace Splunk.Sdk
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class SavedSearchArgs : IDictionary<string, object>
    {
        #region Constructors

        public SavedSearchArgs()
        {
            this.dictionary = new Dictionary<string, object>();
        }

        #endregion

        #region Properties

        public object this[string key]
        {
            get { return this.dictionary[key]; }
            set { this.dictionary[key] = value; }
        }

        public int Count
        { get { return this.dictionary.Count; } }

        public bool IsReadOnly
        { get { return false; } }

        public ICollection<string> Keys
        { get { return this.dictionary.Keys; } }

        public ICollection<object> Values
        { get { return this.dictionary.Values; } }

        #endregion

        #region Methods

        public void Add(string key, object value)
        {
            this.dictionary.Add(key, value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            this.dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            this.dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return this.dictionary.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return this.dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            
            if (array.Length - index < this.Count)
            {
                throw new ArgumentException("array");
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            foreach (var item in this.dictionary)
            {
                array[index++] = item;
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var item in this.dictionary)
            {
                yield return new KeyValuePair<string, object>("args." + item.Key, item.Value);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
 
        public bool Remove(string key)
        {
            return this.dictionary.Remove(key);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return this.Remove(item);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (KeyValuePair<string, object> arg in this)
            {
                builder.Append("args.");
                builder.Append(arg.Key);
                builder.Append('=');
                builder.Append(arg.Value);
                builder.Append("; ");
            }

            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 2;
            }

            return builder.ToString();
        }

        public bool TryGetValue(string key, out object value)
        {
            return this.dictionary.TryGetValue(key, out value);
        }

        #endregion

        #region privates

        Dictionary<string, object> dictionary;

        #endregion
    }
}
