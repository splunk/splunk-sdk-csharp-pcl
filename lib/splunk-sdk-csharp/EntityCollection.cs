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

namespace Splunk.Sdk
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class EntityCollection<T> : IReadOnlyList<T> where T : Entity
    {
        internal EntityCollection(Context context, Namespace @namespace, ResourceName name, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            this.Context = context;
            this.Namespace = @namespace;
            this.Name = name;

            if (parameters != null)
            {
                this.Parameters = parameters.ToDictionary(pair => pair.Key, pair => pair.Value);
            }
        }

        #region Properties

        /// <summary>
        /// Gets the <see cref="Context"/> instance for this <see cref="EntityCollection"/>.
        /// </summary>
        public Context Context
        { get; private set; }

        /// <summary>
        /// Gets the number of entites in this <see cref="EntityCollection"/>.
        /// </summary>
        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the resource name of this <see cref="EntityCollection"/>.
        /// </summary>
        public ResourceName Name
        { get; private set; }

        /// <summary>
        /// Gets the <see cref="Namespace"/> containing this <see cref="EntityCollection"/>.
        /// </summary>
        public Namespace Namespace
        { get; private set; }

        public T this[int index]
        {
            get { throw new NotImplementedException(); }
        }

        public IReadOnlyDictionary<string, object> Parameters
        { get; private set; }

        #endregion

        #region Methods

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

       IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public async Task Refresh()
        {
            XDocument document = await this.Context.GetDocument(this.Namespace, this.Name, this.Parameters);
            // TODO: Convert to object model
        }
        
        #endregion
    }
}
