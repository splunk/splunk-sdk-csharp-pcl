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
// [O] Property accessors should not throw, but return default value if the underlying field is undefined (?)

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;

    public sealed class ServerMessageCollection : Entity<ServerMessageCollection>, IReadOnlyDictionary<string, string>
    {
        #region Constructors

        public ServerMessageCollection(Context context, Namespace @namespace)
            : base(context, @namespace, ClassResourceName)
        { }

        public ServerMessageCollection()
        { }

        #endregion

        #region Properties

        public string this[string key]
        {
            get { return (string)((IDictionary<string, object>)this.Content)[key]; }
        }

        public int Count
        {
            get { return ((IDictionary<string, object>)this.Content).Count; }
        }

        public IEnumerable<string> Keys
        {
            get { return ((IDictionary<string, object>)this.Content).Keys;  }
        }

        public IEnumerable<string> Values
        {
            get 
            { 
                var values = ((IDictionary<string, object>)this.Content).Values;
                
                foreach (var value in values)
                {
                    yield return (string)value; 
                }
            }
        }

        #endregion

        #region Methods

        public bool ContainsKey(string key)
        {
            return ((IDictionary<string, object>)this.Content).ContainsKey(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            object o;
            
            if (((IDictionary<string, object>)this.Content).TryGetValue(key, out o))
            {
                value = (string)o;
                return true;
            }

            value = null;
            return false;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            var dictionary = (IDictionary<string, object>)this.Content;

            foreach (var item in dictionary)
            {
                yield return new KeyValuePair<string, string>(item.Key, (string)item.Value);
            }
          
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("messages");

        #endregion
    }
}
