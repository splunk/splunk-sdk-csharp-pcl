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

namespace Splunk.Sdk
{
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a class that represents a collection of Splunk data indexes.
    /// </summary>
    public class IndexCollection : EntityCollection<IndexCollection, Index>
    {
        #region Constructors

        internal IndexCollection(Context context, Namespace @namespace)
            : base(context, @namespace, ClassResourceName)
        { }

        public IndexCollection()
        { }

        #endregion

        #region Methods

        public Index CreateIndex(string name, IndexArgs args, IndexAttributes attributes)
        {
            return this.CreateIndexAsync(name, args, attributes).Result;
        }

        public async Task<Index> CreateIndexAsync(string name, IndexArgs args, IndexAttributes attributes)
        {
            var entity = new Index(this.Context, this.Namespace, name);
            await entity.CreateAsync(args, attributes);
            return entity;
        }

        public Index GetIndex(string name)
        {
            return this.GetIndexAsync(name).Result;
        }

        public async Task<Index> GetIndexAsync(string name)
        {
            var entity = new Index(this.Context, this.Namespace, name);
            await entity.GetAsync();
            return entity;
        }

        public void RemoveIndex(string name)
        {
            this.RemoveIndexAsync(name).Wait();
        }

        public async Task RemoveIndexAsync(string name)
        {
            var entity = new Index(this.Context, this.Namespace, name);
            await entity.RemoveAsync();
        }

        public void UpdateIndex(string name, IndexAttributes attributes)
        {
            this.UpdateIndexAsync(name, attributes).Wait();
        }

        public async Task UpdateIndexAsync(string name, IndexAttributes attributes)
        {
            var entity = new Index(this.Context, this.Namespace, name);
            await entity.UpdateAsync(attributes);
        }

        #endregion

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("data", "indexes");

        #endregion
    }
}
